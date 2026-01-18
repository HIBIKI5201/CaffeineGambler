using Develop.Player;
using UnityEngine;

public class PlayerInitilize : MonoBehaviour
{
    [SerializeField] private PlayerPresenter _playerPresenter;
    [SerializeField] private PlayerViewer _playerViewer;
    [SerializeField] private int _initialMoney = 1000;

    private PlayerData _playerData;

    private void Awake()
    {
        _playerData = new PlayerData(_initialMoney);
        //_playerPresenter.Init(_playerData);
    }

    private void OnDestroy()
    {
        _playerData?.OnDestroy();
    }
}
