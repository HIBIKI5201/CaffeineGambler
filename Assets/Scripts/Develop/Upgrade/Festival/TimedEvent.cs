using System;

namespace Develop.Upgrade.Festival
{
     public class TimedEvent
    {

        public TimedEvent(FakeClock clock, FakeRandom random,int eventTime)
        {
            this._clock = clock;
            this._random = random;
            this._eventTime = eventTime;
            _nextStartTime = _clock.Now + _random.Range(30, 90);
        }
        public bool IsActive { get; private set; }

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

        private FakeClock _clock;
        private FakeRandom _random;
        private int _nextStartTime;
        private int _eventTime;
        private int _endTime;

    }
}