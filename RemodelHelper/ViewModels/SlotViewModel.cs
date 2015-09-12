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
        public SlotItemInfo Info { get; set; }
    }

    class ItemViewModel : SlotViewModel
    {
        public IReadOnlyCollection<NewSlotViewModel> NewSlots { get; set; }
    }

    class NewSlotViewModel : SlotViewModel
    {
        public string Name => this.Info?.Name ?? "更新不可";

        public int Level { get; set; }

        public IReadOnlyCollection<ShipInfo> Ships { get; set; }
    }
}
