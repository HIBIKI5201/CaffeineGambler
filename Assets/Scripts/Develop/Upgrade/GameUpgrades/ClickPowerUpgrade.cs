using Develop.Upgrade;
using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    /// クリックパワーアップグレードのクラス。
    /// </summary>
    public class ClickPowerUpgrade : UpgradeBase, IModifier
    {
        /// <summary>
        ///     現在の倍率を返す。
        /// </summary>
        public float Multiplier => _multiplier;

        public override string Name => "採取強化";

        public override int MaxLevel => 5;

        private float _multiplier;
        private const int _baseCost = 5;

        public float Modify(float value)
        {
            return value * _multiplier;
        }

        public override int GetCost()
        {
            return (int)(_baseCost * Mathf.Pow(2, Level - 1));
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            _multiplier = CalculateMultiplier(Level);
        }

        /// <summary>
        ///     レベルに応じた倍率を計算する。
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private float CalculateMultiplier(int level)
        {
            // n(level)=(1+0.5×level) × (1−0.05×(level−1))
            float linear = 0.5f * level;
            float decay = -0.05f * (level - 1);

            float n = (1 + linear) * (1 + decay);

            // 小数点第一位まで
            // 例　2.253 -> 2.2
            return Mathf.Floor(n * 10f) / 10f;
        }
    }
}