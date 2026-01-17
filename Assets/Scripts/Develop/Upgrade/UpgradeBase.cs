namespace Develop.Upgrade
{
    /// <summary>
    /// アップグレードの基底クラス。
    /// </summary>
    public abstract class UpgradeBase : IUpgrade
    {
        public abstract string Name { get; }

        public int Level { get; private set; }

        public abstract int MaxLevel { get; }

        public bool CanLevelUp => Level < MaxLevel;

        public virtual void ApplyUpgrade()
        {
            if (CanLevelUp)
            {
                Level++;
            }
        }

        public abstract int GetCost();
    }
}
