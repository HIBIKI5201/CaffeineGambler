using UnityEngine;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックの進行役（ディーラー）。
    /// ユーザー入力を受け付け、LogicとEconomyを仲介する。
    /// 入力層に位置する。
    /// </summary>
    public class BlackJackDealer : MonoBehaviour
    {
        // ゲーム中かどうか
        public bool IsGameActive { get; private set; }

        // 簡易実装のためDebug.Logで経過を表示するが、本来はPresenter/Viewへ通知する
        
        /// <summary>
        /// 依存関係を注入する初期化メソッド。
        /// </summary>
        public void Initialize(BlackJackLogic logic, GamblingEconomy economy)
        {
            _logic = logic;
            _economy = economy;
            IsGameActive = false;
        }

        /// <summary>
        /// ゲームを開始し、賭け金を支払う。
        /// </summary>
        /// <param name="betAmount">賭け金</param>
        public void StartGame(int betAmount)
        {
            if (IsGameActive)
            {
                Debug.LogWarning("ゲームは既に進行中です。");
                return;
            }

            if (!_economy.TryBet(betAmount))
            {
                Debug.Log("資金が不足しています。");
                return;
            }

            _currentBetAmount = betAmount;
            _logic.ResetGame();
            _logic.DealInitialCards();
            IsGameActive = true;

            Debug.Log($"ゲーム開始: 賭け金 {betAmount}");
            LogHandStatus();

            // ナチュラルブラックジャック即決判定（オプション）
            // ここではプレイヤーの判断を待たずに即勝敗が決まるルールも一般的だが
            // 簡略化のため、常にHit/Standフェーズへ移行する（あるいはStandで判定される）
        }

        /// <summary>
        /// プレイヤーがカードを引く。
        /// </summary>
        public void Hit()
        {
            if (!IsGameActive) return;

            bool isBurst = _logic.Hit();
            LogHandStatus();

            if (isBurst)
            {
                Debug.Log("バーストしました！");
                EndGame();
            }
        }

        /// <summary>
        /// プレイヤーがカードを引くのをやめ、勝負する。
        /// </summary>
        public void Stand()
        {
            if (!IsGameActive) return;

            Debug.Log("スタンドします。ディーラーのターン。");
            _logic.DealerTurn();
            EndGame();
        }

        // 依存関係
        private BlackJackLogic _logic;
        private GamblingEconomy _economy;

        // 現在の賭け金
        private int _currentBetAmount;

        /// <summary>
        /// ゲームを終了し、結果を確定・配当を行う。
        /// </summary>
        private void EndGame()
        {
            GamblingResult result = _logic.DetermineResult();
            _economy.Payout(_currentBetAmount, result);
            
            IsGameActive = false;
            _currentBetAmount = 0;

            Debug.Log($"ゲーム終了。結果: {result}");
            LogHandStatus();
        }

        /// <summary>
        /// 現在の手札状況をログ出力（デバッグ用）
        /// </summary>
        private void LogHandStatus()
        {
            string pHand = string.Join(",", _logic.PlayerHand);
            int pScore = _logic.CalculateScore(new System.Collections.Generic.List<int>(_logic.PlayerHand));
            
            string dHand = string.Join(",", _logic.DealerHand);
            int dScore = _logic.CalculateScore(new System.Collections.Generic.List<int>(_logic.DealerHand));

            Debug.Log($"Player: [{pHand}] ({pScore}) vs Dealer: [{dHand}] ({dScore})");
        }
    }
}
