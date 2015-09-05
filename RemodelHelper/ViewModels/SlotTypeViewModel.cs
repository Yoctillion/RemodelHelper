using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace RemodelHelper.ViewModels
{
    public delegate void SelectionChangedEvent(bool selected);

    class SlotTypeViewModel : ViewModel
    {
        public Action SelectionChangedAction { get; set; }
        
        public int Id { get; }

        public string Name { get; }

        private bool _isSelected = true;

        public bool IsSelected
        {
            get { return this._isSelected; }
            set
            {
                if (this._isSelected != value)
                {
                    this._isSelected = value;
                    this.RaisePropertyChanged();
                    this.SelectionChangedAction?.Invoke();
                }
            }
        }

        public SlotTypeViewModel(SlotItemEquipType type)
        {
            this.Id = type.Id;
            this.Name = type.Name != "大型電探" && type.Name != "小型電探" ? type.Name : "電探";
        }

        public void SetSelected(bool value)
        {
            this._isSelected = value;
            this.RaisePropertyChanged(nameof(this.IsSelected));
        }

    }
}
