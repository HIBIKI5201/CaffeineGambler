namespace Develop.Gambling
{
    /// <summary>
    ///     ギャンブルの結果を表す列挙型。
    ///     勝敗に応じた配当計算の分岐に使用する。
    /// </summary>
    public enum GamblingResult
    {
        /// <summary>
        ///     負け。
        /// </summary>
        Lose,

        /// <summary>
        ///     通常勝利。
        /// </summary>
        Win,

        /// <summary>
        ///     引き分け（プッシュ）。
        /// </summary>
        Draw,

        /// <summary>
        ///     ブラックジャックでの勝利。
        /// </summary>
        BlackJack
    }
}