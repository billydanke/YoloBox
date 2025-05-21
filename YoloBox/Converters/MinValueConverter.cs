using System;
using System.Globalization;
using System.Windows.Data;

namespace YoloBox.Converters
{
    public class MinValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is double d1) || !(values[1] is double d2))
                return 0.0;

            return Math.Min(d1, d2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
