using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YoloBox.Classes;
using YoloBox.Views;

namespace YoloBox.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly Window _mainWindow;

        public ICommand OpenDataCleaningCommand { get; }
        public ICommand OpenImageLabelingCommand { get; }
        public ICommand OpenIcons8ReferenceCommand { get; }

        public MainWindowViewModel(Window mainWindow)
        {
            _mainWindow = mainWindow;

            OpenDataCleaningCommand = new RelayCommand(_ => OpenDataCleaningWindow());
            OpenImageLabelingCommand = new RelayCommand(_ => OpenImageLabelingWindow());
            OpenIcons8ReferenceCommand = new RelayCommand(param => OpenIcons8Reference(param?.ToString()));
        }

        private void OpenDataCleaningWindow()
        {
            var window = new DataCleaningWindow();
            window.Show();
            _mainWindow.Close();
        }

        private void OpenImageLabelingWindow()
        {
            var window = new ImageLabelingWindow();
            window.Show();
            _mainWindow.Close();
        }

        private void OpenIcons8Reference(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
