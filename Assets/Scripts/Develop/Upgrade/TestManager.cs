using Develop.Player;
using Develop.Upgrade;
using UnityEngine;
using System.Collections.Generic;

public class TestManager : MonoBehaviour
{
    [SerializeField] private ShopManager _shopManager;
    [SerializeField] private AutoProductionRunner _autoProductionRunner;
    [SerializeField] private MoneyViwer _moneyViwer;

    private PlayerData _playerData;

    private void Start()
    {
        _playerData = new PlayerData(1000);

        _moneyViwer.Bind(_playerData);
        List<IUpgrade> upgrades = UpgradeFactory.Create();
        _shopManager.Init(_playerData,upgrades.ToArray());
        _autoProductionRunner.Init(_playerData, upgrades);
    }

    private void Update()
    {
        // 毎フレームお金増える
        //_playerData.AddMoney(1);
    }
}
