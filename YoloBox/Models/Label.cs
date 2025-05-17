using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YoloBox.Models
{
    public class Label
    {
        public Class Class { get; set; }
        public int ClassNumber => Class?.ClassId ?? -1;
        public string ClassName => Class?.Name ?? $"Class {ClassNumber}";
        public Brush ClassBrush => Class?.Brush ?? Brushes.White;

        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Label(Class classModel, double centerX, double centerY, double width, double height)
        {
            Class = classModel;
            CenterX = centerX;
            CenterY = centerY;
            Width = width;
            Height = height;
        }
    }
}
