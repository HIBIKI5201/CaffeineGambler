
using Develop.Save;
using System.Diagnostics;

namespace Develop.Player
{
    /// <summary>
    /// プレイヤーのデータを管理するクラス。    
    /// </summary>
    public class PlayerData
    {
        /// <summary>
        /// 新規ゲーム開始時のプレイヤーデータを生成する。    
        /// </summary>
        public PlayerData(int initialMoney)
        {
            Money = initialMoney;
        }

        /// <summary>
        /// セーブデータからプレイヤーデータを復元する。    
        /// </summary>
        public PlayerData(PlayerStatusSaveData saveData)
        {
            Money = saveData.Money;
        }

   　　　// プレイヤーの所持金
        public int Money { get; private set; }

        public void AddMoney(int amount)
        {
            Money += amount;
        }

        /// <summary>
        /// 所持金を消費する。
        /// 消費できない場合は false を返す。
        /// </summary>
        /// <param name="amount"> 消費する金額。</param>
        /// <reterns> 消費できたかどうか </reterns>
        public bool TrySpendMoney(int amount)
        {
            if (Money >= amount)
            {
                Money -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// プレイヤーデータをセーブデータに変換する。
        /// </summary>
        /// <returns> セーブデータ </returns>
        public PlayerStatusSaveData ToSaveData()
        {
            return new PlayerStatusSaveData(Money);
        }

    }
}

