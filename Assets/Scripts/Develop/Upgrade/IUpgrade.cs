using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    /// アップグレードのインターフェース。 
    /// </summary>
    public interface IUpgrade
    {
        /// <summary> アップグレードの名前。</summary>
        string Name { get; }
        /// <summary> アップグレードのレベル。</summary>
        int Level { get; }
        /// <summary> アップグレードの最大レベル。</summary>
        int MaxLevel { get; }
        /// <summary> アップグレードのコスト。</summary>
        int Cost { get; }
        /// <summary> アップグレードを適用する。 </summary>
        void ApplyUpgrade();
    }
}
