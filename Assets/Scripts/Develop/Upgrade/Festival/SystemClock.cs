using UnityEngine;
namespace Develop.Upgrade.Festival
{
    /// <summary>
    /// Unityの時間を提供する。
    /// </summary>
    public class SystemClock : IClock
    {
        /// <summary>
        /// ゲーム開始からの経過秒数を取得する。
        /// </summary>
        public int Now
        {
            get { return Mathf.FloorToInt(Time.time); }
        }
    }
}

