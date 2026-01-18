using Develop.Player;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitilize : MonoBehaviour
{
    [SerializeField] private PlayerPresenter _playerPresenter;
    [SerializeField] private PlayerViewer _playerViewer;
    [SerializeField] private int _initialMoney = 1000;
    [SerializeField] private List<MonoBehaviour> _upgradeSources = new();

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
