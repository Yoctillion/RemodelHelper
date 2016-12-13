using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class SlotItemProperty : Control
    {
        static SlotItemProperty()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SlotItemProperty), new FrameworkPropertyMetadata(typeof(SlotItemProperty)));
        }

        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(SlotItemProperty), new UIPropertyMetadata(""));

        public string PropertyName
        {
            get { return (string)this.GetValue(PropertyNameProperty); }
            set { this.SetValue(PropertyNameProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(SlotItemProperty), new UIPropertyMetadata(0, ValueChangedCallback));

        public int Value
        {
            get { return (int)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty OldValueProperty =
            DependencyProperty.Register(nameof(OldValue), typeof(int?), typeof(SlotItemProperty), new UIPropertyMetadata(null, OldValueChangedCallback));

        public int? OldValue
        {
            get { return (int?)this.GetValue(OldValueProperty); }
            set { this.SetValue(OldValueProperty, value); }
        }

        public static readonly DependencyProperty DiffProperty =
            DependencyProperty.Register(nameof(Diff), typeof(int), typeof(SlotItemProperty), new UIPropertyMetadata(0));

        public int Diff
        {
            get { return (int)this.GetValue(DiffProperty); }
            private set { this.SetValue(DiffProperty, value); }
        }

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (SlotItemProperty)d;
            var currentValue = (int)e.NewValue;
            var oldValue = source.OldValue;
            source.UpdateDiff(oldValue, currentValue);
        }

        private static void OldValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (SlotItemProperty) d;
            var oldValue = (int?)e.NewValue;
            var currentValue = source.Value;
            source.UpdateDiff(oldValue, currentValue);
        }

        private void UpdateDiff(int? oldValue, int currentValue)
        {
            this.Diff = (currentValue - oldValue) ?? 0;
        }
    }
}
