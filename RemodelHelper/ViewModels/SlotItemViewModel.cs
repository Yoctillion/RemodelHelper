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
    public class SlotItemViewModel : ViewModel
    {
        public SlotItemInfo Base { get; set; }

        public SlotItemInfo Info { get; set; }

        public string Name => this.Info?.Name ?? "更新不可";

        public bool HasValue => this.Info != null;

        public int Level { get; set; }
    }

    public class BaseSlotItemViewModel : SlotItemViewModel
    {
        public UpgradeSlotItemViewModel[] UpgradeSlotItems { get; set; }

        public bool IsAvailable(DayOfWeek day)
        {
            return this.UpgradeSlotItems.Any(slot => slot.IsAvailable(day));
        }
    }

    public class UpgradeSlotItemViewModel : SlotItemViewModel
    {
        public int Fuel { get; set; }

        public int Ammo { get; set; }

        public int Steel { get; set; }

        public int Bauxite { get; set; }

        public ConsumptionViewModel[] Consumptions { get; set; }

        public bool NeedAssistant { get; set; }

        public AssistantInfo[] Assistants { get; set; }

        public AssistantGroupViewModel[] AssistantGroups { get; set; }

        public bool IsAvailable(DayOfWeek day)
        {
            return this.Assistants.Any(assistant => assistant.IsAvailable(day));
        }
    }
}
