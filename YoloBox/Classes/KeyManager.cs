using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace YoloBox.Classes
{
    public static class KeyManager
    {
        public static readonly HashSet<Key> AllowedKeys = new HashSet<Key>
        {
            Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0, Key.OemMinus, Key.OemPlus,
            Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9, Key.NumPad0,
            Key.B, Key.C, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J, Key.K, Key.L, Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R, Key.T, Key.U, Key.V, Key.X, Key.Y, Key.Z,
            Key.OemOpenBrackets, Key.OemCloseBrackets, Key.OemBackslash,
        };

        public static Key DeselectKey = Key.Escape;
    }
}
