using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Grabacr07.KanColleWrapper.Models;

namespace RemodelHelper.Models
{
    public abstract class SlotInfo : IIdentifiable
    {
        public int Id => this.Info?.Id ?? 0;

        public SlotItemInfo Info { get; }


        internal SlotInfo(SlotItemInfo info)
        {
            this.Info = info;
        }
    }


    public class BaseSlotInfo : SlotInfo
    {
        public IdentifiableTable<UpgradeSlotInfo> UpgradeSlots { get; } = new IdentifiableTable<UpgradeSlotInfo>();


        internal BaseSlotInfo(Item item) : base(item.GetBaseSlotInfo())
        {
            this.AddUpgradeSlot(item);
        }

        internal void AddUpgradeSlot(Item item)
        {
            if (!this.UpgradeSlots.ContainsKey(item.NewId))
                this.UpgradeSlots.Add(new UpgradeSlotInfo(item));
            else
                this.UpgradeSlots[item.NewId].AddAssistant(item);
        }

        public bool IsAvailable(DayOfWeek day) => this.UpgradeSlots.Values.Any(newSlot => newSlot.IsAvailable(day));
    }


    public class UpgradeSlotInfo : SlotInfo
    {
        public IdentifiableTable<AssistantInfo> Assistants { get; } = new IdentifiableTable<AssistantInfo>();

        public int Level { get; set; }

        internal UpgradeSlotInfo(Item item) : base(item.GetUpgradeSlotInfo())
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
