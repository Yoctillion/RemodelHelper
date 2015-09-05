using System;
using System.Collections.Generic;
using Livet;
using RemodelHelper.Models;

namespace RemodelHelper.ViewModels
{
    class ToolViewModel : ViewModel
    {
        private ItemViewModel[] _items = new ItemViewModel[0];

        public ItemViewModel[] Items
        {
            get { return this._items; }
            set
            {
                if (this._items != value)
                {
                    this._items = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public IReadOnlyCollection<string> DaysOfWeek { get; }

        private DayOfWeek _currentDay;

        public DayOfWeek CurrentDay
        {
            get { return this._currentDay; }
            set
            {
                if (this._currentDay != value)
                {
                    this._currentDay = value;
                    this.RaisePropertyChanged();
                    this.Update();
                }
            }
        }


        public ToolViewModel()
        {
            DaysOfWeek = new[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };

            RemodelData.Current.PropertyChanged += DataUpdated;

            var dayTrigger = new DayOfWeekChangeTrigger(TimeSpan.FromMinutes(1), 9) { IsEnabled = true };
            dayTrigger.DateChanged += this.UpdateDate;
            this.CurrentDay = dayTrigger.Today;
        }

        private void DataUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e) => Update();

        private async void Update()
        {
            this.Items = await RemodelData.Current.GetRemodelInfo(this.CurrentDay);
        }

        private void UpdateDate(DayOfWeek before, DayOfWeek after)
        {
            if (this.CurrentDay == before)
                this.CurrentDay = after;
        }
    }
}
