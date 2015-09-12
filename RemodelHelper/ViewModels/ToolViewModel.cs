using System;
using System.Collections.Generic;
using System.Linq;
using Livet;
using RemodelHelper.Models;

namespace RemodelHelper.ViewModels
{
    internal class ToolViewModel : ViewModel
    {
        private RemodelDataProvider DataProvider => RemodelDataProvider.Current;


        private IReadOnlyCollection<ItemViewModel> _items = new ItemViewModel[0];

        public IReadOnlyCollection<ItemViewModel> Items
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

        private IReadOnlyCollection<SlotTypeViewModel> _slotTypes;

        public IReadOnlyCollection<SlotTypeViewModel> SlotTypes
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

        public bool IsAllTypeSelected
        {
            get { return SlotTypes.All(type => type.IsSelected); }
            set
            {
                foreach (var type in SlotTypes) type.SetSelected(value);
                this.UpdateItemInfo();
            }
        }

        public IReadOnlyCollection<string> DaysOfWeek { get; } = new[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };

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
                    this.UpdateItemInfo();
                }
            }
        }


        public ToolViewModel()
        {
            DataProvider.PropertyChanged += (x, y) => this.Update();

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            var dayTrigger = new DateChangeTrigger(timeZone) { IsEnabled = true };
            dayTrigger.DateChanged += (before, after) => this.CurrentDay = after.DayOfWeek;

            this.CurrentDay = dayTrigger.Today.DayOfWeek;

            this.DataProvider.DataChanged += this.Update;
        }

        public void Update()
        {
            if (!DataProvider.IsUpdateFinished) return;

            this.UpdateSlotTypes();
            this.UpdateItemInfo();
        }

        public void UpdateSlotTypes()
        {
            this.SlotTypes = this.DataProvider.Items.Values
                .Select(slot => slot.Info.EquipType)
                .Distinct()
                .Where(type => type.Name != "大型探照灯" && type.Name != "大型電探")
                .Select(type => new SlotTypeViewModel(type) { SelectionChangedAction = this.UpdateItemInfo })
                .ToArray();

            this.RaisePropertyChanged(nameof(this.IsAllTypeSelected));
        }

        public void UpdateItemInfo()
        {
            this.Items = this.DataProvider.Items.Values
                .Where(this.IsItemAvailable)
                .Select(slot => new ItemViewModel
                {
                    Info = slot.Info,
                    NewSlots = slot.NewSlots.Values
                        .Where(newSlot => newSlot.IsAvailable(this.CurrentDay))
                        .Select(newSlot => new NewSlotViewModel
                        {
                            Info = newSlot.Info,
                            Level = newSlot.Level,
                            Ships = newSlot.Ships.Values
                                .Where(ship => ship.Info != null && ship.IsAvailable(this.CurrentDay))
                                .Select(ship => ship.Info)
                                .ToArray(),
                        }).ToArray(),
                })
                .ToArray();
        }

        private bool IsItemAvailable(ItemInfo item)
        {
            var slotType = item.Info.EquipType;
            return item.IsAvailable(this.CurrentDay)
                   && this.SlotTypes
                       .Where(type => type.IsSelected)
                       .Any(type => slotType.Id == type.Id || slotType.Name.Contains(type.Name));
        }
    }
}
