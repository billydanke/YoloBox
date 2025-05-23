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
using System.Windows.Media;
using YoloBox.Classes;
using YoloBox.Models;
using YoloBox.Views;

namespace YoloBox.ViewModels
{
    public class AddClassViewModel : INotifyPropertyChanged
    {
        private readonly AddClassWindow _addClassWindow;

        public ICommand SaveClassesCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ResetHotKeyCommand { get; }

        private string _windowTitle = "Add Class";
        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ClassesFilePath { get; set; }

        public ObservableCollection<Class> Classes { get; set; } = new ObservableCollection<Class>();

        public int ClassId { get; set; } = -1;

        private string _className;
        public string ClassName
        {
            get => _className;
            set
            {
                if (_className != value)
                {
                    _className = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color _color = Colors.White;
        public Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(R));
                    OnPropertyChanged(nameof(G));
                    OnPropertyChanged(nameof(B));
                    OnPropertyChanged(nameof(Brush));
                }
            }
        }
        public byte R
        {
            get => Color.R;
            set => Color = Color.FromRgb(value, Color.G, Color.B);
        }
        public byte G
        {
            get => Color.G;
            set => Color = Color.FromRgb(Color.R, value, Color.B);
        }
        public byte B
        {
            get => Color.B;
            set => Color = Color.FromRgb(Color.R, Color.G, value);
        }
        public SolidColorBrush Brush => new SolidColorBrush(Color);

        private Key _hotkey;
        public Key HotKey
        {
            get => _hotkey;
            set
            {
                if (_hotkey != value)
                {

                    if(value != Key.None)
                    {
                        bool isInUse = Classes.Any(c => c.ClassId != ClassId && c.HotKey == value) == true;

                        if (isInUse)
                        {
                            MessageBox.Show($"The key '{value}' is already assigned to another class! Please try again with a different hotkey.", "Duplicate Key", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    
                    _hotkey = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasHotkey));
                }
            }
        }

        public bool HasHotkey => HotKey == Key.None ? false : true;

        public IEnumerable<Key> AllowedKeys => new[] { Key.None }.Concat(KeyManager.AllowedKeys);

        public AddClassViewModel(AddClassWindow addClassWindow, string labelFolderPath, ObservableCollection<Class> classes, int classId = -1)
        {
            _addClassWindow = addClassWindow;
            ClassesFilePath = Path.Join(labelFolderPath, "Classes.txt");
            Classes = classes;
            
            if(classId == -1) 
            {
                ClassId = Classes.Count;
                Color = Utils.GetRandomColor();
            }
            else
            {
                ClassId = classId;
                Class existingClass = Classes.First(x => x.ClassId == ClassId);
                WindowTitle = $"Edit Class - {existingClass.Name}";
                Color = existingClass.Color;
                ClassName = existingClass.Name;
                HotKey = existingClass.HotKey;
            }

            SaveClassesCommand = new RelayCommand(_ => SaveClass());
            CancelCommand = new RelayCommand(_ => _addClassWindow.Close());
            ResetHotKeyCommand = new RelayCommand(_ => ResetHotKey());
        }

        private void SaveClass()
        {
            Class? existingClass = Classes.FirstOrDefault(x => x.ClassId == ClassId);
            if (existingClass != null)
            {
                existingClass.Name = ClassName;
                existingClass.Color = Color;
                existingClass.HotKey = HotKey;
            }
            else
            {
                Classes.Add(new Class(ClassId, ClassName, Color, HotKey));
            }

            RewriteClassesFile();
            _addClassWindow.Close();
        }

        private void RewriteClassesFile()
        {
            if (Classes == null || !Classes.Any() || string.IsNullOrWhiteSpace(ClassesFilePath))
                return;

            IOrderedEnumerable<Class> orderedClasses = Classes.OrderBy(c => c.ClassId);

            IEnumerable<string> lines = orderedClasses.Select(c => c.Name);

            File.WriteAllLines(ClassesFilePath, lines);
        }

        private void ResetHotKey()
        {
            HotKey = Key.None;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
