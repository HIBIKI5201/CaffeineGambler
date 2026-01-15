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
        [SerializeField] private string _drawLabel = "Draw";
        [SerializeField] private string _playerWinLabel = "Player Wins!";
        [SerializeField] private string _enemyWinLabel = "Enemy Wins!";

        private readonly CompositeDisposable _disposables = new();

        private void Awake() => SetupButtonSubscriptions();

        private void OnDestroy() => _disposables.Dispose();

        /// <summary>
        /// ディールボタン押下時に呼ばれ、両者へ再配布し UI を初期化する。
        /// </summary>
        public void OnDealBothButton() => DealBothHands();

        /// <summary>
        /// 対決ボタン押下時に呼ばれ、役比較＋結果表示をまとめて行う。
        /// </summary>
        public void OnBattleButton()
        {
            if (!CanBattle())
            {
                return;
            }

            var snapshot = ResolveBattleSequence();
            UpdateBattleResult(snapshot);
            _enemyPresenter?.SetRevealState(true);
        }

        private void SetupButtonSubscriptions()
        {
            _dealButton?
                .OnClickAsObservable()
                .Subscribe(_ => DealBothHands())
                .AddTo(_disposables);

            _battleButton?
                .OnClickAsObservable()
                .Where(_ => CanBattle())
                .Select(_ => ResolveBattleSequence())
                .Subscribe(snapshot =>
                {
                    UpdateBattleResult(snapshot);
                    _enemyPresenter?.SetRevealState(true);
                })
                .AddTo(_disposables);
        }

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

        private BattleSnapshot ResolveBattleSequence()
        {
            _gameManager.SortHand(PokerGameManager.HandOwner.Player);
            _gameManager.SortHand(PokerGameManager.HandOwner.Enemy);

            var result = _gameManager.ResolveBattle(out var playerRank, out var enemyRank);
            return new BattleSnapshot(result, playerRank, enemyRank);
        }

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

        private bool EnsureGameManager()
        {
            if (_gameManager != null)
            {
                return true;
            }

            Debug.LogWarning("GameManager reference is missing.");
            return false;
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
