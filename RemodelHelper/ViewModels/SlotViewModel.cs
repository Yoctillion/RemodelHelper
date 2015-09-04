using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using RemodelHelper.Models;

namespace RemodelHelper.ViewModels
{
    class SlotViewModel : ViewModel
    {
        public string Name { get; set; }

        public SlotItemIconType Type { get; set; }

    }

    class NewSlotViewModel : SlotViewModel
    {
        public ShipViewModel[] Ships { get; set; }
    }
}
