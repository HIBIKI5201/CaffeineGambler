using UnityEngine;
namespace Develop.Upgrade.Festival.Test
{
   
        /// <summary>
        /// 時間制限付きイベントのドメインモデル。
        /// </summary>
        internal class TimedEvent
        {
            public int TotalCoffeeBeans { get; private set; }

            public TimedEvent(int threshold, int multiplier)
            {
            }

            /// <summary>
            /// イベントを開始する。
            /// </summary>
            public void StartEvent()
            {
            }

            /// <summary>
            /// 採取が行われたことをイベントに通知する。
            /// </summary>
            public void OnHarvest()
            {
            }

            /// <summary>
            /// イベント中に取得したコーヒー豆を加算する。
            /// </summary>
            public void AddCoffeeBeans(int amount)
            {
            }

            /// <summary>
            /// イベントを終了する。
            /// </summary>
            public void EndEvent()
            {
            }
        
    }

}

