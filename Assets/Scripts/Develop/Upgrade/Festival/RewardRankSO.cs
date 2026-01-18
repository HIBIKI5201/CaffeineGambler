using System.Collections.Generic;
using UnityEngine;

namespace Develop.Upgrade.Festival
{
    [CreateAssetMenu(fileName = "RewardRank", menuName = "Develop/Upgrade/Festival/RewardRank")]
    public class RewardRankSO : ScriptableObject
    {
        public List<RewardRank> list = new List<RewardRank>();

        private void Reset()
        {
            if (list == null) list = new List<RewardRank>();
            if (list.Count == 0)
            {
                list.Add(new RewardRank()); // デフォルトの1件を追加
            }
        }
    }
}
