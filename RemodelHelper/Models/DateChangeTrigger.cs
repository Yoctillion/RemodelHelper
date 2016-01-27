using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace RemodelHelper.Models
{
    public class DateChangeTrigger : IDisposable
    {
        private readonly DispatcherTimer _timer;

        public TimeZoneInfo TimeZone { get; }

        public event Action<DateTime, DateTime> DateChanged;

        private DateTime _currentDay;

        public DateTime Today => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, this.TimeZone).Date;


        private DateChangeTrigger(TimeZoneInfo info)
        {
            this.TimeZone = info;
            this._currentDay = this.Today;

            this._timer = new DispatcherTimer { Interval = this.GetSleepTime() };
            this._timer.Tick += (x, y) =>
            {
                this._timer.Interval = this.GetSleepTime();

                this.DateChanged?.Invoke(this._currentDay, this.Today);
                this._currentDay = this.Today;
            };
            this._timer.Start();
        }

        private TimeSpan GetSleepTime() => this.Today.AddDays(1) - DateTime.Now;

        public void Dispose()
        {
            this._timer.Stop();
        }


        private static readonly Dictionary<TimeZoneInfo, DateChangeTrigger> Triggers = new Dictionary<TimeZoneInfo, DateChangeTrigger>();

        public static DateChangeTrigger GetTigger(TimeZoneInfo info = null)
        {
            if (info == null) info = TimeZoneInfo.Utc;

            DateChangeTrigger trigger;
            if (!Triggers.TryGetValue(info, out trigger))
            {
                trigger = new DateChangeTrigger(info);
                Triggers.Add(info, trigger);
            }

            return trigger;
        }
    }
}
