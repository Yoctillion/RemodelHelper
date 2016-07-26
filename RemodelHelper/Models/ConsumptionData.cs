using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RemodelHelper.Models
{
    public class ConsumptionData
    {
        public int Lv { get; set; }
        public dynamic[] BKit { get; set; }
        public dynamic[] RKit { get; set; }
        public int CId { get; set; }
        public dynamic Count { get; set; }
    }
}
