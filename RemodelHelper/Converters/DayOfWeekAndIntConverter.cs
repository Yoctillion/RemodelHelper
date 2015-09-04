using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemodelHelper.Converters
{
    class DayOfWeekAndIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DayOfWeek)) return -1;
            return (int)((DayOfWeek)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) return -1;
            return (DayOfWeek)value;
        }
    }
}
