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
    public class WeekPresenter : Control
    {
        static WeekPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WeekPresenter), new FrameworkPropertyMetadata(typeof(WeekPresenter)));
        }

        public Week Value
        {
            get { return (Week)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Week), typeof(WeekPresenter), new UIPropertyMetadata(default(Week)));

        public DayOfWeek CurrentDay
        {
            get { return (DayOfWeek)this.GetValue(CurrentDayProperty); }
            set { this.SetValue(CurrentDayProperty, value); }
        }

        public static readonly DependencyProperty CurrentDayProperty =
            DependencyProperty.Register(nameof(CurrentDay), typeof(DayOfWeek), typeof(WeekPresenter), new UIPropertyMetadata(default(DayOfWeek)));

        public Thickness CellBorderThickness
        {
            get { return (Thickness)this.GetValue(CellBorderThicknessProperty); }
            set { this.SetValue(CellBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty CellBorderThicknessProperty =
            DependencyProperty.Register(nameof(CellBorderThickness), typeof(Thickness), typeof(WeekPresenter));

        public Brush CellBorderBrush
        {
            get { return (Brush)this.GetValue(CellBorderBrushProperty); }
            set { this.SetValue(CellBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty CellBorderBrushProperty =
            DependencyProperty.Register(nameof(CellBorderBrush), typeof(Brush), typeof(WeekPresenter));

    }
}
