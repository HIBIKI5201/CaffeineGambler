using Develop.Player;
using Develop.Upgrade;
using UnityEngine;
using System.Collections.Generic;

namespace Runtime.Upgrade
{
    /// <summary>
    /// Upgrade 系の初期化だけを担当するクラス
    /// </summary>
    public class UpgradeInitializer : MonoBehaviour
    {
        [SerializeField] private ShopManager _shopManager;
        [SerializeField] private AutoProductionRunner _autoProductionRunner;

        /// <summary>
        /// Upgrade 系の初期化をまとめて行う
        /// </summary>
        public void Init(PlayerData playerData, List<IUpgrade> upgrades)
        {
            // Shop に渡す（購入処理）
            _shopManager.Init(playerData, upgrades.ToArray());

            // 自動生産に渡す
            _autoProductionRunner.Init(playerData, upgrades);
        }
    }
}
