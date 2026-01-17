namespace Develop.Upgrade.Festival
{
     public class TimedEvent
    {

        public TimedEvent(FakeClock clock, FakeRandom random)
        {
            this._clock = clock;
            this._random = random;

            _nextStartTime = _clock.Now + _random.Range(30, 90);
        }
        public bool IsActiv { get; private set; }

        public void Update()
        {
            if(!IsActiv && _clock.Now >= _nextStartTime)
                IsActiv = true;
        }

        private FakeClock _clock;
        private FakeRandom _random;
        private int _nextStartTime;

    }
}