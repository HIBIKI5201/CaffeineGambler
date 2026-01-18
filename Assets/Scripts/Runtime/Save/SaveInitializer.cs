using Develop.Player;
using Develop.Save;
using Develop.Upgrade;
using NUnit.Framework;
using System.Collections.Generic;

namespace Runtime.Save
{
    /// <summary>
    /// セーブデータの初期化を行うクラス。
    /// </summary>
    public class SaveInitializer
    {
        private PlayerDataHandler _playerDataHandler;

        public void Init(PlayerData playerData,List<IUpgrade> upgrades)
        {
            _playerDataHandler = new PlayerDataHandler();
            _playerDataHandler.LoadAndApply(playerData,upgrades);
        }
        public void Save(PlayerData playerData, List<IUpgrade> upgrades)
        {
            _playerDataHandler.Save(playerData,upgrades);
        }
    }
}
