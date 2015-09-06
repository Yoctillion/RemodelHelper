using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace RemodelHelper.Models
{
    public class ShipWeekInfo : IIdentifiable
    {
        public int Id => this.Info?.Id ?? 0;

        public string Name => this.Info.Name;

        public ShipInfo Info { get; }

        public Week Week { get; }

        internal ShipWeekInfo(Item item)
        {
            this.Info = item.GetShipInfo();
            this.Week = item.Week;
        }

        public bool IsAvailable(DayOfWeek day) => this.Week.Contains(day);
    }
}
