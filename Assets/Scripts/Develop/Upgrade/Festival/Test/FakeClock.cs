using UnityEngine;

namespace Develop.Upgrade.Festival
{
    public class FakeClock : IClock
    {
        public int Now { get; private set; }

        /// <summary>
        /// 時間を指定秒数進める。
        /// </summary>
        public void Advance(int seconds)
        {
            Now += seconds;
        }
    }
}