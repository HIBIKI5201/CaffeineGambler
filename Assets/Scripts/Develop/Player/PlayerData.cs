
using Develop.Save;
using System;

namespace Develop.Player
{
    /// <summary>
    /// プレイヤーのデータを管理するクラス。    
    /// </summary>
    public class PlayerData
    {
        /// <summary>
        /// プレイヤーのデータを生成する。    
        /// </summary>
        /// <param name="money"> 所持金 </param>
        public PlayerData(int money)
        {
            if (money < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(money),
                    "初期所持金は0以上である必要があります。");
            }
            Money = money;
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

       
    }
}

