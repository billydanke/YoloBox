using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using YoloBox.Classes;
using YoloBox.Models;

namespace YoloBox.ViewModels
{
    public class DataCleaningViewModel
    {
        private readonly Window _dataCleaningWindow;

        public ICommand OpenImageFolderCommand { get; }
        public ICommand ReturnToMainMenuCommand { get; }
        public ICommand ExcludeAllCommand { get; }

        public ICommand ExportImagesCommand { get; }

        public string CurrentFolderPath { get; set; }
        public int TotalImages { get; set; }
        public ObservableCollection<DataCleaningImage> ImageList { get; set; } = new ObservableCollection<DataCleaningImage>();

        public DataCleaningViewModel(Window dataCleaningWindow)
        {
            _dataCleaningWindow = dataCleaningWindow;

            ReturnToMainMenuCommand = new RelayCommand(_ => ReturnToMainMenu());
            OpenImageFolderCommand = new RelayCommand(_ => OpenImageFolder());
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
                ImageList.Add(new DataCleaningImage(imagePath));
            }
        }

        private void ReturnToMainMenu()
        {
            var window = new MainWindow();
            window.Show();
            _dataCleaningWindow.Close();
        }
    }
}
