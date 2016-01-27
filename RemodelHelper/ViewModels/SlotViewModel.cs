using System;
using System.Collections;
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
    public class SlotViewModel : ViewModel
    {
        public SlotItemInfo Info { get; set; }

    }

    public class BaseSlotViewModel : SlotViewModel
    {
        public UpgradeSlotViewModel[] UpgradeSlots { get; set; }

        public bool IsAvailable(DayOfWeek day)
        {
            return this.UpgradeSlots.Any(slot => slot.IsAvailable(day));
        }
    }

    public class UpgradeSlotViewModel : SlotViewModel
    {
        public string Name => this.Info?.Name ?? "更新不可";

        public int Level { get; set; }

        public bool NeedAssistant { get; set; }

        public AssistantInfo[] Assistants { get; set; }

        public AssistantGroupViewModel[] AssistantGroups { get; set; }

        public bool IsAvailable(DayOfWeek day)
        {
            return this.Assistants.Any(assistant => assistant.IsAvailable(day));
        }
    }
}
