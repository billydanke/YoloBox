using System;
using System.Globalization;
using System.Windows.Data;

namespace YoloBox.Converters
{
    public class DragHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) return 0.0;

            if (values[0] is double y1 && values[1] is double y2)
            {
                return Math.Abs(y2 - y1);
            }

            return 0.0;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
