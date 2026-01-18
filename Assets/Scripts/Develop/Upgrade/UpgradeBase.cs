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

        public void SetLevel(int level)
        {
            // clamp（0〜MaxLevel）
            if (level < 0) level = 0;
            if (level > MaxLevel) level = MaxLevel;

            this.Level = level; 

            HandleLevelChanged();
        }

        public virtual void ApplyUpgrade()
        {
            if (CanLevelUp)
            {
                Level++;
            }
        }

        public abstract int GetCost();

        /// <summary>
        /// レベル変更後に内部状態を同期させるためのメソッド
        /// </summary>
        protected virtual void HandleLevelChanged() { }
    }
}
