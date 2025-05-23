using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace YoloBox.Converters
{
    public class KeyToStringConverter : IValueConverter
    {
        private static readonly Dictionary<Key, string> SpecialMappings = new()
        {
            { Key.D0, "0" },
            { Key.D1, "1" },
            { Key.D2, "2" },
            { Key.D3, "3" },
            { Key.D4, "4" },
            { Key.D5, "5" },
            { Key.D6, "6" },
            { Key.D7, "7" },
            { Key.D8, "8" },
            { Key.D9, "9" },

            { Key.NumPad0, "Numpad 0" },
            { Key.NumPad1, "Numpad 1" },
            { Key.NumPad2, "Numpad 2" },
            { Key.NumPad3, "Numpad 3" },
            { Key.NumPad4, "Numpad 4" },
            { Key.NumPad5, "Numpad 5" },
            { Key.NumPad6, "Numpad 6" },
            { Key.NumPad7, "Numpad 7" },
            { Key.NumPad8, "Numpad 8" },
            { Key.NumPad9, "Numpad 9" },
        };

        public static string CreateFormattedString(Key key)
        {
            if (SpecialMappings.TryGetValue(key, out string mapped))
                return mapped;

            return key.ToString();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Key key ? CreateFormattedString(key) : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (SpecialMappings.ContainsValue(str))
                {
                    return SpecialMappings.First(kvp => kvp.Value == str).Key;
                }

                if (Enum.TryParse<Key>(str, out Key result))
                    return result;
            }

            return Key.None;
        }
    }
}
