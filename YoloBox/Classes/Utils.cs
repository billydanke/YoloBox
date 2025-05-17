using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoloBox.Classes
{
    public static class Utils
    {
        public static System.Windows.Media.Color GetRandomColor()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            return System.Windows.Media.Color.FromRgb((byte)rand.Next(40, 255), (byte)rand.Next(40, 255), (byte)rand.Next(40, 255));
        }
    }
}
