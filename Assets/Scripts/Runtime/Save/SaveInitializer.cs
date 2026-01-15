using Develop.Player;
using Develop.Save;

namespace Runtime.Save
{
    /// <summary>
    /// セーブデータの初期化を行うクラス。
    /// </summary>
    public class SaveInitializer
    {
        private PlayerDataHandler _playerDataHandler;

        public void Init(PlayerData playerData)
        {
            _playerDataHandler = new PlayerDataHandler();
            _playerDataHandler.LoadandApply(playerData);
        }
    }
}
