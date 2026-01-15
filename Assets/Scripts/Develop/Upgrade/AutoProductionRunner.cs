using Develop.Save;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Develop.Upgrade
{
    /// <summary>
    /// 自動生産を実行するクラス。
    /// </summary>
    public class AutoProductionRunner : MonoBehaviour
    {
        private PlayerData _playerData;
        private AutoProductionUpgrade _autoProductionUpgrade;
        private IDisposable _disposable;

        /// <summary>
        ///     初期化処理。
        /// </summary>
        /// <param name="playerData"></param>
        /// <param name="autoProductionUpgrade"></param>
        public void Init(PlayerData playerData, List<IUpgrade> upgrades)
        {
            _playerData = playerData;

            foreach (var upgrade in upgrades)
            {
                if (upgrade is AutoProductionUpgrade autoProdUpgrade)
                {
                    _autoProductionUpgrade = autoProdUpgrade;
                    break;
                }
            }

            _disposable?.Dispose();

            // 自動生産の実行
            _disposable = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ => Produce())
                .AddTo(this);
        }

        /// <summary>
        ///     生産処理。
        /// </summary>
        private void Produce()
        {
            if(_playerData == null || _autoProductionUpgrade == null)
                return;

            // 生産量を取得してプレイヤーの所持金に加算。
            int productionAmount = _autoProductionUpgrade.ProductionPerSecond;
            if (productionAmount > 0)
            {
                _playerData.AddMoney(productionAmount);
                Debug.Log($"自動生産: {productionAmount} 豆を生産しました。");
            }
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}
