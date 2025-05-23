using System;
using System.Collections.Generic;
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
using YoloBox.ViewModels;
using YoloBox.Classes;

namespace YoloBox.Views
{
    /// <summary>
    /// Interaction logic for ImageLabelingWindow.xaml
    /// </summary>
    public partial class ImageLabelingWindow : Window
    {
        public ImageLabelingWindow()
        {
            InitializeComponent();
            DataContext = new ImageLabelingViewModel(this);
        }

        /// <summary>
        /// This is NO BUENO. I didn't want to have anything in the code-behind, but I don't know how to
        /// dynamically assign keybindings any other way. If this has to be the only code-behind function
        /// in this project I guess I can live with it. :(
        /// </summary>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is ImageLabelingViewModel vm && !Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                // First check if the key is the deselection key
                if(e.Key == KeyManager.DeselectKey)
                {
                    vm.SelectedClass = null;
                    e.Handled = true;
                    return;
                }

                // Check if this key matches any of the classes' HotKeys.
                var matchingClass = vm.Classes.FirstOrDefault(x => x.HotKey == e.Key);

                if (matchingClass != null)
                {
                    vm.SelectedClass = matchingClass;
                    e.Handled = true;
                }
            }
        }
    }
}
