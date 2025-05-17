using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace YoloBox.Models
{
    public class LabelImage : INotifyPropertyChanged
    {
        public string ImagePath { get; set; }

        public string ImageName { get => System.IO.Path.GetFileName(ImagePath); }

        private bool _isSeen;
        public bool IsSeen
        {
            get => _isSeen;
            set
            {
                if (_isSeen != value)
                {
                    _isSeen = value;
                    OnPropertyChanged();
                }
            }
        }

        public LabelImage(string imagePath, bool isSeen = false)
        {
            ImagePath = imagePath;
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
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
