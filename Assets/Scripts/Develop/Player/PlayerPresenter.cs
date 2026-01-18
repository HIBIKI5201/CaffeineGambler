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
        [SerializeField]
        private PlayerViewer _playerViewer;
        [SerializeField]
        private float _baseAmount;

        public void Init(PlayerData playerData,List<IUpgrade> upgrades)
        {
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

            _playerData.AddMoney(Mathf.FloorToInt(amount));
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}
