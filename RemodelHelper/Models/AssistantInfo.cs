using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace RemodelHelper.Models
{
    public class AssistantInfo : IIdentifiable
    {
        public int Id => this.ShipInfo?.Id ?? 0;

        public string Name => this.ShipInfo?.Name ?? "-";

        public ShipInfo ShipInfo { get; }

        public Week Week { get; }

        internal AssistantInfo(Item item)
        {
            this.ShipInfo = item.GetAssistantInfo();
            this.Week = item.Week;
        }

        public bool IsAvailable(DayOfWeek day) => this.Week.Contains(day);
    }
}
