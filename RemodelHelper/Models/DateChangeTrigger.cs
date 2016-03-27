using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Grabacr07.KanColleWrapper;

namespace RemodelHelper.Models
{
    public class DateChangeTrigger : Notifier
    {
        private readonly DispatcherTimer _timer;

        public TimeZoneInfo TimeZone { get; }

        public event Action<DateTime, DateTime> DateChanged;

        private DateTime _currentDay;

        public DateTime Today
        {
            get { return this._currentDay; }
            private set
            {
                if (this._currentDay != value)
                {
                    this._currentDay = value;
                    this.RaisePropertyChanged();
                }
            }
        }


        private DateChangeTrigger(TimeZoneInfo info)
        {
            this.TimeZone = info;
            this.Today = this.GetToday();

            this._timer = new DispatcherTimer { Interval = this.GetSleepTime() };
            this._timer.Tick += (x, y) =>
            {
                this._timer.Interval = this.GetSleepTime();

                var today = this.GetToday();
                this.DateChanged?.Invoke(this._currentDay, today);
                this.Today = today;
            };
            this._timer.Start();
        }

        private DateTime GetToday()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, this.TimeZone).Date;
        }

        private TimeSpan GetSleepTime() => this.GetToday().AddDays(1) - DateTime.Now;

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
