using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Grabacr07.KanColleWrapper;
using RemodelHelper.Models;

namespace RemodelHelper.ViewModels
{
    public class DetailViewModel : ItemsViewModel
    {
        public string[] DaysOfWeek { get; } =
            {"周日（日）", "周一（月）", "周二（水）", "周三（火）", "周四（木）", "周五（金）", "周六（土）"};

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

                    if (this.IsOnlyShowCurrentDay)
                    {
                        this.UpdateSlotInfo();
                    }
                }
            }
        }


        private SlotTypeViewModel[] _slotTypes;

        public SlotTypeViewModel[] SlotTypes
        {
            get { return this._slotTypes; }
            set
            {
                if (this._slotTypes != value)
                {
                    this._slotTypes = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool? IsAllTypeSelected
        {
            get
            {
                if (SlotTypes.All(type => type.IsSelected))
                    return true;
                if (SlotTypes.All(type => !type.IsSelected))
                    return false;
                else
                    return null;
            }
            set
            {
                if (value != null)
                {
                    foreach (var type in SlotTypes) type.SetSelected((bool)value);
                    this.UpdateSlotInfo();
                }
            }
        }

        private bool _isOnlyShowAvailable;

        public bool IsOnlyShowAvailable
        {
            get { return this._isOnlyShowAvailable; }
            set
            {
                if (this._isOnlyShowAvailable != value)
                {
                    this._isOnlyShowAvailable = value;
                    this.RaisePropertyChanged();
                    this.UpdateSlotInfo();
                }
            }
        }

        private bool _isOnlyShowCurrentDay;

        public bool IsOnlyShowCurrentDay
        {
            get { return this._isOnlyShowCurrentDay; }
            set
            {
                if (this._isOnlyShowCurrentDay != value)
                {
                    this._isOnlyShowCurrentDay = value;
                    this.RaisePropertyChanged();
                    this.UpdateSlotInfo();
                }
            }
        }

        private readonly DateChangeTrigger _dayTrigger;

        public DetailViewModel()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            this._dayTrigger = DateChangeTrigger.GetTigger(timeZone);
            this._dayTrigger.DateChanged += (before, after) => this.CurrentDay = after.DayOfWeek;

            this.CurrentDay = this._dayTrigger.Today.DayOfWeek;

            DataProvider.DataChanged += this.Update;

            this.UpdateAction -= this.UpdateSlotInfo;
            this.UpdateAction += this.UpdateSlotTypes;
            this.UpdateAction += this.UpdateSlotInfo;

            this.Update();
        }


        public void UpdateSlotTypes()
        {
            this.SlotTypes = DataProvider.Items.Values
                .Select(slot => slot.Info.EquipType)
                .Distinct()
                .Select(type => new SlotTypeViewModel(type) { SelectionChangedAction = this.SlotTypeSelectionChanged })
                .ToArray();

            this.RaisePropertyChanged(nameof(this.IsAllTypeSelected));
        }

        private void SlotTypeSelectionChanged()
        {
            this.UpdateSlotInfo();
            this.RaisePropertyChanged(nameof(this.IsAllTypeSelected));
        }

        protected override bool FilterBaseSlot(BaseSlotInfo baseSlot)
        {
            if (IsOnlyShowCurrentDay)
            {
                // 今日不可改修
                if (!baseSlot.IsAvailable(this.CurrentDay)) return false;
            }

            if (this.IsOnlyShowAvailable)
            {
                // 无此装备
                var slotId = baseSlot.Info.Id;
                if (KanColleClient.Current.Homeport.Itemyard.SlotItems.Values.All(slot => slot.Info.Id != slotId))
                {
                    return false;
                }
            }

            // filter
            if (this.SlotTypes != null)
            {
                var slotType = baseSlot.Info.EquipType;
                if (!this.SlotTypes.Where(type => type.IsSelected).Any(type => type.Equals(slotType)))
                {
                    return false;
                }
            }

            return base.FilterBaseSlot(baseSlot);
        }

        protected override bool FilterUpgradeSlot(BaseSlotInfo baseSlot, UpgradeSlotInfo upgradeSlot)
        {
            // All Lv Max & no upgrade
            if (upgradeSlot.Info == null)
            {
                var bId = baseSlot.Id;
                if (
                    KanColleClient.Current.Homeport.Itemyard.SlotItems.Values
                        .Where(slot => slot.Info.Id == bId)
                        .All(slot => slot.Level == 10))
                {
                    return false;
                }
            }
            return base.FilterUpgradeSlot(baseSlot, upgradeSlot);
        }

        protected override bool FilterAssistant(BaseSlotInfo baseSlot, UpgradeSlotInfo upgradeSlot,
            AssistantInfo assistant)
        {
            if (this.IsOnlyShowCurrentDay)
            {
                // 今日不可改修
                if (!assistant.IsAvailable(this.CurrentDay)) return false;
            }

            if (this.IsOnlyShowAvailable)
            {
                // 不需要二号舰
                if (assistant.ShipInfo == null) return true;

                // 检查是否拥有舰娘
                var shipId = assistant.ShipInfo.Id;
                var client = KanColleClient.Current;
                while (client.Homeport.Organization.Ships.Values.All(ship => ship.Info.Id != shipId))
                {
                    int.TryParse(client.Master.Ships[shipId].RawData.api_aftershipid, out shipId);
                    if (shipId == 0) return false;
                }

                return true;
            }

            return base.FilterAssistant(baseSlot, upgradeSlot, assistant);
        }

        public void BackToToday()
        {
            this.CurrentDay = this._dayTrigger.Today.DayOfWeek;
        }
    }
}
