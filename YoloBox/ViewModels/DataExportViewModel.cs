using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoloBox.Models;

namespace YoloBox.ViewModels
{
    public class DataExportViewModel : INotifyPropertyChanged
    {
        private readonly Window _dataExportWindow;

        public ICommand ChangeExportPathCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ExportCommand { get; }

        public string CurrentFolderPath { get; set; }

        private string _exportFolderPath;
        public string ExportFolderPath
        {
            get => _exportFolderPath;
            set
            {
                if (_exportFolderPath != value)
                {
                    _exportFolderPath = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalImages;
        public int TotalImages
        {
            get => _totalImages;
            set
            {
                if (_totalImages != value)
                {
                    _totalImages = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _startingImageIndex = 0;
        public int StartingImageIndex
        {
            get => _startingImageIndex;
            set
            {
                if (_startingImageIndex != value)
                {
                    _startingImageIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DataCleaningImage> ImageList { get; set; } = new ObservableCollection<DataCleaningImage>();

        public DataExportViewModel(Window dataExportWindow, ObservableCollection<DataCleaningImage> imageList, string currentFolderPath)
        {
            _dataExportWindow = dataExportWindow;
            CurrentFolderPath = currentFolderPath;

            ImageList = [.. imageList.Where(x => x.IsIncluded)];
            TotalImages = ImageList.Count;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
