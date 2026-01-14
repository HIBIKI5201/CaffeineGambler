using System.Collections.Generic;
using System.Linq;

namespace Develop.Poker
{
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
            var cards = hand.Cards;
            if (cards.Count == 0)
            {
                return PokerRank.None;
            }

            var jokerCount = cards.Count(c => c.IsJoker);
            var nonJokerCards = cards.Where(c => !c.IsJoker).OrderBy(c => c.Rank).ToList();

            var rankGroups = nonJokerCards
                .GroupBy(c => c.Rank)
                .Select(g => g.Count())
                .OrderByDescending(c => c)
                .ToList();

            if (rankGroups.Count == 0)
            {
                rankGroups.Add(0);
            }

            rankGroups[0] += jokerCount;

            if (rankGroups.Count == 1)
            {
                rankGroups.Add(0);
            }

            var isFlush = IsFlush(nonJokerCards);
            var isStraight = IsStraightWithJokers(nonJokerCards.Select(c => c.Rank).ToList(), jokerCount);

            if (isStraight && isFlush) return PokerRank.StraightFlush;
            if (rankGroups[0] >= 4) return PokerRank.FourOfAKind;
            if (rankGroups[0] >= 3 && rankGroups[1] >= 2) return PokerRank.FullHouse;
            if (isFlush) return PokerRank.Flush;
            if (isStraight) return PokerRank.Straight;
            if (rankGroups[0] >= 3) return PokerRank.ThreeOfAKind;
            if (rankGroups[0] >= 2 && rankGroups[1] >= 2) return PokerRank.TwoPair;
            if (rankGroups[0] >= 2) return PokerRank.OnePair;

            return PokerRank.None;
        }

        private static bool IsFlush(IReadOnlyList<Card> cards)
        {
            if (cards.Count == 0)
            {
                return true;
            }

            var suit = cards[0].Suit;
            return cards.All(card => card.Suit == suit);
        }

        private static bool IsStraightWithJokers(IReadOnlyList<int> ranks, int jokerCount)
        {
            var distinctRanks = ranks.Distinct().OrderBy(r => r).ToList();
            if (distinctRanks.Count + jokerCount < 5)
            {
                return false;
            }

            var rankSet = new HashSet<int>(distinctRanks);
            if (rankSet.Contains(14))
            {
                rankSet.Add(1); // A を 1 として扱い、A2345 を許容する
            }

            for (var start = 1; start <= 10; start++)
            {
                var neededJokers = 0;
                for (var offset = 0; offset < 5; offset++)
                {
                    var value = start + offset;
                    if (!rankSet.Contains(value))
                    {
                        neededJokers++;
                        if (neededJokers > jokerCount)
                        {
                            break;
                        }
                    }

                    if (offset == 4)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    /// 判定されるポーカーの役一覧。
    /// </summary>
    public enum PokerRank
    {
        None,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush
    }
}
