using Develop.Player;
using UniRx;
using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    /// アップグレードショップの中立クラス。
    /// </summary>
    public class UpgradeShopService
    {
        public Subject<IUpgrade> OnUpgradePurchased = new();

        private PlayerData _playerData;

        public UpgradeShopService(PlayerData playerData)
        {
            _playerData = playerData;
        }

        /// <summary>
        ///     強化が購入可能かどうかを確認し、可能であれば購入を試みる。
        /// </summary>
        /// <param name="upgrade">強化名</param>
        /// <returns>買えるかどうか</returns>
        public bool TryPurchaseUpgrade(IUpgrade upgrade)
        {
            // 既に最大レベルに達している場合、購入不可にする。
            if (upgrade.Level >= upgrade.MaxLevel)
            {
                Debug.Log($"Upgrade {upgrade.Name} is already at max level.");
                return false;
            }

            // プレイヤーの所持金が足りない場合、購入不可にする。
            if (_playerData.TrySpendMoney(upgrade.Cost))
            {
                // 強化を適用する。
                upgrade.ApplyUpgrade();
                // 買われたことを通知する。
                OnUpgradePurchased.OnNext(upgrade);
                return true;
            }
            return false;
        }
    }
}
