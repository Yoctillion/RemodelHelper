using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemodelHelper.Models
{
    public class UpgradeData
    {
        public int Id { get; set; }

        public int NewId { get; set; }

        public int Lv { get; set; }

        public int[] Meterials { get; set; }

        public ConsumptionData[] Consumption { get; set; }
    }
}
