using Develop.Player;
using Develop.Upgrade;
using Runtime.Gambling;
using Runtime.Save;
using Runtime.Upgrade;
using System.Collections.Generic;
using UnityEngine;
namespace Runtime
{
    /// <summary>
    /// ゲーム全体の管理を担当するマネージャークラス。
    /// ゲームの初期化、状態管理、主要なシステムの調整を行う。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GamblingInitilizer _gamblingInitializer;
        [SerializeField] private PokerInit _pokerInitializer;
        [SerializeField] private PlayerInitilizer _playerInitializer;
        [SerializeField] private UpgradeInitializer _upgradeInitializer;
        [SerializeField] private int _initialMoney = 1000;
        private PlayerData _playerData;
        private List<IUpgrade> _upgrades;
        private SaveInitializer _saveInitializer;
        private void Awake()
        {
            _playerData = new PlayerData(_initialMoney); // 初期所持金1000でプレイヤーデータを作成
            _upgrades = UpgradeFactory.Create();
            // ギャンブルシステムの初期化を実行
            _saveInitializer = new SaveInitializer();
            _saveInitializer.Init(_playerData,_upgrades);

            _playerInitializer.Init(_playerData, _upgrades);
            _upgradeInitializer.Init(_playerData, _upgrades);
            _gamblingInitializer.GamblingInit(_playerData);
            _pokerInitializer?.Init(_playerData);
        }
        private void OnApplicationQuit()
        {
           _saveInitializer.Save(_playerData,_upgrades);
        }
    }
}

