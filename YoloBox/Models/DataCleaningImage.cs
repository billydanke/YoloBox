using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace YoloBox.Models
{
    public class DataCleaningImage
    {
        public string ImagePath { get; set; }
        public bool IsIncluded { get; set; }
        public bool IsSeen { get; set; }

        public DataCleaningImage(string imagePath, bool isIncluded = true, bool isSeen = false)
        {
            ImagePath = imagePath;
            IsIncluded = isIncluded;
            IsSeen = isSeen;
        }

        public BitmapImage Image
        {
            get
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(ImagePath);
                bitmap.DecodePixelWidth = 256;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }
    }
}
