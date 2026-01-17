using System;

namespace Develop.Upgrade.Festival
{
     public class TimedEvent
    {

        public TimedEvent(IClock clock, IRandom random,int eventTime)
        {
            this._clock = clock;
            this._random = random;
            this._eventTime = eventTime;
            ScheduleNextStart();
        }
        public bool IsActive { get; private set; }
        public int EventCounter { get; private set; }

        public void Update()
        {
            if (!IsActive && _clock.Now >= _nextStartTime)
            {
                EventStart();
            }

            if (IsActive && _clock.Now >= _endTime)
            {
                EventEnd();
            }
        }
        /// <summary>
        ///     採取処理を行う。
        ///     イベントがアクティブな場合のみカウンターを増加させる。
        /// </summary>
        public void Harvest()
        {
            if (!IsActive)
            {
                return;
            }

            EventCounter++;
        }


        private void EventStart()
        {
            IsActive = true;
            _endTime = _clock.Now + _eventTime;
        }
        private void EventEnd()
        {
            IsActive = false;
            ScheduleNextStart();
        }

        private void ScheduleNextStart()
        {
            _nextStartTime = _clock.Now + _random.Range(30, 90);
        }


        private IClock _clock;
        private IRandom _random;
        private int _nextStartTime;
        private int _eventTime;
        private int _endTime;

    }
}