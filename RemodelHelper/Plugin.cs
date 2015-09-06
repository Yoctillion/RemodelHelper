using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;
using Livet;
using RemodelHelper.Models;
using RemodelHelper.ViewModels;
using RemodelHelper.Views;

namespace RemodelHelper
{
    [Export(typeof(ITool))]
    [Export(typeof(IPlugin))]
    [ExportMetadata("Title", "改修助手")]
    [ExportMetadata("Description", "改修工厂辅助插件")]
    [ExportMetadata("Version", "0.3.0")]
    [ExportMetadata("Author", "Yoctillion")]
    [ExportMetadata("Guid", "71C1EE7A-A153-437F-B75F-E3E22ED833F1")]
    class Plugin : ITool, IPlugin
    {
        private readonly ViewModel _vm = new ToolViewModel();

        public string Name => "改修助手";

        public object View => new ToolView { DataContext = _vm };

        public void Initialize()
        {
            RemodelDataProvider.Current.UpdateFromInternet();
        }
    }
}
