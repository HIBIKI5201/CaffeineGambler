using System.Collections.Generic;
using UnityEngine;

namespace Develop.Upgrade.Festival
{
    [CreateAssetMenu(fileName = "RewardRank", menuName = "Develop/Upgrade/Festival/RewardRank")]
    public class RewardRankSO : ScriptableObject
    {
        public List<RewardRank> list = new List<RewardRank>();

        private void OnEnable()
        {
            list.Add(new RewardRank());
        }
    }
}
