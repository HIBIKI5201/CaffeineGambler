using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    ///     チャージ採取アップグレードのクラス。
    /// </summary>
    public class ChargeCollectUpgrade : UpgradeBase
    {
        /// <summary> 1秒あたりの採取回数 </summary>
        public int CollctPerSecond => GetMultiplier(Level);

        public override string Name => "チャージ採取";

        public override int MaxLevel => 5;

        private const int _baseCost = 50;

        public override int GetCost()
        {
            return _baseCost * (Level + 1);
        }

        /// <summary>
        /// 倍率を取得する。
        /// レベルだけで決まるが、レベルのみで返すのが不自然なためメソッド化している。
        /// </summary>
        /// <param name="level">現在のレベル</param>
        /// <returns></returns>
        private int GetMultiplier(int level)
        {
            // 最大レベル以上のレベルが入れられても対応できるようにする。
            return Mathf.Clamp(level, 0, MaxLevel);
        }
    }
}