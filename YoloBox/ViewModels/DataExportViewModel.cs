using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoloBox.Classes;
using YoloBox.Models;

namespace YoloBox.ViewModels
{
    public class DataExportViewModel : INotifyPropertyChanged
    {
        private readonly Window _dataExportWindow;
        private CancellationTokenSource _exportCancellationTokenSource;

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

        private bool _doGenerateFolderStructure = true;
        public bool DoGenerateFolderStructure
        {
            get => _doGenerateFolderStructure;
            set
            {
                if(_doGenerateFolderStructure != value)
                {
                    _doGenerateFolderStructure = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isNonZeroStartingIndex;
        public bool IsNonZeroStartingIndex
        {
            get => _isNonZeroStartingIndex;
            set
            {
                if (_isNonZeroStartingIndex != value)
                {
                    _isNonZeroStartingIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isExporting;
        public bool IsExporting
        {
            get => _isExporting;
            set
            {
                if (_isExporting != value)
                {
                    _isExporting = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _exportedImagesCount;
        public int ExportedImagesCount
        {
            get => _exportedImagesCount;
            set
            {
                if(_exportedImagesCount != value)
                {
                    _exportedImagesCount = value;
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

            ChangeExportPathCommand = new RelayCommand(_ => ChangeExportPath());
            CancelCommand = new RelayCommand(_ => CancelExport());
            ExportCommand = new RelayCommand(_ => ExportImages());
        }

        private void ChangeExportPath()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.DefaultDirectory = CurrentFolderPath;
            dialog.Title = "Set Export Folder";

            if(dialog.ShowDialog() == true)
            {
                ExportFolderPath = dialog.FolderName;
            }
            else
            {
                MessageBox.Show("Unable to set export folder! Please select a valid folder path.", "Invalid Folder Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void CancelExport()
        {
            _exportCancellationTokenSource?.Cancel();
            _dataExportWindow.Close();
        }

        private async Task ExportImages()
        {
            if (string.IsNullOrEmpty(ExportFolderPath)) {
                MessageBox.Show("Unable to export images! No folder path set. Please select an export path and try again.", "Export Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if(!Directory.Exists(ExportFolderPath))
            {
                MessageBoxResult res = MessageBox.Show("Set export directory has not yet been created. Would you like to create it now?", "Image Export", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(res == MessageBoxResult.No) { return; }
                Directory.CreateDirectory(ExportFolderPath);
            }

            string imageOutputPath = ExportFolderPath;
            if(DoGenerateFolderStructure)
            {
                Directory.CreateDirectory(Path.Combine(ExportFolderPath, "images"));
                Directory.CreateDirectory(Path.Combine(ExportFolderPath, "labels"));
                imageOutputPath = Path.Combine(ExportFolderPath, "images");
            }

            IsExporting = true;
            ExportedImagesCount = 0;

            _exportCancellationTokenSource = new CancellationTokenSource();
            var token = _exportCancellationTokenSource.Token;
            bool isCancelled = false;

            try
            {
                await Task.Run(() =>
                {
                    int index = StartingImageIndex;
                    foreach (DataCleaningImage image in ImageList)
                    {
                        if (token.IsCancellationRequested)
                        {
                            isCancelled = true;
                            break;
                        }

                        try
                        {
                            string fileName = $"{index}{Path.GetExtension(image.ImagePath)}";
                            string destination = Path.Combine(imageOutputPath, fileName);

                            File.Copy(image.ImagePath, destination, true);
                            _dataExportWindow.Dispatcher.Invoke(() => ExportedImagesCount++);
                        }
                        catch (Exception ex)
                        {
                            // If something goes wrong, ideally this will throw some error to the user, but for now I will leave it unemplemented.
                            // TODO: display an error to the user if something fails here.
                            Debug.WriteLine($"Failed to copy {image.ImagePath}: {ex.Message}");
                        }
                        index++;
                    }
                });
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Export cancelled.", "Export Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error occured: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsExporting = false;
                _exportCancellationTokenSource = null;
            }

            if(!token.IsCancellationRequested && !isCancelled)
            {
                MessageBox.Show("Export completed!", "Image Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
