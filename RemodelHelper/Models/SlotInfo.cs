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


    public class ItemInfo : SlotInfo
    {
        public IdentifiableTable<NewSlotInfo> NewSlots { get; } = new IdentifiableTable<NewSlotInfo>();


        internal ItemInfo(Item item) : base(item.GetSlotInfo())
        {
            this.AddNewItem(item);
        }

        internal void AddNewItem(Item item)
        {
            if (!this.NewSlots.ContainsKey(item.NewId))
                this.NewSlots.Add(new NewSlotInfo(item));
            else
                this.NewSlots[item.NewId].AddShip(item);
        }

        public bool IsAvailable(DayOfWeek day) => this.NewSlots.Values.Any(newSlot => newSlot.IsAvailable(day));
    }


    public class NewSlotInfo : SlotInfo
    {
        public IdentifiableTable<ShipWeekInfo> Ships { get; } = new IdentifiableTable<ShipWeekInfo>();

        public int Level { get; set; }

        internal NewSlotInfo(Item item) : base(item.GetNewSlotInfo())
        {
            this.AddShip(item);
        }

        internal void AddShip(Item item)
        {
            this.Ships.Add(new ShipWeekInfo(item));
        }

        public bool IsAvailable(DayOfWeek day) => this.Ships.Values.Any(ship => ship.IsAvailable(day));
    }
}
