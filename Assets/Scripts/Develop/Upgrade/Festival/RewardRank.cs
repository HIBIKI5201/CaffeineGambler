using System.Collections.Generic;
using UnityEngine;

namespace Develop.Upgrade.Festival
{
    /// <summary>
    /// 強化レベルに応じたフェスティバルの報酬設定（ScriptableObject）。
    /// </summary>
    [CreateAssetMenu(fileName = "RewardRank", menuName = "Develop/Upgrade/Festival/RewardRank")]
    public class RewardRankSO : ScriptableObject
    {
        public List<RewardRank> list = new List<RewardRank>();
    }


}
