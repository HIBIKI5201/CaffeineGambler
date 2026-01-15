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
        public PlayerData PlayerData { get; private set; }

        public void Load()
        {
            // Keyは PlayerData の型名、実体は PlayerDataSave
            var loadedSave = SaveData.LoadJson(
                keyType: typeof(PlayerData),
                defaultValue: new PlayerDataSave { Money = 1000 }
            );

            // DTO -> PlayerData
            PlayerData = new PlayerData(loadedSave.Money);
        }
        public void Save()
        {
            // PlayerData -> DTO
            var save = new PlayerDataSave
            {
                Money = PlayerData.Money.Value
            };

            // KeyはPlayerDataの型名
            SaveData.SaveJson(typeof(PlayerData), save);
        }

        public void OnDestroy()
        {
            PlayerData?.OnDestroy();
        }
    }
}
