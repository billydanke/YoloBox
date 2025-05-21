using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Label = YoloBox.Models.Label;

namespace YoloBox.Components
{
    /// <summary>
    /// Interaction logic for ResizableLabelBox.xaml
    /// </summary>
    public partial class ResizableLabelBox : UserControl
    {
        public Label Label => DataContext as Label;

        public static readonly DependencyProperty ImageWidthProperty =
    DependencyProperty.Register(nameof(ImageWidth), typeof(double), typeof(ResizableLabelBox),
        new PropertyMetadata(1.0, OnImageSizeChanged));

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(ResizableLabelBox),
                new PropertyMetadata(1.0, OnImageSizeChanged));

        public double ImageWidth
        {
            get => (double)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        public double ImageHeight
        {
            get => (double)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }

        private static void OnImageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ResizableLabelBox box)
                box.UpdateLayout();
        }

        public ResizableLabelBox()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (Label == null) return;

            Width = Label.Width * ImageWidth;
            Height = Label.Height * ImageHeight;

            Canvas.SetLeft(this, Label.CenterX * ImageWidth - Width / 2);
            Canvas.SetTop(this, Label.CenterY * ImageHeight - Height / 2);
        }

        private void ApplySizeChange(double dx, double dy, bool left, bool top)
        {
            double pixelWidth = Width;
            double pixelHeight = Height;

            double pixelCenterX = Label.CenterX * ImageWidth;
            double pixelCenterY = Label.CenterY * ImageHeight;

            double pixelLeft = pixelCenterX - pixelWidth / 2;
            double pixelTop = pixelCenterY - pixelHeight / 2;

            const double MinSize = 10;

            if (left)
            {
                double newLeft = pixelLeft + dx;
                double maxLeft = pixelLeft + pixelWidth - MinSize;
                newLeft = Math.Min(newLeft, maxLeft); // Prevent overlap
                pixelWidth = pixelLeft + pixelWidth - newLeft;
                pixelLeft = newLeft;
            }
            else
            {
                pixelWidth = Math.Max(MinSize, pixelWidth + dx);
            }

            if (top)
            {
                double newTop = pixelTop + dy;
                double maxTop = pixelTop + pixelHeight - MinSize;
                newTop = Math.Min(newTop, maxTop); // Prevent overlap
                pixelHeight = pixelTop + pixelHeight - newTop;
                pixelTop = newTop;
            }
            else
            {
                pixelHeight = Math.Max(MinSize, pixelHeight + dy);
            }

            pixelLeft = Math.Max(0, pixelLeft);
            pixelTop = Math.Max(0, pixelTop);

            if (pixelLeft + pixelWidth > ImageWidth)
                pixelWidth = ImageWidth - pixelLeft;

            if (pixelTop + pixelHeight > ImageHeight)
                pixelHeight = ImageHeight - pixelTop;

            double newCenterX = (pixelLeft + pixelWidth / 2) / ImageWidth;
            double newCenterY = (pixelTop + pixelHeight / 2) / ImageHeight;

            Label.CenterX = Math.Clamp(newCenterX, 0, 1);
            Label.CenterY = Math.Clamp(newCenterY, 0, 1);
            Label.Width = Math.Clamp(pixelWidth / ImageWidth, 0, 1);
            Label.Height = Math.Clamp(pixelHeight / ImageHeight, 0, 1);

            Width = pixelWidth;
            Height = pixelHeight;
        }


        // Thumb Drag Events
        private void TopLeftThumb_DragDelta(object s, DragDeltaEventArgs e) => ApplySizeChange(e.HorizontalChange, e.VerticalChange, true, true);
        private void TopRightThumb_DragDelta(object s, DragDeltaEventArgs e) => ApplySizeChange(e.HorizontalChange, e.VerticalChange, false, true);
        private void BottomLeftThumb_DragDelta(object s, DragDeltaEventArgs e) => ApplySizeChange(e.HorizontalChange, e.VerticalChange, true, false);
        private void BottomRightThumb_DragDelta(object s, DragDeltaEventArgs e) => ApplySizeChange(e.HorizontalChange, e.VerticalChange, false, false);
        private void LeftThumb_DragDelta(object s, DragDeltaEventArgs e) => ApplySizeChange(e.HorizontalChange, 0, true, false);
        private void RightThumb_DragDelta(object s, DragDeltaEventArgs e) => ApplySizeChange(e.HorizontalChange, 0, false, false);
        private void TopThumb_DragDelta(object s, DragDeltaEventArgs e) => ApplySizeChange(0, e.VerticalChange, false, true);
        private void BottomThumb_DragDelta(object s, DragDeltaEventArgs e) => ApplySizeChange(0, e.VerticalChange, false, false);
    }
}
