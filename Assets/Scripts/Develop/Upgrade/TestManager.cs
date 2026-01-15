using Develop.Player;
using Develop.Upgrade;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [SerializeField] private ShopManager _shopManager;

    private PlayerData _playerData;

    private void Start()
    {
        _playerData = new PlayerData(1000);
        var upgrades = UpgradeFactory.Create(_playerData);
        _shopManager.Init(_playerData,upgrades.ToArray());
    }

    private void Update()
    {
        _playerData.AddMoney(1); // 毎フレームお金増える
    }
}
