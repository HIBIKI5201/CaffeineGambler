using Develop.Player;
using Develop.Upgrade;
using System.Collections.Generic;

namespace Develop.Save
{
    /// <summary>
    /// プレイヤーデータ操作用のクラス。
    /// </summary>
    public class PlayerDataHandler
    {
        public void LoadAndApply(PlayerData player, List<IUpgrade> upgrages)
        {
            // Keyは PlayerData の型名、実体は PlayerDataSave
            var loadedSave = SaveData.LoadJson(
                keyType: typeof(PlayerData),
                defaultValue: default(PlayerDataSave)
            );

            if (loadedSave == null)
                return;

            loadedSave.Convert(player,upgrages);
        }
        public void Save(PlayerData playerData, List<IUpgrade> upgrages)
        {
            var save = new PlayerDataSave(playerData,upgrages);

            // KeyはPlayerDataの型名
            SaveData.SaveJson(typeof(PlayerData), save);
        }
    }
}
