using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Develop.Poker
{
    /// <summary>
    /// 山札の生成と手札の配布・交換、役判定、勝敗判定までを一括で面倒を見るゲームマネージャー。
    /// </summary>
    public class PokerGameManager : MonoBehaviour
    {
        public IReadOnlyList<Card> CurrentHand => PlayerHand;
        public IReadOnlyList<Card> PlayerHand => _playerHand.Cards;
        public IReadOnlyList<Card> EnemyHand => _enemyHand.Cards;
        public IObservable<HandOwner> HandUpdated => _handUpdated;

        public enum HandOwner
        {
            Player,
            Enemy
        }

        public enum BattleResult
        {
            PlayerWin,
            EnemyWin,
            Draw
        }

        public IReadOnlyList<Card> GetHand(HandOwner owner) =>
            owner == HandOwner.Player ? _playerHand.Cards : _enemyHand.Cards;

        public PokerRank EvaluateCurrentHand() => EvaluateHand(HandOwner.Player);

        public PokerRank EvaluateHand(HandOwner owner) =>
            HandEvaluator.Evaluate(owner == HandOwner.Player ? _playerHand : _enemyHand);

        public void DealInitialHand() => DealInitialHand(HandOwner.Player);

        /// <summary>
        /// 指定プレイヤーの手札をすべて捨て、山札から指定枚数引き直す。
        /// </summary>
        public void DealInitialHand(HandOwner owner) =>
            MutateHand(owner, _handSize, RefillHand);

        /// <summary>
        /// プレイヤー・敵の順でまとめて初期手札を配る。
        /// </summary>
        public void DealInitialHands()
        {
            EnsureDeckHasEnoughCards(_handSize * 2);
            RefillHand(_playerHand);
            NotifyHandUpdated(HandOwner.Player);

            RefillHand(_enemyHand);
            NotifyHandUpdated(HandOwner.Enemy);
        }

        public void ReplaceCardAt(int index) => ReplaceCardAt(HandOwner.Player, index);

        /// <summary>
        /// 指定インデックスのカード1枚だけを差し替える。
        /// </summary>
        public void ReplaceCardAt(HandOwner owner, int index)
        {
            var hand = GetMutableHand(owner);
            if (!IsValidCardIndex(hand, index))
            {
                Debug.LogWarning($"ReplaceCardAt({owner}): index {index} is out of range.");
                return;
            }

            MutateHand(owner, hand, 1, h => h.Cards[index] = _deck.Draw());
        }

        public void ReplaceCards(IEnumerable<int> indices) => ReplaceCards(HandOwner.Player, indices);

        /// <summary>
        /// ハンドオーナーとインデックス集合を指定し、一括で引き直す。
        /// </summary>
        public void ReplaceCards(HandOwner owner, IEnumerable<int> indices)
        {
            if (indices == null)
            {
                return;
            }

            var hand = GetMutableHand(owner);
            var targets = indices
                .Distinct()
                .Where(i => IsValidCardIndex(hand, i))
                .ToArray();

            if (targets.Length == 0)
            {
                return;
            }

            MutateHand(owner, hand, targets.Length, h =>
            {
                foreach (var index in targets)
                {
                    h.Cards[index] = _deck.Draw();
                }
            });
        }

        /// <summary>
        /// 指定した手札をランク降順／スート昇順に並べ替える。
        /// </summary>
        public void SortHand(HandOwner owner)
        {
            var hand = GetMutableHand(owner);
            if (hand?.Cards == null || hand.Cards.Count <= 1)
            {
                return;
            }

            MutateHand(owner, hand, 0, h =>
            {
                h.Cards.Sort((left, right) =>
                {
                    var rankComparison = right.Rank.CompareTo(left.Rank);
                    return rankComparison != 0 ? rankComparison : left.Suit.CompareTo(right.Suit);
                });
            });
        }

        /// <summary>
        /// 役 → キッカー順で比較し、プレイヤーと敵の勝敗を返す。
        /// </summary>
        public BattleResult ResolveBattle(out PokerRank playerRank, out PokerRank enemyRank)
        {
            var playerStrength = CalculateStrength(_playerHand);
            var enemyStrength = CalculateStrength(_enemyHand);

            playerRank = playerStrength.Rank;
            enemyRank = enemyStrength.Rank;

            if (playerRank > enemyRank) return BattleResult.PlayerWin;
            if (playerRank < enemyRank) return BattleResult.EnemyWin;

            var kickerCompare = CompareRankValues(playerStrength.RankValues, enemyStrength.RankValues);
            if (kickerCompare > 0) return BattleResult.PlayerWin;
            if (kickerCompare < 0) return BattleResult.EnemyWin;

            return BattleResult.Draw;
        }

        [SerializeField] private bool _includeJoker = false; // Poker では常に false が想定。
        [SerializeField] private int _handSize = 5;

        private Deck _deck;
        private Hand _playerHand;
        private Hand _enemyHand;
        private readonly Subject<HandOwner> _handUpdated = new();

        private void Awake()
        {
            _deck = new Deck(_includeJoker);
            _playerHand = new Hand();
            _enemyHand = new Hand();
        }

        private void OnDestroy()
        {
            _handUpdated.OnCompleted();
            _handUpdated.Dispose();
        }

        private Hand GetMutableHand(HandOwner owner) =>
            owner == HandOwner.Player ? _playerHand : _enemyHand;

        private void NotifyHandUpdated(HandOwner owner) => _handUpdated.OnNext(owner);

        private void DrawToHand(Hand hand, int count)
        {
            for (var i = 0; i < count; i++)
            {
                hand.Add(_deck.Draw());
            }
        }

        /// <summary>
        /// 山札残数が足りないときは自動で再構築（シャッフル）する。
        /// </summary>
        private void EnsureDeckHasEnoughCards(int requiredCards)
        {
            if (requiredCards <= 0 || _deck.Count >= requiredCards)
            {
                return;
            }

            _deck.Reset(_includeJoker);
            Debug.Log("[Poker] Deck was reset due to insufficient cards.");
        }

        private void RefillHand(Hand hand)
        {
            hand.Clear();
            DrawToHand(hand, _handSize);
        }

        private static bool IsValidCardIndex(Hand hand, int index) =>
            hand != null && index >= 0 && index < hand.Cards.Count;

        private void MutateHand(HandOwner owner, int requiredDeckCards, Action<Hand> mutation) =>
            MutateHand(owner, GetMutableHand(owner), requiredDeckCards, mutation);

        private void MutateHand(HandOwner owner, Hand hand, int requiredDeckCards, Action<Hand> mutation)
        {
            if (hand == null)
            {
                Debug.LogWarning($"[{owner}] Hand reference is missing.");
                return;
            }

            if (requiredDeckCards > 0)
            {
                EnsureDeckHasEnoughCards(requiredDeckCards);
            }

            mutation(hand);
            NotifyHandUpdated(owner);
        }

        // --- 以下、勝敗比較用の内部ヘルパー ---
        private HandStrength CalculateStrength(Hand hand)
        {
            var rank = HandEvaluator.Evaluate(hand);
            var rankValues = new List<int>();

            var orderedCards = hand.Cards
                .OrderByDescending(c => c.Rank)
                .ToList();

            var orderedRanks = orderedCards
                .Select(c => c.Rank)
                .ToList();

            var groupInfos = hand.Cards
                .GroupBy(c => c.Rank)
                .Select(g => new GroupInfo(g.Key, g.Count()))
                .ToList();

            var quads = groupInfos.Where(g => g.Count == 4).OrderByDescending(g => g.Rank).Select(g => g.Rank).ToList();
            var triples = groupInfos.Where(g => g.Count == 3).OrderByDescending(g => g.Rank).Select(g => g.Rank).ToList();
            var pairs = groupInfos.Where(g => g.Count == 2).OrderByDescending(g => g.Rank).Select(g => g.Rank).ToList();

            switch (rank)
            {
                case PokerRank.StraightFlush:
                case PokerRank.Straight:
                    rankValues.Add(GetStraightHighCard(orderedRanks));
                    break;
                case PokerRank.FourOfAKind:
                    rankValues.Add(quads[0]);
                    AddRemainingRanks(rankValues, orderedRanks, quads[0]);
                    break;
                case PokerRank.FullHouse:
                    rankValues.Add(triples[0]);
                    rankValues.Add(pairs[0]);
                    break;
                case PokerRank.Flush:
                    rankValues.AddRange(orderedRanks);
                    break;
                case PokerRank.ThreeOfAKind:
                    rankValues.Add(triples[0]);
                    AddRemainingRanks(rankValues, orderedRanks, triples[0]);
                    break;
                case PokerRank.TwoPair:
                    rankValues.Add(pairs[0]);
                    rankValues.Add(pairs[1]);
                    AddRemainingRanks(rankValues, orderedRanks, pairs[0], pairs[1]);
                    break;
                case PokerRank.OnePair:
                    rankValues.Add(pairs[0]);
                    AddRemainingRanks(rankValues, orderedRanks, pairs[0]);
                    break;
                default:
                    rankValues.AddRange(orderedRanks);
                    break;
            }

            return new HandStrength(rank, rankValues);
        }

        private static void AddRemainingRanks(List<int> destination, IReadOnlyList<int> orderedRanks, params int[] excludedRanks)
        {
            for (var i = 0; i < orderedRanks.Count; i++)
            {
                var rank = orderedRanks[i];
                var skip = false;

                for (var j = 0; j < excludedRanks.Length; j++)
                {
                    if (rank == excludedRanks[j])
                    {
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                {
                    destination.Add(rank);
                }
            }
        }

        private static int CompareRankValues(IReadOnlyList<int> left, IReadOnlyList<int> right)
        {
            var length = Math.Min(left.Count, right.Count);
            for (var i = 0; i < length; i++)
            {
                if (left[i] == right[i])
                {
                    continue;
                }

                return left[i] > right[i] ? 1 : -1;
            }

            if (left.Count == right.Count)
            {
                return 0;
            }

            return left.Count > right.Count ? 1 : -1;
        }

        private static int GetStraightHighCard(IReadOnlyList<int> orderedRanks)
        {
            if (orderedRanks.Count < 5)
            {
                return orderedRanks.Count > 0 ? orderedRanks[0] : 0;
            }

            // 5-high ストレート（A2345）だけ特例で 5 をハイカードとして扱う。
            if (orderedRanks[0] == 14 &&
                orderedRanks[1] == 5 &&
                orderedRanks[2] == 4 &&
                orderedRanks[3] == 3 &&
                orderedRanks[4] == 2)
            {
                return 5;
            }

            return orderedRanks[0];
        }

        private readonly struct HandStrength
        {
            public HandStrength(PokerRank rank, List<int> rankValues)
            {
                Rank = rank;
                RankValues = rankValues;
            }

            public PokerRank Rank { get; }
            public List<int> RankValues { get; }
        }

        private readonly struct GroupInfo
        {
            public GroupInfo(int rank, int count)
            {
                Rank = rank;
                Count = count;
            }

            public int Rank { get; }
            public int Count { get; }
        }
    }
}
