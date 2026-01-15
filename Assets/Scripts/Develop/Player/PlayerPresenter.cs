using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Develop.Player
{
    public class PlayerPresenter : MonoBehaviour, IPointerClickHandler
    {
        private CompositeDisposable _disposables;
        private PlayerData _playerData;
        [SerializeField]
        private PlayerViewer _playerViewer;

        public void Init(PlayerData playerData)
        {
            _disposables = new CompositeDisposable();
            _playerData = playerData;

            _playerData.Money
                .Subscribe(money => _playerViewer.SetCount(money))
                .AddTo(_disposables);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _playerData?.AddMoney(1);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}
