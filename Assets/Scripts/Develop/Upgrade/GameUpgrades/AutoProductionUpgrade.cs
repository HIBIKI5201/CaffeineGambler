namespace Develop.Upgrade
{
    /// <summary>
    ///     自動生産機能の強化クラス。
    /// </summary>
    public class AutoProductionUpgrade : UpgradeBase
    {
        public override string Name => "自動生産";

        public override int MaxLevel => 50;

        public int ProductionPerSecond => CalculateProductionPerSecond(_level);

        private const int _theresholdLevel = 5;
        private const int _linerAdd = 2;
        private int _level;

        public override int GetCost()
        {
            // TODO: コスト計算式を決める。
            return 20 + _level * 15;
        }

        /// <summary>
        ///  レベルに応じた1秒あたりの生産量を計算する。
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private int CalculateProductionPerSecond(int level)
        {
            if (level <= 0) return 0;

            // しきい値レベルまでは指数関数的に増加
            if (level < _theresholdLevel)
            {
                return 1 << (level - 1);
            }

            // しきい値レベル以降は線形に増加
            int baseProduction = 1 << (_theresholdLevel - 2);
            int additionalProduction = level - (_theresholdLevel - 1);
            return baseProduction + additionalProduction * _linerAdd;
        }
    }
}
