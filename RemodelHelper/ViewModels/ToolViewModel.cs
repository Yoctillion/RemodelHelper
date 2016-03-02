﻿using System;
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
            DataProvider
                .Subscribe(nameof(RemodelDataProvider.IsUpdating), () => this.IsReady = !DataProvider.IsUpdating)
                .AddTo(this);

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            var dayTrigger = DateChangeTrigger.GetTigger(timeZone);
            dayTrigger.DateChanged += (before, after) => this.CurrentDay = after.DayOfWeek;

            this._currentDay = dayTrigger.Today.DayOfWeek;
        }

        protected override bool FilterBaseSlotItem(BaseSlotItemInfo baseSlotItem)
        {
            if (!baseSlotItem.IsAvailable(this.CurrentDay)) return false;

            return base.FilterBaseSlotItem(baseSlotItem);
        }

        protected override bool FilterAssistant(BaseSlotItemInfo baseSlotItem, UpgradeSlotItemInfo upgradeSlotItem, AssistantInfo assistant)
        {
            return base.FilterAssistant(baseSlotItem, upgradeSlotItem, assistant) && assistant.IsAvailable(this.CurrentDay);
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
#if DEBUG
            DataProvider.Load();
#endif
            DataProvider.UpdateFromInternet();
        }
    }
}
