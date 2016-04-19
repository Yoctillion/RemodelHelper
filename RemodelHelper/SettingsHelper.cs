using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RemodelHelper.Properties;

namespace RemodelHelper
{
    internal class SettingsHelper
    {
        public static SettingsHelper Default = new SettingsHelper(Settings.Default);

        private readonly DispatcherTimer _timer;

        public Settings Settings { get; }

        /// <summary>
        /// 保存操作的延迟时间
        /// </summary>
        public TimeSpan SavingDelay { get; set; }

        public SettingsHelper(Settings settings)
        {
            this.Settings = settings;
            this.SavingDelay = TimeSpan.FromSeconds(1); // 默认 1 秒

            this._timer = new DispatcherTimer();
            this._timer.Tick += (s, e) =>
            {
                this.Settings.Save();
                this._timer.Stop();
            };
        }

        /// <summary>
        /// 在设定的延迟时间后保存。如果在保存完成前再次调用此方法，则重置延迟。
        /// </summary>
        public void SaveWithDelay()
        {
            this._timer.Interval = this.SavingDelay;

            if (!this._timer.IsEnabled)
            {
                this._timer.Start();
            }
        }
    }
}
