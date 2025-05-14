using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using YoloBox.Classes;
using YoloBox.Models;
using YoloBox.Views;

namespace YoloBox.ViewModels
{
    public class DataCleaningViewModel : INotifyPropertyChanged
    {
        private readonly Window _dataCleaningWindow;

        public ICommand OpenImageFolderCommand { get; }
        public ICommand ReturnToMainMenuCommand { get; }
        public ICommand ExcludeAllCommand { get; }
        public ICommand IncludeAllCommand { get; }
        public ICommand ExportImagesCommand { get; }

        private string _currentFolderPath = "None";
        public string CurrentFolderPath
        {
            get => _currentFolderPath;
            set
            {
                if (_currentFolderPath != value)
                {
                    _currentFolderPath = value;
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

        private int _totalIncludedImages;
        public int TotalIncludedImages
        {
            get => _totalIncludedImages;
            set
            {
                if(_totalIncludedImages != value)
                {
                    _totalIncludedImages = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFolderLoaded;
        public bool IsFolderLoaded
        {
            get => _isFolderLoaded;
            set
            {
                if (_isFolderLoaded != value)
                {
                    _isFolderLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DataCleaningImage> ImageList { get; set; } = new ObservableCollection<DataCleaningImage>();

        public DataCleaningViewModel(Window dataCleaningWindow)
        {
            _dataCleaningWindow = dataCleaningWindow;

            ReturnToMainMenuCommand = new RelayCommand(_ => ReturnToMainMenu());
            OpenImageFolderCommand = new RelayCommand(_ => OpenImageFolder());
            ExcludeAllCommand = new RelayCommand(_ => ExcludeAllImages());
            IncludeAllCommand = new RelayCommand(_ => IncludeAllImages());
            ExportImagesCommand = new RelayCommand(_ => ExportIncudedImages());
        }

        private void OpenImageFolder()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.Title = "Open Image Folder";

            if (dialog.ShowDialog() == true)
            {
                CurrentFolderPath = dialog.FolderName;
            }
            else
            {
                MessageBox.Show("Unable to open folder! Please try again.", "Open Image Folder", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LoadAllImages();
        }

        private void LoadAllImages()
        {
            if (CurrentFolderPath == null || CurrentFolderPath == string.Empty) return;

            string[] imagePaths = Directory.GetFiles(CurrentFolderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || 
                x.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                x.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                x.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)).ToArray();

            ImageList.Clear();
            foreach(string imagePath in imagePaths)
            {
                DataCleaningImage image = new DataCleaningImage(imagePath);
                image.PropertyChanged += DataCleaningImage_PropertyChanged;
                ImageList.Add(image);
            }

            IsFolderLoaded = true;
            TotalImages = ImageList.Count;
            TotalIncludedImages = ImageList.Count;
        }

        private void DataCleaningImage_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataCleaningImage.IsIncluded))
            {
                DataCleaningImage? image = sender as DataCleaningImage;
                if(image != null)
                {
                    if (image.IsIncluded) TotalIncludedImages++;
                    else TotalIncludedImages--;
                }
            }
        }

        private void ExcludeAllImages()
        {
            foreach (DataCleaningImage image in ImageList)
            {
                image.IsIncluded = false;
            }
        }

        private void IncludeAllImages()
        {
            foreach (DataCleaningImage image in ImageList)
            {
                image.IsIncluded = true;
            }
        }

        private void ExportIncudedImages()
        {
            var window = new DataExportWindow(ImageList, CurrentFolderPath);
            window.ShowDialog();
        }

        private void ReturnToMainMenu()
        {
            var window = new MainWindow();
            window.Show();
            _dataCleaningWindow.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
