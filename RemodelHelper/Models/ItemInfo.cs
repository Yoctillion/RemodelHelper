using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Grabacr07.KanColleWrapper.Models;

namespace RemodelHelper.Models
{
    public abstract class ItemInfo : IIdentifiable
    {
        public int Id => this.Info?.Id ?? 0;

        public SlotItemInfo Info { get; }


        internal ItemInfo(SlotItemInfo info)
        {
            this.Info = info;
        }
    }


    public class BaseSlotItemInfo : ItemInfo
    {
        public IdentifiableTable<UpgradeSlotItemInfo> UpgradeSlotItems { get; } = new IdentifiableTable<UpgradeSlotItemInfo>();


        internal BaseSlotItemInfo(Item item) : base(item.GetBaseSlotInfo())
        {
            this.AddUpgradeSlotItem(item);
        }

        internal void AddUpgradeSlotItem(Item item)
        {
            if (!this.UpgradeSlotItems.ContainsKey(item.NewId))
                this.UpgradeSlotItems.Add(new UpgradeSlotItemInfo(item));
            else
                this.UpgradeSlotItems[item.NewId].AddAssistant(item);
        }

        public bool IsAvailable(DayOfWeek day) => this.UpgradeSlotItems.Values.Any(newSlot => newSlot.IsAvailable(day));
    }


    public class UpgradeSlotItemInfo : ItemInfo
    {
        public IdentifiableTable<AssistantInfo> Assistants { get; } = new IdentifiableTable<AssistantInfo>();

        public int Level { get; set; }

        public int Fuel { get; internal set; }
        public int Ammo { get; internal set; }
        public int Steel { get; internal set; }
        public int Bauxite { get; internal set; }

        public ConsumptionInfo[] Consumptions { get; internal set; }

        internal UpgradeSlotItemInfo(Item item) : base(item.GetUpgradeSlotInfo())
        {
            this.AddAssistant(item);
        }

        internal void AddAssistant(Item item)
        {
            this.Assistants.Add(new AssistantInfo(item));
        }

        public bool IsAvailable(DayOfWeek day) => this.Assistants.Values.Any(ship => ship.IsAvailable(day));
    }
}
