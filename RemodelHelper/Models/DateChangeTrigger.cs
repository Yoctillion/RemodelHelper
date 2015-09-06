using System;
using System.Windows.Threading;

namespace RemodelHelper.Models
{
    public class DateChangeTrigger : IDisposable
    {
        private readonly DispatcherTimer _timer;

        private readonly TimeZoneInfo _timeZone;

        public event Action<DateTime, DateTime> DateChanged;

        private DateTime _currentDay;

        public DateTime Today => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, this._timeZone).Date;

        public bool IsEnabled { get; set; }


        public DateChangeTrigger(TimeZoneInfo info = null)
        {
            this._timeZone = info ?? TimeZoneInfo.Utc;

            this._timer = new DispatcherTimer { Interval = this.GetSleepTime() };
            this._timer.Tick += (x, y) =>
            {
                this._timer.Interval = this.GetSleepTime();
                
                if (this.IsEnabled) this.DateChanged?.Invoke(this._currentDay, this.Today);
                this._currentDay = this.Today;
            };
            this._timer.Start();
        }

        private TimeSpan GetSleepTime() => this.Today.AddDays(1) - DateTime.Now;

        public void Dispose()
        {
            this._timer.Stop();
        }
    }
}
