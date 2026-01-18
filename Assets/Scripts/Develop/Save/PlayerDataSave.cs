using Develop.Player;
using Develop.Upgrade;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Develop.Save
{
    /// <summary>
    /// プレイヤーデータ保存用のクラス。
    /// </summary>
    [Serializable]
    public class PlayerDataSave
    {
        // JsonUtility.FromJson<T>() は 引数なしコンストラクタでインスタンスを作る
        public PlayerDataSave() { }

        public PlayerDataSave(PlayerData player, List<IUpgrade> upgrades)
        {
            Money = player.Money.Value;

            UpgradeLevel = new List<UpgradeLevelSave>();
            foreach (var upgrade in upgrades)
            {
                UpgradeLevel.Add(new UpgradeLevelSave
                {
                    TypeName = upgrade.GetType().Name,
                    Level = upgrade.Level,
                });
            }
        }

        /// <summary>
        /// 保存データを PlayerData と upgrades に反映する
        /// </summary>
        public void Convert(PlayerData player, List<IUpgrade> upgrades)
        {
            player.Money.Value = Money;

            if (upgrades == null || UpgradeLevel == null) return;

            Dictionary<string, int> map = new();
            foreach(var a in UpgradeLevel)
            {
                if(!string.IsNullOrEmpty(a.TypeName))
                    map[a.TypeName] = a.Level;
            }

            // 今のupgradesに反映
            foreach (var upgrade in upgrades)
            {
                string type = upgrade.GetType().Name;

                if (map.TryGetValue(type, out int level))
                {
                    // SetLevelできる型（UpgradeBase）のみ復元
                    if (upgrade is UpgradeBase baseUp)
                    {
                        baseUp.SetLevel(level);
                    }
                }
            }
        }

        public int Money;

        public List<UpgradeLevelSave> UpgradeLevel;
    }

    [Serializable]
    public class UpgradeLevelSave
    {
        public string TypeName;
        public int Level;
    }
}