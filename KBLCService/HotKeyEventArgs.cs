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