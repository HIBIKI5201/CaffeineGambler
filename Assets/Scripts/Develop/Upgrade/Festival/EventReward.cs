using Develop.Player;
using Develop.Upgrade.Festival;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Domain
{
    /// <summary>
    /// イベント中の報酬ルールを管理するドメイン。
    /// </summary>
    public class EventReward
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public EventReward(TimedEvent timedEvent,PlayerData playerData)
        {
            _timedEvent = timedEvent;
            _playerData = playerData;
        }

        /// <summary>
        /// 獲得したコーヒー豆の総数。
        /// </summary>
        public int TotalCoffeeBeans { get; private set; }

        /// <summary>
        /// 現在のレベルに応じた報酬ランクを設定する。
        /// </summary>
        public void ConfigureRanks(int currentLevel, RewardRankSO so)
        {
            _ranks.Clear();

            // レベル2未満の場合はボーナス機能自体が無効
            if (currentLevel < 2)
            {
                return;
            }

            // 到達可能なすべてのレベルの条件を登録
            foreach (var setting in so.list)
            {
                if (setting.Level <= currentLevel)
                {
                    _ranks.Add(setting);
                }
            }
        }

        /// <summary>
        /// イベント開始時のリセット。
        /// </summary>
        public void OnEventStarted()
        {
            _counter = 0;
            _eventCoffeeBeans = 0;
        }

        /// <summary>
        /// 採取カウント。
        /// </summary>
        public void OnHarvest()
        {
            if (!_timedEvent.IsActive)
            {
                return;
            }

            _counter++;
        }

        /// <summary>
        /// コーヒー豆の加算。
        /// </summary>
        public void AddCoffeeBeans(int amount)
        {
            if (!_timedEvent.IsActive)
            {
                return;
            }

            _eventCoffeeBeans += amount;
            //TotalCoffeeBeans += amount;
        }

        /// <summary>
        /// イベント終了時の追加報酬精算。
        /// </summary>
        public void OnEventEnded()
        {
            if (!_timedEvent.IsActive)
            {
                // 達成している中で最大の閾値を持つランクを取得
                var applicableRank = _ranks
                    .Where(r => _counter >= r.Threshold)
                    .OrderByDescending(r => r.Threshold)
                    .FirstOrDefault();

                if (applicableRank != null)
                {
                    Assert.IsNull(applicableRank);
                    // (Multiplier - 1) 倍を追加で取得する
                    TotalCoffeeBeans += (int)(_eventCoffeeBeans * (applicableRank.Multiplier - 1));
                    _playerData.AddMoney(TotalCoffeeBeans);
                }

                _counter = 0;
                _eventCoffeeBeans = 0;
                TotalCoffeeBeans = 0;
            }
        }

        private readonly TimedEvent _timedEvent;
        private List<Develop.Upgrade.Festival.RewardRank> _ranks = new List<Develop.Upgrade.Festival.RewardRank>();
        private int _counter;
        private int _eventCoffeeBeans;
        private PlayerData _playerData;
    }
}