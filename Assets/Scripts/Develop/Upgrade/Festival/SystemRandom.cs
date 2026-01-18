using System;

namespace Develop.Upgrade.Festival
{
    /// <summary>
    /// 乱数生成クラス。
    /// </summary>
    public class SystemRandom : IRandom
    {

        /// <summary>
        /// 指定範囲の整数乱数を取得する。
        /// </summary>
        public int Range(int min, int max)
        {
            return _random.Next(min, max + 1);
        }
        private readonly Random _random = new Random();
    }
}