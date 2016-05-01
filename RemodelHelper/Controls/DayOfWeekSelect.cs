using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemodelHelper.Controls
{
    public class DayOfWeekSelect : Control
    {
        static DayOfWeekSelect()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DayOfWeekSelect), new FrameworkPropertyMetadata(typeof(DayOfWeekSelect)));
        }

        public DayOfWeek CurrentDayOfWeek
        {
            get { return (DayOfWeek)this.GetValue(CurrentDayOfWeekProperty); }
            set { this.SetValue(CurrentDayOfWeekProperty, value); }
        }

        public static readonly DependencyProperty CurrentDayOfWeekProperty =
            DependencyProperty.Register(nameof(CurrentDayOfWeek), typeof(DayOfWeek), typeof(DayOfWeekSelect), new UIPropertyMetadata(default(DayOfWeek)));
    }
}
