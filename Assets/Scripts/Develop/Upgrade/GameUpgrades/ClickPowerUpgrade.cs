using Develop.Upgrade;
using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    ///     クリックパワーアップグレードのクラス。
    /// </summary>
    public class ClickPowerUpgrade : UpgradeBase, IModifier
    {
        /// <summary>
        /// 現在の倍率を返す。
        /// </summary>
        public float Multiplier => _multiplier;

        public override string Name => "採取強化";

        public override int MaxLevel => 5;

        private float _multiplier = 1f;
        private const int _baseCost = 5;

        public float Modify(float value)
        {
            return value * _multiplier;
        }

        public override int GetCost()
        {
            return _baseCost * (1 << Level) ;
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            _multiplier = CalculateMultiplier(Level);
        }

        protected override void HandleLevelChanged()
        {
            _multiplier = CalculateMultiplier(Level);
        }

        /// <summary>
        /// レベルに応じた倍率を計算する。
        /// </summary>
        /// <param name="level">現在のレベル</param>
        /// <returns>倍率</returns>
        private float CalculateMultiplier(int level)
        {
            if (level <= 0) return 1f;

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