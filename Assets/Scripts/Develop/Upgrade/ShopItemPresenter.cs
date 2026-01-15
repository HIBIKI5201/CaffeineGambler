using System;
using UniRx;
using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    /// ショップアイテムのプレゼンター。
    /// </summary>
    public class ShopItemPresenter : MonoBehaviour
    {
        public IObservable<Unit> OnBuyClicked => _onBuyClicked;

        private Subject<Unit> _onBuyClicked = new();

        /// <summary>
        ///     入力の初期化。
        /// </summary>
        /// <param name="shopItemView"></param>
        public void Init(ShopItemView shopItemView)
        {
            shopItemView.BuyButton.OnClickAsObservable()
                .Subscribe(_ => _onBuyClicked.OnNext(Unit.Default))
                .AddTo(this);
        }
    }
}
