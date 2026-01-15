using UnityEngine;

namespace Develop.Upgrade
{
    public class AutoProductionUpgrade : IUpgrade
    {
        public string Name => "自動生産";

        public int Level => _level;

        public int MaxLevel => 50;
        public int ProductionPerSecond => CalculateProductionPerSecond(_level);

        private const int _theresholdLevel = 5;
        private const int _linerAdd = 2;
        private int _level;

        public void ApplyUpgrade()
        {
            if (_level >= MaxLevel) return;
            _level++;
        }

        public int GetCost()
        {
            // TODO: コスト計算式を決める
            return 20 + _level * 15;
        }

        private int CalculateProductionPerSecond(int level)
        {
            if (level <= 0) return 0;

            if(level < _theresholdLevel)
            {
                return 1 << (level - 1);
            }

            int baseProduction = 1 << (_theresholdLevel - 2);
            int additionalProduction = level - (_theresholdLevel - 1);
            return baseProduction + additionalProduction * _linerAdd;
        }
    }
}
