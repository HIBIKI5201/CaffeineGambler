using System;
using UniRx;
using Develop.Player;
using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    /// 
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private ShopItemView _shopItemViewPrefab;

        private UpgradeShopService _upgradeShopService;

        public void Init(PlayerData playerData, IUpgrade[] upgrades)
        {
            _upgradeShopService = new UpgradeShopService(playerData);

            foreach (var upgrade in upgrades)
            {
                CreateItem(upgrade);
            }
        }

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
