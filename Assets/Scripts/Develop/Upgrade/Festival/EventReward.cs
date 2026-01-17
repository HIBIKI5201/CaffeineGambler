


using Develop.Upgrade.Festival;

namespace Domain
{
    /// <summary>
    /// イベント中の報酬ルールを管理するドメイン。
    /// </summary>
    public class EventReward
    {

        public int TotalCoffeeBeans { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public EventReward(
            TimedEvent timedEvent,
            int threshold,
            int multiplier)
        {
            _timedEvent = timedEvent;
            _threshold = threshold;
            _multiplier = multiplier;
        }

        /// <summary>
        /// イベント開始時の処理。
        /// カウンターや報酬をリセットする。
        /// </summary>
        public void OnEventStarted()
        {
            _counter = 0;
            _eventCoffeeBeans = 0;
            TotalCoffeeBeans = 0;
        }

        /// <summary>
        /// 採取が行われた事実を通知する。
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
        /// コーヒー豆を取得した事実を通知する。
        /// </summary>
        public void AddCoffeeBeans(int amount)
        {
            if (!_timedEvent.IsActive)
            {
                return;
            }

            _eventCoffeeBeans += amount;
            TotalCoffeeBeans += amount;
        }

        /// <summary>
        /// イベント終了時の報酬確定処理。
        /// </summary>
        public void OnEventEnded()
        {
            if (!_timedEvent.IsActive)
            {
                if (_counter >= _threshold)
                {
                    TotalCoffeeBeans +=
                        _eventCoffeeBeans * (_multiplier - 1);
                }
            }
        }

        private readonly TimedEvent _timedEvent;
        private readonly int _threshold; // m
        private readonly int _multiplier; // n 

        private int _counter;
        private int _eventCoffeeBeans;
    }
}
