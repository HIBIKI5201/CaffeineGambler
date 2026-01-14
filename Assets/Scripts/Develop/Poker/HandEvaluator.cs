using System.Linq;

/// <summary>
/// 手札の役を判定するヘルパークラス。
/// </summary>
public static class HandEvaluator
{
    /// <summary>
    /// 指定された手札の役を評価する。
    /// </summary>
    /// <param name="hand">評価対象の手札。</param>
    /// <returns>判定された役。</returns>
    public static PokerRank Evaluate(Hand hand)
    {
        var cards = hand.Cards.OrderBy(c => c.Rank).ToList();
        var rankGroups = cards.GroupBy(c => c.Rank).Select(g => g.Count()).OrderByDescending(c => c).ToList();
        var isFlush = cards.All(c => c.Suit == cards[0].Suit);
        var isStraight = cards.Select(c => c.Rank).Distinct().Count() == 5 &&
                         cards[^1].Rank - cards[0].Rank == 4;

        if (isStraight && isFlush) return PokerRank.StraightFlush;
        if (rankGroups[0] == 4) return PokerRank.FourOfAKind;
        if (rankGroups[0] == 3 && rankGroups[1] == 2) return PokerRank.FullHouse;
        if (isFlush) return PokerRank.Flush;
        if (isStraight) return PokerRank.Straight;
        if (rankGroups[0] == 3) return PokerRank.ThreeOfAKind;
        if (rankGroups[0] == 2 && rankGroups[1] == 2) return PokerRank.TwoPair;
        if (rankGroups[0] == 2) return PokerRank.OnePair;
        return PokerRank.HighCard;
    }
}

/// <summary>
/// 判定されるポーカーの役一覧。
/// </summary>
public enum PokerRank
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush
}
