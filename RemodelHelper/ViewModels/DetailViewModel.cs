using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Mvvm;
using RemodelHelper.Models;

namespace RemodelHelper.ViewModels
{
    public class DetailViewModel : ItemsViewModel
    {
        public string[] DaysOfWeek { get; } =
            {"周日（日）", "周一（月）", "周二（水）", "周三（火）", "周四（木）", "周五（金）", "周六（土）"};

        private int _width;

        public int Width
        {
            get { return this._width; }
            set
            {
                if (this._width != value)
                {
                    this._width = value;
                    this.RaisePropertyChanged();
                }
            }
        }

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


        private SlotItemTypeViewModel[] _slotItemTypes;

        public SlotItemTypeViewModel[] SlotItemTypes
        {
            get { return this._slotItemTypes; }
            set
            {
                if (this._slotItemTypes != value)
                {
                    this._slotItemTypes = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool? IsAllTypeSelected
        {
            get
            {
                if (SlotItemTypes.All(type => type.IsSelected))
                    return true;
                if (SlotItemTypes.All(type => !type.IsSelected))
                    return false;
                else
                    return null;
            }
            set
            {
                if (value != null)
                {
                    foreach (var type in SlotItemTypes) type.SetSelected((bool)value);
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


        private bool _isHideConsumption;

        public bool IsHideConsumption
        {
            get { return this._isHideConsumption; }
            set
            {
                if (this._isHideConsumption != value)
                {
                    this._isHideConsumption = value;
                    this.RaisePropertyChanged();
                    this.ConsumptionVisibility = value ? Visibility.Collapsed : Visibility.Visible;
                    this.Width = value ? 750 : 1050;
                }
            }
        }

        private Visibility _consumptionVisibility;

        public Visibility ConsumptionVisibility
        {
            get { return this._consumptionVisibility; }
            set
            {
                if (this._consumptionVisibility != value)
                {
                    this._consumptionVisibility = value;
                    this.RaisePropertyChanged();
                }
            }
        }


        private readonly DateChangeTrigger _dayTrigger;

        public DetailViewModel()
        {
            this.Width = 1050;

            this.UpdateAction += this.UpdateSlotTypes;

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            this._dayTrigger = DateChangeTrigger.GetTigger(timeZone);
            this._dayTrigger.DateChanged += (before, after) => this.CurrentDay = after.DayOfWeek;

            this._currentDay = this._dayTrigger.Today.DayOfWeek;

            this.Update();
        }


        public void UpdateSlotTypes()
        {
            this.SlotItemTypes = DataProvider.Items.Values
                .Select(s => s.Info.EquipType)
                .Distinct()
                .Select(type => new SlotItemTypeViewModel(type) { SelectionChangedAction = this.SlotTypeSelectionChanged })
                .ToArray();

            this.RaisePropertyChanged(nameof(this.IsAllTypeSelected));
        }

        private void SlotTypeSelectionChanged()
        {
            this.UpdateSlotInfo();
            this.RaisePropertyChanged(nameof(this.IsAllTypeSelected));
        }

        protected override bool FilterBaseSlotItem(BaseSlotItemInfo baseSlotItem)
        {
            if (IsOnlyShowCurrentDay)
            {
                // 今日不可改修
                if (!baseSlotItem.IsAvailable(this.CurrentDay)) return false;
            }

            if (this.IsOnlyShowAvailable)
            {
                // 无此装备
                if (KanColleClient.Current.Homeport.Itemyard.SlotItems.Values.All(s => s.Info != baseSlotItem.Info))
                {
                    return false;
                }
            }

            // filter
            if (this.SlotItemTypes != null)
            {
                var slotType = baseSlotItem.Info.EquipType;
                if (this.SlotItemTypes.Where(type => !type.IsSelected).Any(type => type.Equals(slotType)))
                {
                    return false;
                }
            }

            return base.FilterBaseSlotItem(baseSlotItem);
        }

        protected override bool FilterUpgradeSlotItem(BaseSlotItemInfo baseSlotItem, UpgradeSlotItemInfo upgradeSlotItem)
        {
            if (this.IsOnlyShowAvailable)
            {
                var materials = KanColleClient.Current.Homeport.Materials;
                var slotItems = KanColleClient.Current.Homeport.Itemyard.SlotItems;

                // 资材
                if (materials.Fuel < upgradeSlotItem.Fuel ||
                    materials.Ammunition < upgradeSlotItem.Ammo ||
                    materials.Steel < upgradeSlotItem.Steel ||
                    materials.Bauxite < upgradeSlotItem.Bauxite)
                {
                    return false;
                }

                var baseItems = slotItems.Values
                    .Where(s => s.Info == baseSlotItem.Info)
                    .ToArray();
                // All Lv Max & no upgrade
                if (upgradeSlotItem.Info == null && baseItems.All(s => s.Level == 10))
                {
                    return false;
                }

                // 不同等级阶段的开发资材/改修资材/装备消耗
                var lvs = new[] { 0, 6, 10, int.MaxValue };
                var canRemodel = upgradeSlotItem.Consumptions
                    .Where((t, i) => baseItems.Any(s => s.Level >= lvs[i] && s.Level < lvs[i + 1]))
                    .Aggregate(false, (c, v) => c | this.Check(v));

                if (!canRemodel)
                {
                    return false;
                }
            }

            return base.FilterUpgradeSlotItem(baseSlotItem, upgradeSlotItem);
        }

        private bool Check(ConsumptionInfo info)
        {
            var materials = KanColleClient.Current.Homeport.Materials;
            var slotItems = KanColleClient.Current.Homeport.Itemyard.SlotItems;

            return materials.DevelopmentMaterials >= info.BuildKit.Normal &&
                   materials.ImprovementMaterials >= info.RemodelKit.Normal &&
                   slotItems.Values.Count(s => s.Info == info.ConsumeSlotItem && s.Level == 0 && s.RawData.api_locked == 0) >= info.ConsumeCount;
        }

        protected override bool FilterAssistant(BaseSlotItemInfo baseSlotItem, UpgradeSlotItemInfo upgradeSlotItem,
            AssistantInfo assistant)
        {
            if (this.IsOnlyShowCurrentDay)
            {
                // 今日不可改修
                if (!assistant.IsAvailable(this.CurrentDay)) return false;
            }

            if (this.IsOnlyShowAvailable)
            {
                // 需要二号舰
                if (assistant.ShipInfo != null)
                {
                    // 检查是否拥有舰娘
                    var shipId = assistant.ShipInfo.Id;
                    var client = KanColleClient.Current;
                    var set = new HashSet<int>();
                    while (client.Homeport.Organization.Ships.Values.All(ship => ship.Info.Id != shipId))
                    {
                        // 改造链循环（如霞改二/乙）
                        if (!set.Add(shipId)) return false;

                        int.TryParse(client.Master.Ships[shipId].RawData.api_aftershipid, out shipId);
                        // 没有后续改造舰娘
                        if (shipId == 0) return false;
                    }
                }
            }

            return base.FilterAssistant(baseSlotItem, upgradeSlotItem, assistant);
        }

        public void BackToToday()
        {
            this.CurrentDay = this._dayTrigger.Today.DayOfWeek;
        }
    }
}
