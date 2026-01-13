using Develop.Player;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPresenter : MonoBehaviour, IPointerClickHandler
{
    private CompositeDisposable _disposables;
    private PlayerData _playerData;
    private PlayerViwer _playerViwer;

    public void Init(PlayerData playerData, PlayerViwer playerViwer)
    {
        _disposables = new CompositeDisposable();
        _playerData = playerData;
        _playerViwer = playerViwer;

        _playerData.Money
            .Subscribe(money => _playerViwer.SetCount(money))
            .AddTo(_disposables);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _playerData?.AddMoney(1);
    }

    private void OnDestroy()
    {
        _disposables?.Dispose();
        _playerData?.OnDestroy();
    }
}
