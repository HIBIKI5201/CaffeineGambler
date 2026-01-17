using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    ///     ラッキー採取アップグレードのクラス。
    /// </summary>
    public class LuckyCollectUpgrade : UpgradeBase, IModifier
    {
        public int ChancePercent => (int)_chancePercent;

        public float Multiplier => _multiplier;

        public override string Name => "ラッキー採取";

        public override int MaxLevel => 3;

        private int _baseCost = 50;
        private float _chancePercent = 3f;
        private float _multiplier = 1f;

        public override int GetCost()
        {
            return _baseCost * (1 << Level);
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            UpdateParameter(Level);
        }

        public float Modify(float value)
        {
            if (Level <= 0) return value;

            // 0〜100の乱数を生成
            float randomValue = Random.Range(0f, 100f);
            if (randomValue < _chancePercent)
            {
                Debug.Log($"ラッキー採取発動! 倍率: {_multiplier} (乱数: {randomValue}, 必要値: {_chancePercent})");
                return value * _multiplier;
            }

            return value;
        }

        /// <summary>
        /// レベルに応じたパラメータを更新する。
        /// </summary>
        /// <param name="level"></param>
        private void UpdateParameter(int level)
        {
            // m(L) = 1 + 2L
            _chancePercent = 1 + 2 * level;

            // n(level) = 1 + 0.5L + 0.1L(L − 1)
            float n = 1f + 0.5f * level + 0.1f * level * (level - 1);

            // 小数点第一位まで
            _multiplier = Mathf.Floor(n * 10f) / 10f;
        }
    }
}
