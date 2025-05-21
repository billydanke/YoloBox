using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace YoloBox.Models
{
    public class Class : INotifyPropertyChanged
    {
        private int _classId;
        public int ClassId
        {
            get => _classId;
            set
            {
                if (_classId != value)
                {
                    _classId = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Brush)); // for UI binding convenience
                    OnPropertyChanged(nameof(HoverBrush));
                    OnPropertyChanged(nameof(GlowEffect));
                }
            }
        }

        private Key _hotkey;
        public Key HotKey
        {
            get => _hotkey;
            set
            {
                if (_hotkey != value)
                {
                    _hotkey = value;
                    OnPropertyChanged();
                }
            }
        }

        public SolidColorBrush Brush => new SolidColorBrush(Color);
        public SolidColorBrush HoverBrush => new SolidColorBrush(Color.FromArgb(50, Color.R, Color.G, Color.B));

        public Effect GlowEffect => new DropShadowEffect
        {
            Color = Color,
            BlurRadius = 10,
            ShadowDepth = 0,
            Opacity = 0.25
        };

        public Class(int classId, string name, Color color)
        {
            ClassId = classId;
            Name = name;
            Color = color;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
