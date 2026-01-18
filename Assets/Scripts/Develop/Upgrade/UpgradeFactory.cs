using Develop.Save;
using System.Collections.Generic;
using Develop.Player;
namespace Develop.Upgrade
{
    /// <summary>
    /// アップグレードのファクトリークラス。
    /// </summary>
    public static class UpgradeFactory
    {
        /// <summary>
        ///     強化内容のリストを生成する。
        /// </summary>
        /// <param name="playerData"></param>
        /// <returns></returns>
        public static List<IUpgrade> Create()
        {
            // ここでアップグレードのインスタンスを生成し、リストに追加する。
            // 強化処理を実装したクラスをここで生成するようにしてください。
            List<IUpgrade> upgrades = new List<IUpgrade>
            {
                new ClickPowerUpgrade(),
                new AutoProductionUpgrade(),
                new LuckyCollectUpgrade(),
                new ChargeCollectUpgrade()
            };
            return upgrades;
        }
    }
}