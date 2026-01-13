using Develop.Player;
using UnityEngine;

namespace Develop.Gambling.Develop
{
    /// <summary>
    ///     ブラックジャック関連のオブジェクト生成と依存解決を担当するロケータークラス。
    ///     インスタンス生成の責務をここに集約する。
    /// </summary>
    public static class BlackJackDealerLocator
    {
        private static BlackJackDealer _dealer;

        /// <summary>
        ///     設定とプレイヤーデータに基づいて、初期化済みのディーラーを取得または生成する。
        /// </summary>
        /// <param name="bjSettings">ブラックジャック設定</param>
        /// <param name="economySettings">経済設定</param>
        /// <param name="playerData">プレイヤーデータ</param>
        /// <returns>初期化済みのBlackJackDealer</returns>
        public static BlackJackDealer Resolve(
            BlackJackSettings bjSettings, 
            GamblingEconomySettings economySettings, 
            PlayerData playerData)
        {
            // シーン内でディーラーが重複しないようにするため
            if (_dealer != null)
            {
                return _dealer;
            }

            // DealerはMonoBehaviourであり、GameObjectにアタッチされる必要があるため
            GameObject dealerObject = new GameObject("BlackJackDealer");
            _dealer = dealerObject.AddComponent<BlackJackDealer>();

            // Dealerが動作するために必要なロジックと経済システムを構築するため
            var economy = new GamblingEconomy(playerData, economySettings);
            var logic = new BlackJackLogic(bjSettings);

            // 生成した依存関係をDealerに渡して機能するようにするため
            _dealer.Initialize(logic, economy);

            return _dealer;
        }

        /// <summary>
        ///     生成済みのディーラーを破棄する（リセット用）。
        /// </summary>
        public static void Clear()
        {
            // 参照を切ってGC対象にする、または再生成可能にするため
            _dealer = null;
        }
    }
}
