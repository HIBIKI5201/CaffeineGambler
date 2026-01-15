using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    /// テスト用のアップグレードクラス。
    /// </summary>
    public class TestUpgrade : IUpgrade
    {
        public string Name => "テスト";
        public int Level { get; private set; }
        public int MaxLevel => 10;
        public int GetCost()
        {
            return 10 + Level * 5;
        }
        public void ApplyUpgrade()
        {
            if (Level >= MaxLevel)
                return;
            Level++;
        }
    }
}
