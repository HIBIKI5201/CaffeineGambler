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
        private CompositeDisposable _disposables = new();

        public void Init(ShopItemView shopItemView)
        {
            _disposables.Clear();

            shopItemView.BuyButton.OnClickAsObservable()
                .Subscribe(_ => _onBuyClicked.OnNext(Unit.Default))
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
