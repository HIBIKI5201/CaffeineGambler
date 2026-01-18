using Develop.Player;
using Develop.Upgrade;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitilizer : MonoBehaviour
{
    [SerializeField] private PlayerPresenter _playerPresenter;
    private PlayerData _playerData;
    public void Init(PlayerData playerdata,List<IUpgrade> upgrades)
    {
        _playerData = playerdata;
        _playerPresenter.Init(_playerData,upgrades);
    }

    private void OnDestroy()
    {
        _playerData?.OnDestroy();
    }
}
