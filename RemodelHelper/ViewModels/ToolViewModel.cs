using System;
using System.Collections.Generic;
using System.Linq;
using Livet;
using Livet.Messaging;
using MetroTrilithon.Mvvm;
using RemodelHelper.Models;

namespace RemodelHelper.ViewModels
{
    public class ToolViewModel : ItemsViewModel
    {
        public string[] DaysOfWeek { get; } =
            { "周日（日）", "周一（月）", "周二（水）", "周三（火）", "周四（木）", "周五（金）", "周六（土）" };

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
                    this.UpdateSlotInfo();
                }
            }
        }

        private bool _isReady;

        public bool IsReady
        {
            get { return this._isReady; }
            set
            {
                if (this._isReady != value)
                {
                    this._isReady = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ToolViewModel()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            var dayTrigger = DateChangeTrigger.GetTigger(timeZone);
            dayTrigger.DateChanged += (before, after) => this.CurrentDay = after.DayOfWeek;

            this.CurrentDay = dayTrigger.Today.DayOfWeek;

            DataProvider
                .Subscribe(nameof(RemodelDataProvider.Items), this.Update, false)
                .AddTo(this);

            DataProvider
                .Subscribe(nameof(RemodelDataProvider.IsUpdating), () => this.IsReady = !DataProvider.IsUpdating)
                .AddTo(this);
        }

        protected override IEnumerable<BaseSlotViewModel> GetUpdateSlotInfo()
        {
            return base.GetUpdateSlotInfo().Where(item => item.IsAvailable(this.CurrentDay));
        }

        protected override bool FilterBaseSlot(BaseSlotInfo baseSlot)
        {
            if (!baseSlot.IsAvailable(this.CurrentDay)) return false;

            return base.FilterBaseSlot(baseSlot);
        }

        protected override bool FilterAssistant(BaseSlotInfo baseSlot, UpgradeSlotInfo upgradeSlot, AssistantInfo assistant)
        {
            return base.FilterAssistant(baseSlot, upgradeSlot, assistant) && assistant.IsAvailable(this.CurrentDay);
        }

        public void OpenDetailWindow()
        {
            var message = new TransitionMessage("Show/DetailWindow")
            {
                TransitionViewModel = new DetailViewModel()
            };
            this.Messenger.Raise(message);
        }

        public void UpdateData()
        {
            DataProvider.UpdateFromInternet();
        }
    }
}
