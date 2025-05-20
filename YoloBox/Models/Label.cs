using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YoloBox.Models
{
    public class Label : INotifyPropertyChanged
    {
        public Class Class { get; set; }
        public int ClassNumber => Class?.ClassId ?? -1;
        public string ClassName => Class?.Name ?? $"Class {ClassNumber}";
        public Brush ClassBrush => Class?.Brush ?? Brushes.White;
        public Color ClassColor => Class?.Color ?? Colors.White;
        public Brush SemiTransparentBrush => new SolidColorBrush(Color.FromArgb(50, ClassColor.R, ClassColor.G, ClassColor.B));

        private double _centerX;
        public double CenterX
        {
            get => _centerX;
            set
            {
                if (_centerX != value)
                {
                    _centerX = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _centerY;
        public double CenterY
        {
            get => _centerY;
            set
            {
                if (_centerY != value)
                {
                    _centerY = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                if(_height != value)
                {
                    _height = value;
                    OnPropertyChanged();
                }
            }
        }

        public Label(Class classModel, double centerX, double centerY, double width, double height)
        {
            Class = classModel;
            CenterX = centerX;
            CenterY = centerY;
            Width = width;
            Height = height;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
