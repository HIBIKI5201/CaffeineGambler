using Develop.Player;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPresenter : MonoBehaviour,IPointerClickHandler
{
    private CompositeDisposable _disposables;
    private PlayerData _playerData;
    private PlayerViwer _playerViwer;

    public void OnPointerClick(PointerEventData eventData)
    {
        _playerData.AddMoney(1);
    }

    private void Start()
    {
        _disposables = new CompositeDisposable();
        _playerData = new PlayerData(0);
        _playerViwer = GetComponent<PlayerViwer>();

        _playerData.Money
            .Subscribe(money => _playerViwer.SetCount(money))
            .AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
        _playerData?.OnDestroy();
    }
}
