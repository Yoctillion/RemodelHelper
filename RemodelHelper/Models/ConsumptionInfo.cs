using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace RemodelHelper.Models
{
    public class ConsumptionInfo
    {
        public int Level { get; internal set; }

        public KitCount BuildKit { get; internal set; }
        public KitCount RemodelKit { get; internal set; }

        public SlotItemInfo ConsumeSlotItem { get; internal set; }
        public int ConsumeCount { get; internal set; }
    }

    public class KitCount
    {
        public int Normal { get; internal set; }
        public int Ensure { get; internal set; }

        public override string ToString()
        {
            var n = this.Normal >= 0 ? this.Normal.ToString() : "-";
            var e = this.Ensure >= 0 ? this.Ensure.ToString() : "-";
            return $"{n}/{e}";
        }
    }
}
