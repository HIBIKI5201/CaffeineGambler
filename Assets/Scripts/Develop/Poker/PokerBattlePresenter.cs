using TMPro;
using UnityEngine;

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
        [SerializeField] private string _drawLabel = "Draw";
        [SerializeField] private string _playerWinLabel = "Player Wins!";
        [SerializeField] private string _enemyWinLabel = "Enemy Wins!";

        /// <summary>
        /// ディールボタン押下時に呼ばれ、両者へ再配布し UI を初期化する。
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

            // 結果ラベルは一旦リセットしておく。
            _resultLabel?.SetText("-");
        }

        /// <summary>
        /// 対決ボタン押下時に呼ばれ、役比較＋結果表示をまとめて行う。
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

            // 手札未配布のまま評価されないようガード。
            if (playerHand == null || playerHand.Count == 0 || enemyHand == null || enemyHand.Count == 0)
            {
                Debug.LogWarning("Both hands must be dealt before resolving a battle.");
                return;
            }

            // ResolveBattle で役比較→キッカー比較まで完了する。
            var result = _gameManager.ResolveBattle(out var playerRank, out var enemyRank);
            var message = result switch
            {
                PokerGameManager.BattleResult.PlayerWin => _playerWinLabel,
                PokerGameManager.BattleResult.EnemyWin => _enemyWinLabel,
                _ => _drawLabel
            };

            // 役名と勝敗メッセージをまとめて表示。
            _resultLabel?.SetText($"Player: {playerRank} vs Enemy: {enemyRank}\n{message}");

            // 対決後も UI が最新の手札状態を指すよう再描画しておく。
            _playerPresenter?.RefreshView();
            _enemyPresenter?.RefreshView();
        }
    }
}
