using System.Collections.Generic;
using System.Linq;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックの点数計算ルールを定義する純粋な計算クラス。
    /// 状態を持たず、与えられた手札からスコアを算出することに特化する。
    /// </summary>
    public class BlackJackScoreCalculator
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="settings">点数計算に使用する設定</param>
        public BlackJackScoreCalculator(BlackJackSettings settings)
        {
            // 計算ルールの定数（目標点数、Aの扱い、絵札の点数）を保持するため
            _settings = settings;
        }

        /// <summary>
        /// 手札の合計点数を計算する。
        /// A（エース）はバーストしない限り11点として扱い、バーストする場合は1点として計算する。
        /// </summary>
        /// <param name="cards">計算対象のカードリスト</param>
        /// <returns>現在の合計点数</returns>
        public int CalculateScore(IEnumerable<Card> cards)
        {
            int score = 0;
            int aceCount = 0;

            foreach (var card in cards)
            {
                if (card.Rank == Rank.Ace)
                {
                    // Aは一旦設定されたボーナス点（通常11点）として計算
                    score += _settings.AceBonusScore;
                    aceCount++;
                }
                else if (card.Rank >= Rank.Ten)
                {
                    // 10, J, Q, Kは一律設定された点数（通常10点）
                    score += _settings.FaceCardScore;
                }
                else
                {
                    // それ以外はランクの数値をそのまま加算
                    score += (int)card.Rank;
                }
            }

            // 目標点数を超えている（バースト）場合、Aを1点として扱い直して調整
            while (score > _settings.TargetScore && aceCount > 0)
            {
                score -= (_settings.AceBonusScore - 1);
                aceCount--;
            }

            return score;
        }

        /// <summary>
        /// 手札がブラックジャック（初期2枚で目標点数に到達）かどうかを判定する。
        /// </summary>
        /// <param name="cards">判定対象のカードリスト</param>
        /// <returns>ブラックジャックなら true</returns>
        public bool IsBlackJack(IReadOnlyList<Card> cards)
        {
            // 枚数が初期配布数（通常2枚）かつスコアが目標点（通常21点）の場合のみブラックジャック
            return cards.Count == _settings.InitialHandCount && CalculateScore(cards) == _settings.TargetScore;
        }

        /// <summary>
        /// 手札がバーストしているかどうかを判定する。
        /// </summary>
        /// <param name="cards">判定対象のカードリスト</param>
        /// <returns>バーストなら true</returns>
        public bool IsBust(IEnumerable<Card> cards)
        {
            // スコアが目標点を超えているかどうかをチェック
            return CalculateScore(cards) > _settings.TargetScore;
        }

        private readonly BlackJackSettings _settings;
    }
}
