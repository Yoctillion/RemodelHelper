using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using Livet.Messaging;
using RemodelHelper.Models;
using RemodelHelper.Views;

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
            this.CurrentDay = DateTime.Now.DayOfWeek;
            RemodelData.Current.PropertyChanged += DataUpdated;
        }

        private void DataUpdated(object sender, System.ComponentModel.PropertyChangedEventArgs e) => Update();

        private async void Update()
        {
            this.Items = await RemodelData.Current.GetRemodelInfo(this.CurrentDay);
        }
    }
}
