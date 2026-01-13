
using UnityEngine;
namespace Develop.Gambling.Develop
{
    /// <summary>
    /// ブラックジャックのディーラーを取得するロケータークラス。
    /// </summary>
    public static class BlackJackDealerLocator
    {
        private static BlackJackDealer _dealer;

        /// <summary>
        ///     BlackJackDealer を取得する。
        ///     既に存在する場合はそれを返し、存在しない場合は新しく生成して返す。
        /// </summary>
        /// <retruns> ディーラー </retruns>
        public static BlackJackDealer GetDealer()
        {
            if (_dealer != null)
            {
                return _dealer;
            }

            GameObject dealerObject = new GameObject("BlackJackDealer");
            _dealer = dealerObject.AddComponent<BlackJackDealer>();

            return _dealer;
        }
    }
}
