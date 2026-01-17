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
        public void LoadAndApply(PlayerData player)
        {
            // Keyは PlayerData の型名、実体は PlayerDataSave
            var loadedSave = SaveData.LoadJson(
                keyType: typeof(PlayerData),
                defaultValue: default(PlayerDataSave)
            );

            if (loadedSave == null)
                return;

            loadedSave.Convert(player);
        }
        public void Save(PlayerData playerData)
        {
            var save = new PlayerDataSave(playerData);

            // KeyはPlayerDataの型名
            SaveData.SaveJson(typeof(PlayerData), save);
        }
    }
}
