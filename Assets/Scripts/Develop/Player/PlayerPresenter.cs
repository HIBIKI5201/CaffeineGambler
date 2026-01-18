using Develop.Upgrade;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Develop.Player
{
    public class PlayerPresenter : MonoBehaviour, IPointerClickHandler
    {
        private CompositeDisposable _disposables;
        private PlayerData _playerData;
        private CollectionCalculation _collectionCalculationPresenter;
        private HarvestBus _harvestBus;
        [SerializeField]
        private PlayerViewer _playerViewer;
        [SerializeField]
        private float _baseAmount;

        public void Init(PlayerData playerData,List<IUpgrade> upgrades,HarvestBus harvestBus)
        {
            _harvestBus = harvestBus;
            _disposables = new CompositeDisposable();
            _playerData = playerData;

            _playerData.Money
                .Subscribe(money => _playerViewer.SetCount(money))
                .AddTo(_disposables);

            List<IModifier> modifers = upgrades.OfType<IModifier>().ToList();
            _collectionCalculationPresenter = new CollectionCalculation(modifers);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            float amount = _collectionCalculationPresenter.ApplyModifiers(_baseAmount);
            int result = Mathf.FloorToInt(amount);

            _playerData.AddMoney(result);
            _harvestBus?.OnHarvested.OnNext(result);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}
