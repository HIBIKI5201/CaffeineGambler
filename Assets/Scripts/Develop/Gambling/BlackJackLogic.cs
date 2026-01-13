using System.Collections.Generic;
using UnityEngine;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックのゲームロジックを担当するクラス。
    /// カードの配布、点数計算、勝敗判定を行う。
    /// 勝負層に位置する。
    /// </summary>
    public class BlackJackLogic
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public BlackJackLogic()
        {
            ResetGame();
        }

        // プレイヤーの手札
        public IReadOnlyList<int> PlayerHand => _playerHand;
        // ディーラーの手札
        public IReadOnlyList<int> DealerHand => _dealerHand;

        // ゲームが終了したかどうか
        public bool IsGameFinished { get; private set; }

        private List<int> _playerHand;
        private List<int> _dealerHand;
        
        // デッキ（簡易実装として無限デッキを想定し、ランダム生成する）
        // 1=A, 11=J, 12=Q, 13=K

        /// <summary>
        /// ゲームの状態をリセットする。
        /// </summary>
        public void ResetGame()
        {
            _playerHand = new List<int>();
            _dealerHand = new List<int>();
            IsGameFinished = false;
        }

        /// <summary>
        /// 最初の2枚ずつのカードを配る。
        /// </summary>
        public void DealInitialCards()
        {
            _playerHand.Add(DrawCard());
            _playerHand.Add(DrawCard());
            
            _dealerHand.Add(DrawCard());
            _dealerHand.Add(DrawCard());

            // ナチュラルブラックジャックの判定などはここで行っても良いが、
            // 進行制御（Dealerクラス）側で点数を確認して判定することを想定。
        }

        /// <summary>
        /// プレイヤーがカードを引く（ヒット）。
        /// </summary>
        /// <returns>バーストした場合は true</returns>
        public bool Hit()
        {
            if (IsGameFinished) return false;

            _playerHand.Add(DrawCard());
            
            if (CalculateScore(_playerHand) > 21)
            {
                // バースト
                return true;
            }
            return false;
        }

        /// <summary>
        /// ディーラーのターンを実行する（スタンド後の処理）。
        /// ディーラーは17以上になるまで引き続ける。
        /// </summary>
        public void DealerTurn()
        {
            if (IsGameFinished) return;

            while (CalculateScore(_dealerHand) < 17)
            {
                _dealerHand.Add(DrawCard());
            }
        }

        /// <summary>
        /// 最終的な勝敗を判定する。
        /// </summary>
        /// <returns>勝敗結果</returns>
        public GamblingResult DetermineResult()
        {
            IsGameFinished = true;

            int playerScore = CalculateScore(_playerHand);
            int dealerScore = CalculateScore(_dealerHand);

            // プレイヤーがバーストしている場合
            if (playerScore > 21)
            {
                return GamblingResult.Lose;
            }

            // ディーラーがバーストしている場合
            if (dealerScore > 21)
            {
                // プレイヤーはバーストしていないので勝ち
                // ブラックジャックかどうかの判定（初手2枚で21の場合）
                if (IsBlackJack(_playerHand)) return GamblingResult.BlackJack;
                return GamblingResult.Win;
            }

            // 点数比較
            if (playerScore > dealerScore)
            {
                if (IsBlackJack(_playerHand)) return GamblingResult.BlackJack;
                return GamblingResult.Win;
            }
            else if (playerScore < dealerScore)
            {
                return GamblingResult.Lose;
            }
            else
            {
                // 引き分け
                return GamblingResult.Draw;
            }
        }

        /// <summary>
        /// カードを1枚引く（1〜13のランダム）。
        /// </summary>
        private int DrawCard()
        {
            // 1(A) ～ 13(K)
            return UnityEngine.Random.Range(1, 14);
        }

        /// <summary>
        /// 手札の点数を計算する。
        /// Aは1または11として扱い、最適な方を採用する。
        /// J, Q, Kは10として扱う。
        /// </summary>
        public int CalculateScore(List<int> hand)
        {
            int score = 0;
            int aceCount = 0;

            foreach (var card in hand)
            {
                if (card == 1) // Ace
                {
                    aceCount++;
                    score += 11; // 一旦11として加算
                }
                else if (card >= 11) // J, Q, K
                {
                    score += 10;
                }
                else
                {
                    score += card;
                }
            }

            // 21を超えていて、Aがある場合はAを1として扱う（-10する）
            while (score > 21 && aceCount > 0)
            {
                score -= 10;
                aceCount--;
            }

            return score;
        }

        /// <summary>
        /// 手札がブラックジャック（2枚で21点）かどうか判定する。
        /// </summary>
        private bool IsBlackJack(List<int> hand)
        {
            return hand.Count == 2 && CalculateScore(hand) == 21;
        }
    }
}
