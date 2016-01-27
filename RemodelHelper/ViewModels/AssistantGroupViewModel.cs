using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using RemodelHelper.Models;

namespace RemodelHelper.ViewModels
{
    public class AssistantGroupViewModel : ViewModel
    {
        public Week Week { get; set; }

        public AssistantInfo[] Assistants { get; set; }
    }
}
