using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using YoloBox.Classes;
using YoloBox.Models;
using YoloBox.Views;

namespace YoloBox.ViewModels
{
    public class ImageLabelingViewModel : INotifyPropertyChanged
    {
        private readonly Window _imageLabelingWindow;

        public ICommand OpenImageFolderCommand { get; }
        public ICommand SetLabelFolderCommand { get; }
        public ICommand ReturnToMainMenuCommand { get; }
        public ICommand AddNewClassCommand { get; }
        public ICommand EditClassCommand { get; }
        public ICommand DeleteLabelCommand { get; }
        public ICommand PreviousImageCommand { get; }
        public ICommand NextImageCommand { get; }

        private string _currentImageFolderPath = "None";
        public string CurrentImageFolderPath
        {
            get => _currentImageFolderPath;
            set
            {
                if(_currentImageFolderPath != value)
                {
                    _currentImageFolderPath = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _currentLabelFolderPath;
        public string CurrentLabelFolderPath
        {
            get => _currentLabelFolderPath;
            set
            {
                if (_currentLabelFolderPath != value)
                {
                    _currentLabelFolderPath = value;
                    OnPropertyChanged();
                }
            }
        }

        private Class _selectedClass;
        public Class SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (_selectedClass != value)
                {
                    _selectedClass = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Class> Classes { get; set; } = new ObservableCollection<Class>();
        public ObservableCollection<Label> Labels { get; set; } = new ObservableCollection<Label>();

        private LabelImage _selectedLabelImage;
        public LabelImage SelectedLabelImage
        {
            get => _selectedLabelImage;
            set
            {
                if (_selectedLabelImage != value)
                {
                    try
                    {
                        if (_selectedLabelImage != null)
                        {
                            SaveLabelsForImage(_selectedLabelImage);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to save labels: {ex.Message}", "Label Saving Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                    _selectedLabelImage = value;

                    try
                    {
                        if (_selectedLabelImage != null)
                        {
                            _selectedLabelImage.IsSeen = true;
                            LoadLabelsForImage(_selectedLabelImage);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load labels: {ex.Message}", "Label Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<LabelImage> ImageList { get; set; } = new ObservableCollection<LabelImage>();

        public ImageLabelingViewModel(Window imageLabelingWindow)
        {
            _imageLabelingWindow = imageLabelingWindow;

            OpenImageFolderCommand = new RelayCommand(_ => OpenImageFolder());
            SetLabelFolderCommand = new RelayCommand(_ => SetLabelFolder());
            ReturnToMainMenuCommand = new RelayCommand(_ => ReturnToMainMenu());
            AddNewClassCommand = new RelayCommand(_ => AddNewClass());
            EditClassCommand = new RelayCommand<Class>(classModel => EditClass(classModel));
            DeleteLabelCommand = new RelayCommand<Label>(label =>
            {
                if (label != null && Labels.Contains(label))
                {
                    Labels.Remove(label);
                }
            });
            NextImageCommand = new RelayCommand(_ => GoToNextImage());
            PreviousImageCommand = new RelayCommand(_ => GoToPreviousImage());
        }

        private void OpenImageFolder()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.Title = "Open Image Folder";

            if(dialog.ShowDialog() == true)
            {
                CurrentImageFolderPath = dialog.FolderName;
                
                string[] imagePaths = Directory.GetFiles(CurrentImageFolderPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(x => x.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    x.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    x.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    x.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)).ToArray();

                ImageList.Clear();
                foreach (string imagePath in imagePaths)
                {
                    LabelImage labelImage = new LabelImage(imagePath);
                    ImageList.Add(labelImage);
                }
            }

            if(string.IsNullOrEmpty(CurrentLabelFolderPath))
            {
                MessageBox.Show("The label folder has not yet been set! Please select a label folder to continue.", "Label Folder Not Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                SetLabelFolder();
            }
            else
            {
                var res = MessageBox.Show("Label folder already selected. Would you like to change it?", "Label Folder Already Selected", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    SetLabelFolder();
                }
            }

            // Load the first image in the list automatically
            if(ImageList.Any())
            {
                SelectedLabelImage = ImageList.First();
            }
        }

        private void SetLabelFolder()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.Title = "Set Label Folder";

            if(dialog.ShowDialog() == true && Directory.Exists(dialog.FolderName))
            {
                CurrentLabelFolderPath = dialog.FolderName;
                LoadClassesFromFile(Path.Combine(CurrentLabelFolderPath, "Classes.txt"));
            }
            else
            {
                MessageBox.Show("Unable to set label folder! Please try again.", "Set Label Folder", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadLabelsForImage(LabelImage image)
        {
            if (image == null) { return; }

            if (string.IsNullOrEmpty(CurrentLabelFolderPath))
            {
                MessageBox.Show("The label folder has not yet been set! Please select a label folder to continue.", "Label Folder Not Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                SetLabelFolder();

                if(string.IsNullOrEmpty(CurrentLabelFolderPath)) { return; }
            }

            Labels.Clear();

            string fileName = Path.GetFileNameWithoutExtension(image.ImagePath) + ".txt";
            string labelPath = Path.Combine(CurrentLabelFolderPath, fileName);

            if (File.Exists(labelPath))
            {
                foreach(var line in File.ReadAllLines(labelPath))
                {
                    string[] parts = line.Split(' ');

                    if (parts.Length == 5 && int.TryParse(parts[0], out int classId) && double.TryParse(parts[1], out double centerX) && double.TryParse(parts[2], out double centerY) && double.TryParse(parts[3], out double width) && double.TryParse(parts[4], out double height))
                    {
                        var classModel = Classes.FirstOrDefault(x => x.ClassId == classId);

                        if(classModel == null)
                        {
                            MessageBoxResult res = MessageBox.Show($"Class ID {classId} is not yet defined in your Classes.txt! Would you like to create it now?", "Unknown Class", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if(res == MessageBoxResult.Yes)
                            {
                                AddNewClass(classId);
                                classModel = Classes.FirstOrDefault(x => x.ClassId == classId);
                            }

                            // If the class does not already exist (and yet is present in an existing label file), and the user does not want to create the class,
                            // It will get a dummy placeholder Class so as to not lose data when the labels get re-saved.
                            if (classModel == null)
                            {
                                classModel = new Class(classId, $"Unknown Class ({classId})", Colors.Gray);
                            }
                        }

                        Labels.Add(new Label(classModel, centerX, centerY, width, height));
                    }
                }
            }
        }

        private void SaveLabelsForImage(LabelImage image)
        {
            if (image == null || !Labels.Any()) { return; }

            if (string.IsNullOrEmpty(CurrentLabelFolderPath))
            {
                MessageBox.Show("The label folder has not yet been set! Please select a label folder to continue.", "Label Folder Not Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                SetLabelFolder();

                if (string.IsNullOrEmpty(CurrentLabelFolderPath)) { return; }
            }

            string fileName = Path.GetFileNameWithoutExtension(image.ImagePath) + ".txt";
            string labelPath = Path.Combine(CurrentLabelFolderPath, fileName);

            IEnumerable<string> lines = Labels.Select(label => $"{label.ClassNumber} {label.CenterX.ToString("0.######", CultureInfo.InvariantCulture)} {label.CenterY.ToString("0.######", CultureInfo.InvariantCulture)} {label.Width.ToString("0.######", CultureInfo.InvariantCulture)} {label.Height.ToString("0.######", CultureInfo.InvariantCulture)}");

            File.WriteAllLines(labelPath, lines);
        }

        private void LoadClassesFromFile(string classesFilePath)
        {
            if (!File.Exists(classesFilePath)) { return; }

            Classes.Clear();

            string[] lines = File.ReadAllLines(classesFilePath);

            for (int i = 0; i < lines.Length; i++)
            {
                string className = lines[i].Trim();

                if (!string.IsNullOrWhiteSpace(className))
                {
                    Class classModel = new Class(i, className, Utils.GetRandomColor());
                    Classes.Add(classModel);
                }
            }
        }

        private void AddNewClass(int classId = -1)
        {
            if(string.IsNullOrEmpty(CurrentLabelFolderPath))
            {
                MessageBox.Show("Unable to add new class, label folder is not set! Please set a label destination folder to continue.", "Failed to Add Class", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new AddClassWindow(CurrentLabelFolderPath, Classes, classId);
            window.ShowDialog();
        }

        private void EditClass(Class? classModel)
        {
            if (classModel != null)
            {
                AddNewClass(classModel.ClassId);
            }
        }

        private void GoToNextImage()
        {
            int index = ImageList.IndexOf(SelectedLabelImage);
            if(index >= 0 && index < ImageList.Count - 1)
            {
                SelectedLabelImage = ImageList[index + 1];
            }
        }

        private void GoToPreviousImage()
        {
            int index = ImageList.IndexOf(SelectedLabelImage);
            if (index > 0)
            {
                SelectedLabelImage = ImageList[index - 1];
            }
        }

        private void ReturnToMainMenu()
        {
            var window = new MainWindow();
            window.Show();
            _imageLabelingWindow.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
