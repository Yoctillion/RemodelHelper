using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using RemodelHelper.Models;

namespace RemodelHelper.ViewModels
{
    class ToolViewModel : ViewModel
    {
        private Master Master => KanColleClient.Current.Master;

        private RemodelData Data => RemodelData.Current;


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
                this.UpdateSlotInfo();
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
                    this.UpdateSlotInfo();
                }
            }
        }


        public ToolViewModel()
        {
            this.DaysOfWeek = new[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };

            RemodelData.Current.PropertyChanged += (x, y) => this.Update();

            var dayTrigger = new DayOfWeekChangeTrigger(TimeSpan.FromMinutes(1), 9) { IsEnabled = true };
            dayTrigger.DateChanged += this.UpdateDate;

            this.CurrentDay = dayTrigger.Today;

            this.Update();
        }

        public void Update()
        {
            this.UpdateSlotTypes();
            this.UpdateSlotInfo();
        }

        public async void UpdateSlotTypes()
        {
            this.SlotTypes = await Task.Run(() => this.GetSlotTypes());
        }

        private IReadOnlyCollection<SlotTypeViewModel> GetSlotTypes()
        {
            this.WaitForMaster();

            return this.Data.Items
                .Select(item => item.SlotId)
                .Select(id => Master.SlotItems[id]?.EquipType)
                .Where(type => type != null && type.Name != "大型探照灯" && type.Name != "大型電探")
                .Distinct()
                .Select(type => new SlotTypeViewModel(type) { SelectionChangedAction = this.UpdateSlotInfo })
                .ToArray();
        }

        public async void UpdateSlotInfo()
        {
            this.RaisePropertyChanged(nameof(this.IsAllTypeSelected));
            this.Items = await Task.Run(() => this.GetSlotInfo());
        }

        private IReadOnlyCollection<ItemViewModel> GetSlotInfo()
        {
            // 等待Master初始化完成
            this.WaitForMaster();
            // 等待装备类型信息初始化完成
            while (this.SlotTypes == null) Thread.Sleep(100);

            return this.Data.Items
                .Where(this.IsItemAvailable)
                .GroupBy(item => item.SlotId)
                .Select(slotGroup => new ItemViewModel
                {
                    Slot = new SlotViewModel
                    {
                        Name = Master.SlotItems[slotGroup.Key]?.Name ?? "未知",
                        Type = Master.SlotItems[slotGroup.Key]?.IconType ?? SlotItemIconType.Unknown,
                    },

                    NewSlot = slotGroup.GroupBy(slot => slot.NewId).Select(newSlots => new NewSlotViewModel
                    {
                        Name = Master.SlotItems[newSlots.Key]?.Name ?? "更新不可",
                        Type = Master.SlotItems[newSlots.Key]?.IconType ?? SlotItemIconType.Unknown,

                        Ships =
                            newSlots.Select(item => new ShipViewModel { Name = Master.Ships[item.ShipId]?.Name ?? "" })
                                .ToArray(),
                    }).ToArray(),
                }).ToArray();
        }

        private void WaitForMaster()
        {
            while (Master == null) Thread.Sleep(100);
        }

        private void UpdateDate(DayOfWeek before, DayOfWeek after)
        {
            if (this.CurrentDay == before)
                this.CurrentDay = after;
        }

        private bool IsItemAvailable(Item item)
        {
            var slotType = this.Master.SlotItems[item.SlotId]?.EquipType;
            return item.Week.HasFlag(this.CurrentDay.Convert())
                   && this.SlotTypes
                       .Where(type => type.IsSelected)
                       .Any(type => slotType?.Id == type.Id
                                    || (slotType?.Name.Contains(type.Name) ?? false));
        }
    }
}
