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


        private BaseSlotViewModel[] _baseSlots;

        public BaseSlotViewModel[] BaseSlots
        {
            get { return this._baseSlots; }
            set
            {
                if (this._baseSlots != value)
                {
                    this._baseSlots = value;
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
            this.BaseSlots = this.GetUpdateSlotInfo().ToArray();
        }

        protected virtual IEnumerable<BaseSlotViewModel> GetUpdateSlotInfo()
        {
            return DataProvider.Items.Values
                .Where(this.FilterBaseSlot)
                .Select(bSlot => new BaseSlotViewModel
                {
                    Info = bSlot.Info,
                    UpgradeSlots = bSlot.UpgradeSlots.Values
                        .Where(upgradeSlot => this.FilterUpgradeSlot(bSlot, upgradeSlot))
                        .Select(uSlot => new UpgradeSlotViewModel
                        {
                            Info = uSlot.Info,
                            Level = uSlot.Level,
                            NeedAssistant = uSlot.Assistants.Values.Any(ship => ship.ShipInfo == null),
                            Assistants = uSlot.Assistants.Values
                                .Where(a => this.FilterAssistant(bSlot, uSlot, a))
                                .ToArray(),
                            AssistantGroups = uSlot.Assistants.Values.Where(a => this.FilterAssistant(bSlot, uSlot, a))
                            .GroupBy(assistant => assistant.Week)
                            .Select(group => new AssistantGroupViewModel
                            {
                                Week = group.Key,
                                Assistants = group.ToArray(),
                            }).ToArray(),
                        })
                        .Where(newSlot => newSlot.NeedAssistant || newSlot.Assistants.Length > 0)
                        .ToArray(),
                })
                .Where(item => item.UpgradeSlots.Length > 0);
        }

        protected virtual bool FilterBaseSlot(BaseSlotInfo baseSlot)
        {
            return true;
        }

        protected virtual bool FilterUpgradeSlot(BaseSlotInfo baseSlot, UpgradeSlotInfo upgradeSlot)
        {
            return true;
        }

        protected virtual bool FilterAssistant(BaseSlotInfo baseSlot, UpgradeSlotInfo upgradeSlot, AssistantInfo assistant)
        {
            return true;
        }
    }
}
