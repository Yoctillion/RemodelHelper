using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using RemodelHelper.Models;

namespace RemodelHelper.ViewModels
{
    class ItemViewModel : ViewModel
    {
        public SlotViewModel Slot { get; set; }

        public NewSlotViewModel[] NewSlot { get; set; }
    }

}
