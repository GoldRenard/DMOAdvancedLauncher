// ======================================================================
// DMO KEYBOARD LAYOUT CHANGER SERVICE
// Copyright (C) 2015 Ilya Egorov (goldrenard@gmail.com)

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// ======================================================================

using System;
using System.Windows.Forms;

namespace KBLCService {

    /// <summary>
    /// Перечисление возможных модификаторов
    /// </summary>
    [Flags]
    public enum ModifierKeys : uint {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }

    /// <summary>
    /// Аргументы ивента нажатия хоткея
    /// </summary>
    public class HotKeyEventArgs : EventArgs {
        private ModifierKeys _modifier;
        private Keys _key;

        internal HotKeyEventArgs(ModifierKeys modifier, Keys key) {
            _modifier = modifier;
            _key = key;
        }

        public ModifierKeys Modifier {
            get {
                return _modifier;
            }
        }

        public Keys Key {
            get {
                return _key;
            }
        }

        /// <summary>
        /// Проверяет, является ли хоткей разновидностью Ctrl + Shift
        /// </summary>
        /// <returns>True если является</returns>
        public bool IsControlHotkey {
            get {
                if (this.Key == (System.Windows.Forms.Keys.LButton | System.Windows.Forms.Keys.ShiftKey) && this.Modifier == ModifierKeys.Shift) {
                    return true;
                }
                if (this.Key == System.Windows.Forms.Keys.ShiftKey && this.Modifier == ModifierKeys.Control) {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Проверяет, является ли хоткей разновидностью Alt + Shift
        /// </summary>
        /// <returns>True если является</returns>
        public bool IsAltHotkey {
            get {
                if (this.Key == (System.Windows.Forms.Keys.RButton | System.Windows.Forms.Keys.ShiftKey) && this.Modifier == ModifierKeys.Shift) {
                    return true;
                }
                if (this.Key == System.Windows.Forms.Keys.ShiftKey && this.Modifier == ModifierKeys.Alt) {
                    return true;
                }
                return false;
            }
        }
    }
}