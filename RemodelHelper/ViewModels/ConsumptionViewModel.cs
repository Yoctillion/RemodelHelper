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
    public class ConsumptionViewModel : ViewModel
    {
        public string Name
        {
            get
            {
                if (this.Info != null)
                {
                    switch (this.Info.Level)
                    {
                        case 0:
                            return "初期";
                        case 6:
                            return "★6";
                        case 10:
                            return "★max";
                    }
                }

                return string.Empty;
            }
        }

        private bool _isDifferent;

        public bool IsDifferent
        {
            get { return this._isDifferent; }
            set
            {
                if (this._isDifferent != value)
                {
                    this._isDifferent = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private ConsumptionInfo _info;

        public ConsumptionInfo Info
        {
            get { return this._info; }
            set
            {
                if (this._info != value)
                {
                    this._info = value;
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged(nameof(this.Name));
                }
            }
        }
    }
}
