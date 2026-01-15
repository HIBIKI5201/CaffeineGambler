using System;
using UniRx;
using Develop.Save;
using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    ///     ショップの生成などを行うマネージャークラス。
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private ShopItemView _shopItemViewPrefab;

        private UpgradeShopService _upgradeShopService;

        /// <summary> 初期化処理をする。 </summary>
        /// <param name="playerData"></param>
        /// <param name="upgrades"></param>
        public void Init(PlayerData playerData, IUpgrade[] upgrades)
        {
            _upgradeShopService = new UpgradeShopService(playerData);

            foreach (var upgrade in upgrades)
            {
                CreateItem(upgrade);
            }
        }

        /// <summary> アイテムを生成する。</summary>
        /// <param name="upgrade"></param>
        private void CreateItem(IUpgrade upgrade)
        {
            ShopItemView shopItemView = Instantiate(_shopItemViewPrefab, _contentRoot);
            ShopItemPresenter presenter = shopItemView.GetComponent<ShopItemPresenter>();

            shopItemView.Set(upgrade);
            presenter.Init(shopItemView);

            presenter.OnBuyClicked
                .Subscribe(_ =>
                {
                    if (_upgradeShopService.TryPurchaseUpgrade(upgrade))
                    {
                        shopItemView.Set(upgrade);
                    }
                })
                .AddTo(presenter);
        }
    }
}
