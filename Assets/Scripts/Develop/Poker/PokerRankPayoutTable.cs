using System;
using System.Collections.Generic;
using UnityEngine;

namespace Develop.Poker
{
    [CreateAssetMenu(menuName = "Develop/Poker/Payout Table", fileName = "PokerRankPayoutTable")]
    public class PokerRankPayoutTable : ScriptableObject
    {
        [SerializeField]
        private List<RankMultiplier> _entries = new();

        [Serializable]
        private struct RankMultiplier
        {
            public PokerRank Rank;
            [Min(1)] public int Multiplier;
        }

        public int GetMultiplier(PokerRank rank)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].Rank == rank)
                {
                    return Mathf.Max(1, _entries[i].Multiplier);
                }
            }

            return 1;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var ranks = (PokerRank[])Enum.GetValues(typeof(PokerRank));
            foreach (var rank in ranks)
            {
                if (_entries.Exists(entry => entry.Rank == rank))
                {
                    continue;
                }

                _entries.Add(new RankMultiplier
                {
                    Rank = rank,
                    Multiplier = 1
                });
            }
        }
#endif
    }
}