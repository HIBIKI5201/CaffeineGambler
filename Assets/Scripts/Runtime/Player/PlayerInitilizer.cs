using Develop.Player;
using Develop.Upgrade;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitilizer : MonoBehaviour
{
    [SerializeField] private PlayerPresenter _playerPresenter;
    private PlayerData _playerData;
    private HarvestBus _harvestBus;
    public void Init(PlayerData playerdata,List<IUpgrade> upgrades,HarvestBus harvestBus)
    {
        _playerData = playerdata;
        _playerPresenter.Init(_playerData,upgrades,harvestBus);
    }

    private void OnDestroy()
    {
        _playerData?.OnDestroy();
    }
}
