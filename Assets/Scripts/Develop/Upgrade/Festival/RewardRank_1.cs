using UnityEngine;
namespace Develop.Upgrade.Festival
{
    [System.Serializable]
    public class RewardRank 
    {
        /// <summary>
        /// 強化レベル。
        /// </summary>
        public int Level;

        /// <summary>
        /// 報酬獲得に必要なカウンターの閾値（m）。
        /// </summary>
        public int Threshold;

        /// <summary>
        /// 適用される報酬倍率（n）。
        /// </summary>
        public int Multiplier;
    }
}
