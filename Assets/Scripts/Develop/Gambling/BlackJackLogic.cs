using System.Collections.Generic;
using UnityEngine;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックのゲームロジックを担当するクラス。
    /// ScriptableObject から設定を読み込み、カードの配布や勝敗判定を行う。
    /// </summary>
    public class BlackJackLogic
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="settings">ゲーム設定データ</param>
        public BlackJackLogic(BlackJackSettings settings)
        {
            // 設定を保持
            _settings = settings;
            ResetGame();
        }

        public IReadOnlyList<int> PlayerHand => _playerHand;
        public IReadOnlyList<int> DealerHand => _dealerHand;
        public bool IsGameFinished { get; private set; }

        /// <summary>
        /// ゲームの状態をリセットする。
        /// </summary>
        public void ResetGame()
        {
            // 手札の初期化
            _playerHand = new List<int>();
            _dealerHand = new List<int>();
            IsGameFinished = false;
        }

        /// <summary>
        /// 最初の2枚ずつのカードを配る。
        /// </summary>
        public void DealInitialCards()
        {
            // 初期枚数分カードを追加
            for (int i = 0; i < _settings.InitialHandCount; i++)
            {
                _playerHand.Add(DrawCard());
                _dealerHand.Add(DrawCard());
            }
        }

        /// <summary>
        /// プレイヤーがカードを引く（ヒット）。
        /// </summary>
        /// <returns>バーストした場合は true</returns>
        public bool Hit()
        {
            if (IsGameFinished) return false;

            // カードを追加してスコア確認
            _playerHand.Add(DrawCard());

            if (CalculateScore(_playerHand) > _settings.TargetScore)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ディーラーのターンを実行する。
        /// </summary>
        public void DealerTurn()
        {
            if (IsGameFinished) return;

            // 閾値を超えるまで引き続ける
            while (CalculateScore(_dealerHand) < _settings.DealerStandThreshold)
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

            // プレイヤーのバースト判定
            if (playerScore > _settings.TargetScore)
            {
                return GamblingResult.Lose;
            }

            // ディーラーのバースト判定
            if (dealerScore > _settings.TargetScore)
            {
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
                return GamblingResult.Draw;
            }
        }

        /// <summary>
        /// 手札の点数を計算する。
        /// </summary>
        /// <param name="hand">計算対象の手札</param>
        /// <returns>計算された点数</returns>
        public int CalculateScore(List<int> hand)
        {
            int score = 0;
            int aceCount = 0;

            foreach (var card in hand)
            {
                if (card == _settings.CardAce)
                {
                    // Aは一旦11点として計算
                    aceCount++;
                    score += (_settings.CardAce + _settings.AceBonusScore);
                }
                else if (card >= _settings.FaceCardThreshold)
                {
                    // 絵札は一律設定された点数（10点）
                    score += _settings.FaceCardScore;
                }
                else
                {
                    score += card;
                }
            }

            // 目標点数を超えている場合、Aを1点として扱い直す
            while (score > _settings.TargetScore && aceCount > 0)
            {
                score -= _settings.AceBonusScore;
                aceCount--;
            }

            return score;
        }

        private readonly BlackJackSettings _settings;
        private List<int> _playerHand;
        private List<int> _dealerHand;

        /// <summary>
        /// カードを1枚ランダムに引く。
        /// </summary>
        private int DrawCard()
        {
            return Random.Range(_settings.RandomRangeMin, _settings.RandomRangeMaxExclusive);
        }

        /// <summary>
        /// 手札がブラックジャックかどうか判定する。
        /// </summary>
        private bool IsBlackJack(List<int> hand)
        {
            // 枚数と点数が設定通りならブラックジャック
            return hand.Count == _settings.InitialHandCount && CalculateScore(hand) == _settings.TargetScore;
        }
    }
}