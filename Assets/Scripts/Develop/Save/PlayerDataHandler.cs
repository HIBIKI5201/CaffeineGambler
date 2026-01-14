using Develop.Player;
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
            int savedMoney = SaveData.LoadInt(PlayerData.MoneyKey, 1000);
            PlayerData = new PlayerData(savedMoney);
            Debug.Log($"Money loaded: {savedMoney}");
        }

        public void Save()
        {
            if (PlayerData != null)
            {
                SaveData.SaveInt(PlayerData.MoneyKey, PlayerData.Money.Value);
                Debug.Log($"Money saved: {PlayerData.Money.Value}");
            }
            else
            {
                Debug.LogWarning("PlayerData is null. Cannot save.");
            }
        }

        public void OnDestroy()
        {
            PlayerData?.OnDestroy();
        }
    }
}
