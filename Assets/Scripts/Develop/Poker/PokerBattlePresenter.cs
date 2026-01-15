using Develop.Player;
using UniRx;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Develop.Poker
{
    /// <summary>
    /// プレイヤーと敵の手札状態を同期させ、UI ボタン経由で配札・対決を制御するプレゼンター。
    /// </summary>
    public class PokerBattlePresenter : MonoBehaviour
    {
        [SerializeField] private PokerGameManager _gameManager;
        [SerializeField] private CardPresenter _playerPresenter;
        [SerializeField] private CardPresenter _enemyPresenter;
        [SerializeField] private TextMeshProUGUI _resultLabel;
        [SerializeField] private Button _dealButton;
        [SerializeField] private Button _battleButton;
        [SerializeField] private PokerRankPayoutTable _payoutTable;
        [SerializeField] private TMP_InputField _betInputField; // UI から賭け金を入力させるフィールド。
        [SerializeField][Min(0)] private int _betAmount = 10;   // 現在設定されている賭け金。所持金に応じて丸め込まれる。
        [SerializeField] private string _drawLabel = "Draw";
        [SerializeField] private string _playerWinLabel = "Player Wins!";
        [SerializeField] private string _enemyWinLabel = "Enemy Wins!";
        [SerializeField] private string _insufficientFundsMessage = "Not enough money to place a bet.";

        private readonly CompositeDisposable _disposables = new();
        private PlayerData _playerData;

        private void Awake()
        {
            SetupButtonSubscriptions();
            SetupBetInputBinding();
        }

        private void OnDestroy() => _disposables.Dispose();

        /// <summary>
        /// PlayerData を受け取り、所持金ストリームを監視（賭け金の上限連動）する。
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

        /// <summary>
        /// スクリプト外部（UI スライダーなど）から賭け金を直接設定したいときに利用。
        /// </summary>
        public void SetBetAmount(int amount) => SetBetAmountInternal(amount, true);

        /// <summary>ディールボタン押下時に呼ばれ、両者へ再配布し UI を初期化する。</summary>
        public void OnDealBothButton() => DealBothHands();

        /// <summary>対決ボタン押下時に呼ばれ、役比較＋結果表示をまとめて行う。</summary>
        public void OnBattleButton() => ExecuteBattle();

        /// <summary>
        /// 対決処理の本体。前提チェック → ベット消費 → 勝敗判定 → 配当 → 敵手札公開までを一括で実行。
        /// </summary>
        private void ExecuteBattle()
        {
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
        }

        /// <summary>
        /// ディール／バトルボタンを UniRx で購読し、重複登録を防ぎながら処理を紐づける。
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
        /// ベット入力欄に対し onEndEdit をフックし、ユーザー入力 → 内部値更新 → 反映 のループを作る。
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

        /// <summary>プレイヤー・敵の手札を配り直し、結果表示や公開状態をリセットする。</summary>
        private void DealBothHands()
        {
            if (!EnsureGameManager())
            {
                return;
            }

            _gameManager.DealInitialHands();

            _playerPresenter?.ResetRevealState(refreshImmediately: false);
            _enemyPresenter?.ResetRevealState(refreshImmediately: false);

            _playerPresenter?.RefreshView();
            _enemyPresenter?.RefreshView();
            _resultLabel?.SetText("-");
        }

        /// <summary>勝負可能か（GameManager 参照があるか、両手札が揃っているか）をチェックする。</summary>
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
        /// 現在の賭け金を所持金に合わせて丸めた後、実際に差し引く。0 円ならそのまま続行。
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
        /// 勝負直前に両者の手札をソートし、Evaluate をまとめて呼び出して勝敗判定に使う情報を作る。
        /// </summary>
        private BattleSnapshot ResolveBattleSequence()
        {
            _gameManager.SortHand(PokerGameManager.HandOwner.Player);
            _gameManager.SortHand(PokerGameManager.HandOwner.Enemy);

            var result = _gameManager.ResolveBattle(out var playerRank, out var enemyRank);
            return new BattleSnapshot(result, playerRank, enemyRank);
        }

        /// <summary>
        /// 勝敗結果と倍率テーブルを元に配当処理を行う。Draw 時はベット額を全額返却。
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
        /// 勝敗メッセージと役情報をラベルに表示し、手札ビューも最新状態にする。
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

        /// <summary>GameManager がセットされているかを確認し、未設定なら警告を出して早期リターン。</summary>
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
        /// 入力された文字列から賭け金を算出して反映。無効値は 0 とみなす。
        /// </summary>
        private void UpdateBetAmountFromInput(string rawValue)
        {
            var parsed = ParseBetInput(rawValue);
            SetBetAmountInternal(parsed, false);
            SyncBetInputToCurrentBet();
        }

        /// <summary>
        /// 所持金が増減した際に賭け金を調整し、入力欄と同期を保つ。
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

        /// <summary>
        /// 所持金を上限として賭け金を丸め込むユーティリティ。
        /// </summary>
        private void ClampBetToAffordableAmount()
        {
            var max = GetMaxAffordableBet();
            if (_betAmount > max)
            {
                SetBetAmountInternal(max, true);
            }
        }

        /// <summary>
        /// 内部賭け金を更新し、必要なら InputField へ反映する。
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
        /// InputField のテキストを現在の賭け金に合わせて更新。SetTextWithoutNotify でフィードバックループを防ぐ。
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

        /// <summary>所持金が不明な場合は int.MaxValue を返し、賭け金制限をかけない。</summary>
        private int GetMaxAffordableBet() =>
            _playerData != null ? Mathf.Max(0, _playerData.Money.Value) : int.MaxValue;

        /// <summary>入力された文字列を数値に変換し、負値を 0 扱いにする。</summary>
        private static int ParseBetInput(string rawValue) =>
            int.TryParse(rawValue, out var parsed) ? Mathf.Max(0, parsed) : 0;

        /// <summary>ResolveBattle の結果をまとめる小さな DTO。</summary>
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
