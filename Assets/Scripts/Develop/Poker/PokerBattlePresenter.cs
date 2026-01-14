using TMPro;
using UnityEngine;

namespace Develop.Poker
{
    /// <summary>
    /// プレイヤーと敵の手札状態を同期させ、UI ボタン経由で配札・対決を制御するプレゼンター。
    /// </summary>
    public class PokerBattlePresenter : MonoBehaviour
    {
        /// <summary>役判定や配札を担うゲームマネージャー参照。</summary>
        [SerializeField] private PokerGameManager _gameManager;

        /// <summary>プレイヤー手札の描画・操作を担当するプレゼンター。</summary>
        [SerializeField] private CardPresenter _playerPresenter;

        /// <summary>敵手札の描画を担当するプレゼンター。</summary>
        [SerializeField] private CardPresenter _enemyPresenter;

        /// <summary>勝敗メッセージを表示するラベル。</summary>
        [SerializeField] private TextMeshProUGUI _resultLabel;

        /// <summary>引き分け時に表示するテキスト。</summary>
        [SerializeField] private string _drawLabel = "Draw";

        /// <summary>プレイヤー勝利時に表示するテキスト。</summary>
        [SerializeField] private string _playerWinLabel = "Player Wins!";

        /// <summary>敵勝利時に表示するテキスト。</summary>
        [SerializeField] private string _enemyWinLabel = "Enemy Wins!";

        /// <summary>
        /// 両者に同時に手札を配り、ビューを初期化する（ディールボタン用）。
        /// </summary>
        public void OnDealBothButton()
        {
            if (_gameManager == null)
            {
                Debug.LogWarning("GameManager reference is missing.");
                return;
            }

            _gameManager.DealInitialHands();
            _playerPresenter?.RefreshView();
            _enemyPresenter?.RefreshView();
            _resultLabel?.SetText("-");
        }

        /// <summary>
        /// プレイヤーと敵の役を比較し、勝敗と役名をラベルへ表示する（対決ボタン用）。
        /// </summary>
        public void OnBattleButton()
        {
            if (_gameManager == null)
            {
                Debug.LogWarning("GameManager reference is missing.");
                return;
            }

            var playerHand = _gameManager.GetHand(PokerGameManager.HandOwner.Player);
            var enemyHand = _gameManager.GetHand(PokerGameManager.HandOwner.Enemy);

            if (playerHand == null || playerHand.Count == 0 || enemyHand == null || enemyHand.Count == 0)
            {
                Debug.LogWarning("Both hands must be dealt before resolving a battle.");
                return;
            }

            var result = _gameManager.ResolveBattle(out var playerRank, out var enemyRank);
            var message = result switch
            {
                PokerGameManager.BattleResult.PlayerWin => _playerWinLabel,
                PokerGameManager.BattleResult.EnemyWin => _enemyWinLabel,
                _ => _drawLabel
            };

            _resultLabel?.SetText($"Player: {playerRank} vs Enemy: {enemyRank}\n{message}");
            _playerPresenter?.RefreshView();
            _enemyPresenter?.RefreshView();
        }
    }
}
