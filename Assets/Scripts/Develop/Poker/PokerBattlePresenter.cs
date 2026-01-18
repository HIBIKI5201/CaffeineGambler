using Develop.Player;
using UniRx;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Develop.Poker
{
    /// <summary>
    /// プレイヤー・敵のカードプレゼンターとボタン入力を束ね、賭け金処理〜勝敗演出までを制御する。
    /// 引き直しは1回のみ許可し、引き直し後は自動でバトルを実行する。
    /// </summary>
    public class PokerBattlePresenter : MonoBehaviour
    {
        [SerializeField] private PokerGameManager _gameManager;                 // 山札生成・役判定を担うマネージャー。
        [SerializeField] private CardPresenter _playerPresenter;                // プレイヤーのカード描画／選択操作担当。
        [SerializeField] private CardPresenter _enemyPresenter;                 // 敵カードの描画担当（基本的に選択不可）。
        [SerializeField] private TextMeshProUGUI _resultLabel;                  // 勝敗や役結果を表示するラベル。
        [SerializeField] private Button _dealButton;                            // 配札ボタン（ラウンド開始時の初期化）。
        [SerializeField] private Button _battleButton;                          // 手動で勝負を確定させるボタン。
        [SerializeField] private PokerRankPayoutTable _payoutTable;             // 役ごとの倍率を定義した ScriptableObject。
        [SerializeField] private TMP_InputField _betInputField;                 // UI 上で賭け金を入力するフィールド。
        [SerializeField][Min(0)] private int _betAmount = 10;                   // 現在の賭け金（所持金を超える場合は自動で丸め込む）。
        [SerializeField] private string _drawLabel = "Draw";                    // 引き分け時の表示テキスト。
        [SerializeField] private string _playerWinLabel = "Player Wins!";       // プレイヤー勝利時の表示テキスト。
        [SerializeField] private string _enemyWinLabel = "Enemy Wins!";         // 敵勝利時の表示テキスト。
        [SerializeField] private string _insufficientFundsMessage = "Not enough money to place a bet.";

        private readonly CompositeDisposable _disposables = new();
        private PlayerData _playerData;
        private bool _battleResolvedThisRound;                                  // このラウンドで既に決着済みかどうか（2重決済を防ぐ）。

        private void Awake()
        {
            SetupButtonSubscriptions();                                         // 配札／バトルボタンの購読登録。
            SetupBetInputBinding();                                             // ベット入力欄の onEndEdit ハンドリング。
        }

        private void OnEnable() => SubscribePlayerRedrawEvent();                // プレイヤーの引き直しイベントを購読。
        private void OnDisable() => UnsubscribePlayerRedrawEvent();             // 無効化時にイベント購読を解除。
        private void OnDestroy() => _disposables.Dispose();                     // UniRx 購読をまとめて破棄。 

        /// <summary>
        /// 所持金モデルを受け取り、Money ストリームと賭け金入力を同期させる。
        /// </summary>
        public void Initialize(PlayerData playerData)
        {
            _playerData = playerData;

            _playerData?.Money
                .Subscribe(OnPlayerMoneyChanged)
                .AddTo(_disposables);

            ClampBetToAffordableAmount();
            SyncBetInputToCurrentBet();
        }

        /// <summary>外部 UI から賭け金を直接変更したい場合のエントリポイント。</summary>
        public void SetBetAmount(int amount) => SetBetAmountInternal(amount, true);

        /// <summary>ディールボタン押下時の公開 API。</summary>
        public void OnDealBothButton() => DealBothHands();

        /// <summary>バトルボタン押下時の公開 API。</summary>
        public void OnBattleButton() => ExecuteBattle();

        /// <summary>
        /// 勝負処理の本体。ラウンド決着済みか、手札が揃っているか、賭け金が支払えるかを順番に判定し、
        /// 条件を満たしたら配当処理と演出更新を実行する。
        /// </summary>
        private void ExecuteBattle()
        {
            if (_battleResolvedThisRound)
            {
                Debug.LogWarning("Battle has already been resolved this round.");
                return;
            }

            if (!CanBattle())
            {
                return;
            }

            if (!TryConsumeBet(out var betAmount))
            {
                return;
            }

            var snapshot = ResolveBattleSequence();
            ApplyBattlePayout(snapshot, betAmount);
            UpdateBattleResult(snapshot);
            _enemyPresenter?.SetRevealState(true);

            _battleResolvedThisRound = true;                                    // このラウンドで再実行されないようロック。
        }

        /// <summary>
        /// 配札／バトルボタンを UniRx で購読し、二重登録を防ぎながら処理に紐づける。
        /// </summary>
        private void SetupButtonSubscriptions()
        {
            _dealButton?
                .OnClickAsObservable()
                .Subscribe(_ => DealBothHands())
                .AddTo(_disposables);

            _battleButton?
                .OnClickAsObservable()
                .Subscribe(_ => ExecuteBattle())
                .AddTo(_disposables);
        }

        /// <summary>
        /// 賭け金入力欄の onEndEdit を購読し、ユーザー入力 → 内部値更新 → フィールド同期の流れを構築する。
        /// </summary>
        private void SetupBetInputBinding()
        {
            if (_betInputField == null)
            {
                return;
            }

            _betInputField.onEndEdit
                .AsObservable()
                .Subscribe(UpdateBetAmountFromInput)
                .AddTo(_disposables);

            SyncBetInputToCurrentBet();
        }

        /// <summary>
        /// ラウンドを開始し、両プレゼンターの状態と表示を初期化した上で、GameManager に配札させる。
        /// </summary>
        private void DealBothHands()
        {
            if (!EnsureGameManager())
            {
                return;
            }

            _playerPresenter?.ResetRoundState(refreshImmediately: false);
            _enemyPresenter?.ResetRoundState(refreshImmediately: false);
            _battleResolvedThisRound = false;                                   // 新しいラウンドなので再びバトル可能に。

            _gameManager.DealInitialHands();

            _playerPresenter?.ResetRevealState(refreshImmediately: false);
            _enemyPresenter?.ResetRevealState(refreshImmediately: false);

            _playerPresenter?.RefreshView();
            _enemyPresenter?.RefreshView();
            _resultLabel?.SetText("-");
        }

        /// <summary>
        /// 両手札が揃っているか、GameManager 参照が有効かをチェックする。
        /// </summary>
        private bool CanBattle()
        {
            if (!EnsureGameManager())
            {
                return false;
            }

            var playerHand = _gameManager.GetHand(PokerGameManager.HandOwner.Player);
            var enemyHand = _gameManager.GetHand(PokerGameManager.HandOwner.Enemy);

            if (playerHand == null || playerHand.Count == 0 || enemyHand == null || enemyHand.Count == 0)
            {
                Debug.LogWarning("Both hands must be dealt before resolving a battle.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 所持金を超えないよう賭け金を丸め、実際に所持金から差し引く。0 円ならそのまま続行可能。
        /// </summary>
        private bool TryConsumeBet(out int betAmount)
        {
            ClampBetToAffordableAmount();
            betAmount = _betAmount;

            if (betAmount == 0)
            {
                return true;
            }

            if (_playerData == null)
            {
                Debug.LogWarning("PlayerData reference is missing. Cannot place a bet.");
                return false;
            }

            if (_playerData.TrySpendMoney(betAmount))
            {
                return true;
            }

            Debug.LogWarning(_insufficientFundsMessage);
            return false;
        }

        /// <summary>
        /// バトル前に両手札をソートし、役評価と勝敗結果をまとめて取得する。
        /// </summary>
        private BattleSnapshot ResolveBattleSequence()
        {
            _gameManager.SortHand(PokerGameManager.HandOwner.Player);
            _gameManager.SortHand(PokerGameManager.HandOwner.Enemy);

            var result = _gameManager.ResolveBattle(out var playerRank, out var enemyRank);
            return new BattleSnapshot(result, playerRank, enemyRank);
        }

        /// <summary>
        /// 勝敗結果と倍率テーブルから配当額を算出し、PlayerData に反映する。
        /// 引き分け時は賭け金を全額返却。
        /// </summary>
        private void ApplyBattlePayout(BattleSnapshot snapshot, int betAmount)
        {
            if (_playerData == null || betAmount <= 0)
            {
                return;
            }

            switch (snapshot.Result)
            {
                case PokerGameManager.BattleResult.PlayerWin:
                    var multiplier = _payoutTable != null ? _payoutTable.GetMultiplier(snapshot.PlayerRank) : 1;
                    var reward = betAmount * Mathf.Max(1, multiplier);
                    _playerData.AddMoney(reward);
                    break;

                case PokerGameManager.BattleResult.Draw:
                    _playerData.AddMoney(betAmount);
                    break;
            }
        }

        /// <summary>
        /// 勝敗テキストを更新し、カードプレゼンターに再描画を要求する。
        /// </summary>
        private void UpdateBattleResult(BattleSnapshot snapshot)
        {
            var message = snapshot.Result switch
            {
                PokerGameManager.BattleResult.PlayerWin => _playerWinLabel,
                PokerGameManager.BattleResult.EnemyWin => _enemyWinLabel,
                _ => _drawLabel
            };

            _resultLabel?.SetText($"Player: {snapshot.PlayerRank} vs Enemy: {snapshot.EnemyRank}\n{message}");

            _playerPresenter?.RefreshView();
            _enemyPresenter?.RefreshView();
        }

        /// <summary>GameManager が未設定の場合は警告を出して処理を中断する。</summary>
        private bool EnsureGameManager()
        {
            if (_gameManager != null)
            {
                return true;
            }

            Debug.LogWarning("GameManager reference is missing.");
            return false;
        }

        /// <summary>
        /// 入力欄に打ち込まれた文字列を賭け金として解釈し、内部値へ反映する。
        /// </summary>
        private void UpdateBetAmountFromInput(string rawValue)
        {
            var parsed = ParseBetInput(rawValue);
            SetBetAmountInternal(parsed, false);
            SyncBetInputToCurrentBet();
        }

        /// <summary>
        /// 所持金が変動した際、賭け金が上限を超えていたら自動で丸め込む。
        /// </summary>
        private void OnPlayerMoneyChanged(int currentMoney)
        {
            if (_betAmount > currentMoney)
            {
                SetBetAmountInternal(currentMoney, true);
            }
            else
            {
                SyncBetInputToCurrentBet();
            }
        }

        /// <summary>所持金まで賭けられるよう、BetAmount を clamp するユーティリティ。</summary>
        private void ClampBetToAffordableAmount()
        {
            var max = GetMaxAffordableBet();
            if (_betAmount > max)
            {
                SetBetAmountInternal(max, true);
            }
        }

        /// <summary>
        /// ベット額を更新し、必要に応じて入力フィールドへ反映する。
        /// </summary>
        private void SetBetAmountInternal(int amount, bool updateInputField)
        {
            _betAmount = Mathf.Clamp(amount, 0, GetMaxAffordableBet());

            if (updateInputField)
            {
                SyncBetInputToCurrentBet();
            }
        }

        /// <summary>
        /// TMP_InputField へ現在の賭け金を反映。SetTextWithoutNotify でフィードバックループを防止。
        /// </summary>
        private void SyncBetInputToCurrentBet()
        {
            if (_betInputField == null)
            {
                return;
            }

            var text = _betAmount.ToString();
            if (_betInputField.text != text)
            {
                _betInputField.SetTextWithoutNotify(text);
            }
        }

        /// <summary>PlayerData が存在しない場合は制限なし（int.MaxValue）として扱う。</summary>
        private int GetMaxAffordableBet() =>
            _playerData != null ? Mathf.Max(0, _playerData.Money.Value) : int.MaxValue;

        /// <summary>入力文字列を数値に変換し、負値は 0 として扱う。</summary>
        private static int ParseBetInput(string rawValue) =>
            int.TryParse(rawValue, out var parsed) ? Mathf.Max(0, parsed) : 0;

        /// <summary>プレイヤーの引き直し完了イベントを購読し、強制バトルを仕掛けるトリガーにする。</summary>
        private void SubscribePlayerRedrawEvent()
        {
            if (_playerPresenter == null)
            {
                return;
            }

            _playerPresenter.RedrawPerformed -= HandlePlayerRedrawPerformed;     // 多重登録防止。
            _playerPresenter.RedrawPerformed += HandlePlayerRedrawPerformed;
        }

        private void UnsubscribePlayerRedrawEvent()
        {
            if (_playerPresenter == null)
            {
                return;
            }

            _playerPresenter.RedrawPerformed -= HandlePlayerRedrawPerformed;
        }

        /// <summary>
        /// プレイヤーが引き直しを行ったら即座にバトルを実行する（交換1回のみ&交換後強制バトルの仕様）。
        /// </summary>
        private void HandlePlayerRedrawPerformed(CardPresenter presenter)
        {
            if (presenter != _playerPresenter)
            {
                return;
            }

            ExecuteBattle();
        }

        private readonly struct BattleSnapshot
        {
            public BattleSnapshot(PokerGameManager.BattleResult result, PokerRank playerRank, PokerRank enemyRank)
            {
                Result = result;
                PlayerRank = playerRank;
                EnemyRank = enemyRank;
            }

            public PokerGameManager.BattleResult Result { get; }
            public PokerRank PlayerRank { get; }
            public PokerRank EnemyRank { get; }
        }
    }
}
