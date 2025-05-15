using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YoloBox.Models;
using YoloBox.ViewModels;

namespace YoloBox.Views
{
    /// <summary>
    /// Interaction logic for DataExportWindow.xaml
    /// </summary>
    public partial class DataExportWindow : Window
    {
        public DataExportWindow(ObservableCollection<DataCleaningImage> imageList, string currentFolderPath)
        {
            InitializeComponent();
            DataContext = new DataExportViewModel(this, imageList, currentFolderPath);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}
