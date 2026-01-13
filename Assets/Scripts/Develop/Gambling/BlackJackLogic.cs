using System.Collections.Generic;
using UnityEngine;

namespace Develop.Gambling
{
    /// <summary>
    ///     ブラックジャックのゲームロジックを担当するクラス。
    ///     ScriptableObject から設定を読み込み、カードの配布や勝敗判定を行う。
    /// </summary>
    public class BlackJackLogic
    {
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="settings">ゲーム設定データ</param>
        public BlackJackLogic(BlackJackSettings settings)
        {
            // ゲームルールを決定する設定を保持するため
            _settings = settings;

            // 初期状態を整えるため
            ResetGame();
        }

        /// <summary>
        ///     プレイヤーの手札。
        /// </summary>
        public IReadOnlyList<int> PlayerHand => _playerHand;

        /// <summary>
        ///     ディーラーの手札。
        /// </summary>
        public IReadOnlyList<int> DealerHand => _dealerHand;

        /// <summary>
        ///     ゲームが終了しているかどうか。
        /// </summary>
        public bool IsGameFinished { get; private set; }

        /// <summary>
        ///     ゲームの状態をリセットする。
        /// </summary>
        public void ResetGame()
        {
            // 新しいゲームを開始するためにデータを初期化するため
            _playerHand = new List<int>();
            _dealerHand = new List<int>();
            IsGameFinished = false;
        }

        /// <summary>
        ///     最初の2枚ずつのカードを配る。
        /// </summary>
        public void DealInitialCards()
        {
            // ブラックジャックの基本ルールである初期配布を行うため
            for (int i = 0; i < _settings.InitialHandCount; i++)
            {
                _playerHand.Add(DrawCard());
                _dealerHand.Add(DrawCard());
            }
        }

        /// <summary>
        ///     プレイヤーがカードを引く（ヒット）。
        /// </summary>
        /// <returns>バーストした場合は true</returns>
        public bool Hit()
        {
            // 終了後にカードを引けないようにするため
            if (IsGameFinished) return false;

            // 新しいカードを手札に加えるため
            _playerHand.Add(DrawCard());

            // 合計値が目標を超えた（バーストした）か判定するため
            if (CalculateScore(_playerHand) > _settings.TargetScore)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     ディーラーのターンを実行する。
        /// </summary>
        public void DealerTurn()
        {
            // 終了後に処理を行わないようにするため
            if (IsGameFinished) return;

            // ディーラーのルールに従って、一定スコアに達するまでカードを引き続けるため
            while (CalculateScore(_dealerHand) < _settings.DealerStandThreshold)
            {
                _dealerHand.Add(DrawCard());
            }
        }

        /// <summary>
        ///     最終的な勝敗を判定する。
        /// </summary>
        /// <returns>勝敗結果</returns>
        public GamblingResult DetermineResult()
        {
            // 二重に判定処理が走らないようにするため
            IsGameFinished = true;

            int playerScore = CalculateScore(_playerHand);
            int dealerScore = CalculateScore(_dealerHand);

            // プレイヤーがバーストしていれば負けを確定させるため
            if (playerScore > _settings.TargetScore)
            {
                return GamblingResult.Lose;
            }

            // ディーラーがバーストしていればプレイヤーの勝ち（ブラックジャック考慮）にするため
            if (dealerScore > _settings.TargetScore)
            {
                if (IsBlackJack(_playerHand)) return GamblingResult.BlackJack;
                return GamblingResult.Win;
            }

            // スコアを比較して勝敗を決めるため
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
        ///     手札の点数を計算する。
        /// </summary>
        /// <param name="hand">計算対象の手札</param>
        /// <returns>計算された点数</returns>
        public int CalculateScore(List<int> hand)
        {
            int score = 0;
            int aceCount = 0;

            // 各カードの値を集計するため
            foreach (var card in hand)
            {
                if (card == _settings.CardAce)
                {
                    // Aは有利な11点として一旦計算するため
                    aceCount++;
                    score += (_settings.CardAce + _settings.AceBonusScore);
                }
                else if (card >= _settings.FaceCardThreshold)
                {
                    // 絵札は一律10点として計算するため
                    score += _settings.FaceCardScore;
                }
                else
                {
                    score += card;
                }
            }

            // バーストしそうな場合にAを1点として計算し直すことで救済するため
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
        ///     カードを1枚ランダムに引く。
        /// </summary>
        /// <returns>カードの数値</returns>
        private int DrawCard()
        {
            // 山札からランダムなカードをシミュレートするため
            return Random.Range(_settings.RandomRangeMin, _settings.RandomRangeMaxExclusive);
        }

        /// <summary>
        ///     手札がブラックジャックかどうか判定する。
        /// </summary>
        /// <param name="hand">対象の手札</param>
        /// <returns>ブラックジャックなら true</returns>
        private bool IsBlackJack(List<int> hand)
        {
            // 最初の2枚で合計21点というブラックジャックの特別条件を判定するため
            return hand.Count == _settings.InitialHandCount && CalculateScore(hand) == _settings.TargetScore;
        }
    }
}
