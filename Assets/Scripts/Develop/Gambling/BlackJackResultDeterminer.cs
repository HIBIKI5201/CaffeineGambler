namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックの勝敗結果を判定する純粋なロジッククラス。
    /// スコア計算機を使用して、プレイヤーとディーラーの最終的な勝敗を導き出す。
    /// </summary>
    public class BlackJackResultDeterminer
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="scoreCalculator">点数計算に使用する計算機</param>
        public BlackJackResultDeterminer(BlackJackScoreCalculator scoreCalculator)
        {
            // スコアを基に判定を行うため、計算機を保持
            _scoreCalculator = scoreCalculator;
        }

        /// <summary>
        /// プレイヤーとディーラーの手札を比較し、最終的なゲーム結果を判定する。
        /// </summary>
        /// <param name="playerHand">プレイヤーの手札</param>
        /// <param name="dealerHand">ディーラーの手札</param>
        /// <returns>ギャンブルの結果（勝利、敗北、引き分け、ブラックジャック）</returns>
        public GamblingResult DetermineResult(BlackJackHand playerHand, BlackJackHand dealerHand)
        {
            int playerScore = _scoreCalculator.CalculateScore(playerHand.Cards);
            int dealerScore = _scoreCalculator.CalculateScore(dealerHand.Cards);

            // プレイヤーのバースト判定。バーストは即座に敗北
            if (_scoreCalculator.IsBust(playerHand.Cards))
            {
                return GamblingResult.Lose;
            }

            // ディーラーのバースト判定。プレイヤーがバーストしていなければ勝利
            if (_scoreCalculator.IsBust(dealerHand.Cards))
            {
                // バースト勝ちの場合でも、プレイヤーがブラックジャックならその配当を優先
                if (_scoreCalculator.IsBlackJack(playerHand.Cards)) return GamblingResult.BlackJack;
                return GamblingResult.Win;
            }

            // スコア比較
            if (playerScore > dealerScore)
            {
                // ブラックジャックによる勝利判定
                if (_scoreCalculator.IsBlackJack(playerHand.Cards)) return GamblingResult.BlackJack;
                return GamblingResult.Win;
            }
            else if (playerScore < dealerScore)
            {
                // ディーラーのスコアが高い場合は敗北
                return GamblingResult.Lose;
            }
            else
            {
                // スコアが同じ場合は引き分け（プッシュ）
                return GamblingResult.Draw;
            }
        }

        private readonly BlackJackScoreCalculator _scoreCalculator;
    }
}
