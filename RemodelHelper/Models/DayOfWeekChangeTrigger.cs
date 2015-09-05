using System;
using System.Windows.Threading;

namespace RemodelHelper.Models
{
    public delegate void DateChangeEvent(DayOfWeek before, DayOfWeek after);

    public class DayOfWeekChangeTrigger
    {
        private DayOfWeek _today;

        public DayOfWeek Today
        {
            get { return this._today; }
            private set
            {
                if (this._today != value)
                {
                    var old = this._today;
                    this._today = value;

                    if (this.IsEnabled)
                        this.DateChanged?.Invoke(old, value);
                }
            }
        }

        private readonly int _utcOffset;

        private DayOfWeek CurrentDayOfWeek => DateTime.UtcNow.AddHours(this._utcOffset).DayOfWeek;

        public event DateChangeEvent DateChanged;

        public bool IsEnabled { get; set; }

        public DayOfWeekChangeTrigger(TimeSpan timeSpan = default(TimeSpan), int utcOffset = 0)
        {
            if (timeSpan == default(TimeSpan)) timeSpan = TimeSpan.FromSeconds(1);
            this._utcOffset = utcOffset;

            var timer = new DispatcherTimer { Interval = timeSpan };
            timer.Tick += (x, y) => this.Today = this.CurrentDayOfWeek;
            timer.IsEnabled = true;
        }
    }
}
