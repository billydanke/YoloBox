using System;
using System.Globalization;
using System.Windows.Data;

namespace YoloBox.Converters
{
    public class WidthToItemWidthConverter : IValueConverter
    {
        // Adjust spacing here if needed (4 = number of items per row)
        private const int ItemsPerRow = 4;
        private const int TotalMargin = 10; // total margin between items

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double actualWidth && actualWidth > 0)
            {
                double width = (actualWidth / ItemsPerRow) - TotalMargin;
                return Math.Max(50, width); // minimum width
            }

            return 100; // fallback width
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
