using System;
using System.Globalization;
using System.Windows.Data;

namespace YoloBox.Converters
{
    public class DragWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) return 0.0;

            if (values[0] is double x1 && values[1] is double x2)
            {
                return Math.Abs(x2 - x1);
            }

            return 0.0;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
