using System;

namespace Develop.Upgrade.Festival
{
    public class TimedEvent
    {
        public event Action OnStarted;
        public event Action OnEnded;
        public TimedEvent(IClock clock, IRandom random, int eventTime)
        {
            this._clock = clock;
            this._random = random;
            this._eventTime = eventTime;
            ScheduleNextStart();
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
            OnStarted?.Invoke();
        }
        private void EventEnd()
        {
            IsActive = false;
            ScheduleNextStart();
            OnEnded?.Invoke();
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