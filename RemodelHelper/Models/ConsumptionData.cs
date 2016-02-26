using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemodelHelper.Models
{
    public class ConsumptionData
    {
        public int Lv { get; set; }
        public int[] BKit { get; set; }
        public int[] RKit { get; set; }
        public int CId { get; set; }
        public int Count { get; set; }
    }
}
