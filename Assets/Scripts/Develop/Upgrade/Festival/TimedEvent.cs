using System;

namespace Develop.Upgrade.Festival
{
    /// <summary>
    /// 時間経過によるイベントの発生と終了を管理する。
    /// </summary>
    public class TimedEvent
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public TimedEvent(IClock clock, IRandom random, int eventTime)
        {
            this._clock = clock;
            this._random = random;
            this._eventTime = eventTime;

            // 最初のイベント予約を行う
            ScheduleNextStart();
        }

        /// <summary>
        /// 現在イベントが実行中であるかどうか。
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// イベントが開始された時の通知。
        /// </summary>
        public event Action OnStarted;

        /// <summary>
        /// イベントが終了された時の通知。
        /// </summary>
        public event Action OnEnded;

        /// <summary>
        /// 時間経過を監視し、イベントの状態を遷移させる。
        /// </summary>
        public void Update()
        {
            // 開始時刻の判定
            if (!IsActive && _clock.Now >= _nextStartTime)
            {
                EventStart();
            }

            // 終了時刻の判定
            if (IsActive && _clock.Now >= _endTime)
            {
                EventEnd();
            }
        }

        private IClock _clock;
        private IRandom _random;
        private int _nextStartTime;
        private int _eventTime;
        private int _endTime;

        /// <summary>
        /// イベントを開始し、リスナーへ通知する。
        /// </summary>
        private void EventStart()
        {
            IsActive = true;
            _endTime = _clock.Now + _eventTime;

            // 開始を通知
            OnStarted?.Invoke();
        }

        /// <summary>
        /// イベントを終了し、リスナーへ通知する。
        /// </summary>
        private void EventEnd()
        {
            IsActive = false;

            // 終了を通知
            OnEnded?.Invoke();

            // 次回を予約
            ScheduleNextStart();
        }

        /// <summary>
        /// 30〜90秒の範囲で次回開始時刻を設定する。
        /// </summary>
        private void ScheduleNextStart()
        {
            // SystemRandom.Rangeにより最大値90を含む
            _nextStartTime = _clock.Now + _random.Range(30, 90);
        }
    }
}
