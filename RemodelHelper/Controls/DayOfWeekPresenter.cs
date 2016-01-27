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
using RemodelHelper.Models;

namespace RemodelHelper.Controls
{
    public class DayOfWeekPresenter : Control
    {
        static DayOfWeekPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DayOfWeekPresenter), new FrameworkPropertyMetadata(typeof(DayOfWeekPresenter)));
        }

        public DayOfWeek DayOfWeek
        {
            get { return (DayOfWeek)this.GetValue(DayOfWeekProperty); }
            set { this.SetValue(DayOfWeekProperty, value); }
        }

        public static readonly DependencyProperty DayOfWeekProperty =
            DependencyProperty.Register(nameof(DayOfWeek), typeof(DayOfWeek), typeof(DayOfWeekPresenter),
                new UIPropertyMetadata(default(DayOfWeek), PropertyChangedCallBack));

        public Week Value
        {
            get { return (Week)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Week), typeof(DayOfWeekPresenter),
                new UIPropertyMetadata(default(Week), PropertyChangedCallBack));

        public DayOfWeek CurrentDay
        {
            get { return (DayOfWeek)this.GetValue(CurrentDayProperty); }
            set { this.SetValue(CurrentDayProperty, value); }
        }

        public static readonly DependencyProperty CurrentDayProperty =
            DependencyProperty.Register(nameof(CurrentDay), typeof(DayOfWeek), typeof(DayOfWeekPresenter),
                new UIPropertyMetadata(default(DayOfWeek), PropertyChangedCallBack));

        private static void PropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (DayOfWeekPresenter)d;
            source.Update();
        }

        private void Update()
        {
            var color = this.Value.Contains(this.DayOfWeek)
                ? Color.FromRgb(32, 175, 64)
                : Color.FromRgb(191, 32, 48);
            if (this.DayOfWeek != this.CurrentDay)
            {
                color.A = 0xCC;
            }

            this.Background = new SolidColorBrush(color);
        }
    }
}
