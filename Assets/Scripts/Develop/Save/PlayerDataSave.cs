using Develop.Player;
using System;
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

        public PlayerDataSave(PlayerData player)
        {
            Money = player.Money.Value;
        }

        /// <summary>
        /// 保存データを PlayerData に反映する
        /// </summary>
        public void Convert(PlayerData player)
        {
            player.Money.Value = Money;
        }

        public int Money;
    }
}