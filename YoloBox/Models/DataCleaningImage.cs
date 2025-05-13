using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using YoloBox.Classes;

namespace YoloBox.Models
{
    public class DataCleaningImage : INotifyPropertyChanged
    {
        public string ImagePath { get; set; }

        private bool _isIncluded;
        public bool IsIncluded
        {
            get => _isIncluded;
            set
            {
                if (_isIncluded != value)
                {
                    _isIncluded = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSeen;
        public bool IsSeen
        {
            get => _isSeen;
            set
            {
                if(_isSeen != value)
                {
                    _isSeen = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ToggleIncludedCommand { get; }

        public DataCleaningImage(string imagePath, bool isIncluded = true, bool isSeen = false)
        {
            ImagePath = imagePath;
            IsIncluded = isIncluded;
            IsSeen = isSeen;

            ToggleIncludedCommand = new RelayCommand(_ => IsIncluded = !IsIncluded);
        }

        public BitmapImage Image
        {
            get
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(ImagePath);
                bitmap.DecodePixelWidth = 640;
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
