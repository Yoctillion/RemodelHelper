using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using MetroTrilithon.Mvvm;
using RemodelHelper.Models;

namespace RemodelHelper.ViewModels
{
    public abstract class ItemsViewModel : ViewModel
    {
        protected static RemodelDataProvider DataProvider => RemodelDataProvider.Current;


        private BaseSlotItemViewModel[] _baseSlotItems;

        public BaseSlotItemViewModel[] BaseSlotItems
        {
            get { return this._baseSlotItems; }
            set
            {
                if (this._baseSlotItems != value)
                {
                    this._baseSlotItems = value;
                    this.RaisePropertyChanged();
                }
            }
        }


        protected ItemsViewModel()
        {
            DataProvider
                .Subscribe(nameof(RemodelDataProvider.Items), this.Update, false)
                .AddTo(this);

            this.UpdateAction += this.UpdateSlotInfo;
        }

        protected event Action UpdateAction;

        public void Update()
        {
            this.UpdateAction?.Invoke();
        }

        public void UpdateSlotInfo()
        {
            this.BaseSlotItems = this.GetUpdateSlotItemInfo().ToArray();
        }

        protected virtual IEnumerable<BaseSlotItemViewModel> GetUpdateSlotItemInfo()
        {
            return DataProvider.Items.Values
                .Where(this.FilterBaseSlotItem)
                .Select(baseItem => new BaseSlotItemViewModel
                {
                    Info = baseItem.Info,
                    UpgradeSlotItems = baseItem.UpgradeSlotItems.Values
                        .Where(upgradeSlot => this.FilterUpgradeSlotItem(baseItem, upgradeSlot))
                        .Select(upgradeItem => new UpgradeSlotItemViewModel
                        {
                            Base = baseItem.Info,
                            Info = upgradeItem.Info,
                            Level = upgradeItem.Level,
                            Fuel = upgradeItem.Fuel,
                            Ammo = upgradeItem.Ammo,
                            Steel = upgradeItem.Steel,
                            Bauxite = upgradeItem.Bauxite,
                            Consumptions = upgradeItem.Consumptions
                                .Select(c => new ConsumptionViewModel
                                {
                                    Info = c,
                                    IsDifferent = c.ConsumeSlotItem != null && baseItem.Id != c.ConsumeSlotItem.Id,
                                })
                                .ToArray(),
                            NeedAssistant = upgradeItem.Assistants.Values.Any(ship => ship.ShipInfo == null),
                            Assistants = upgradeItem.Assistants.Values
                                .Where(a => this.FilterAssistant(baseItem, upgradeItem, a))
                                .ToArray(),
                            AssistantGroups = upgradeItem.Assistants.Values.Where(a => this.FilterAssistant(baseItem, upgradeItem, a))
                            .GroupBy(assistant => assistant.Week)
                            .Select(group => new AssistantGroupViewModel
                            {
                                Week = group.Key,
                                Assistants = group.ToArray(),
                            }).ToArray(),
                        })
                        .Where(newItem => newItem.NeedAssistant || newItem.Assistants.Length > 0)
                        .ToArray(),
                })
                .Where(item => item.UpgradeSlotItems.Length > 0);
        }

        protected virtual bool FilterBaseSlotItem(BaseSlotItemInfo baseSlotItem)
        {
            return true;
        }

        protected virtual bool FilterUpgradeSlotItem(BaseSlotItemInfo baseSlotItem, UpgradeSlotItemInfo upgradeSlotItem)
        {
            return true;
        }

        protected virtual bool FilterAssistant(BaseSlotItemInfo baseSlotItem, UpgradeSlotItemInfo upgradeSlotItem, AssistantInfo assistant)
        {
            return true;
        }
    }
}
