using Develop.Player;
using System;
using UnityEngine;

namespace Develop.Save
{
    /// <summary>
    /// プレイヤーデータ操作用のクラス。
    /// </summary>
    public class PlayerDataHandler
    {
        public void LoadAndApply(PlayerData target)
        {
            // Keyは PlayerData の型名、実体は PlayerDataSave
            var loadedSave = SaveData.LoadJson(
                keyType: typeof(PlayerData),
                defaultValue: new PlayerDataSave { Money = 1000 }
            );

            if (loadedSave == null)
                return;

            target.Money.Value = loadedSave.Money;
        }
        public void Save(PlayerData playerData)
        {
            var save = new PlayerDataSave
            {
                Money = playerData.Money.Value
            };

            // KeyはPlayerDataの型名
            SaveData.SaveJson(typeof(PlayerData), save);
        }
    }
}
