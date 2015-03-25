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
    /// Класс для регистрации хоткеев в системе
    /// </summary>
    public sealed class HotkeyHook : IDisposable {

        /// <summary>
        /// Окно для обработки оконных сообщений
        /// </summary>
        private class HookWindow : NativeWindow, IDisposable {
            private static int WM_HOTKEY = 0x0312;

            public event EventHandler<HotKeyEventArgs> KeyPressed;

            protected override void WndProc(ref Message m) {
                base.WndProc(ref m);
                if (m.Msg == WM_HOTKEY) {
                    // получаем клавиши
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);
                    // посылаем ивент
                    if (KeyPressed != null) {
                        KeyPressed(this, new HotKeyEventArgs(modifier, key));
                    }
                }
            }

            #region IDisposable Members

            public void Dispose() {
                this.DestroyHandle();
            }

            #endregion IDisposable Members
        }

        private HookWindow _window = new HookWindow();
        private int _currentId;

        public event EventHandler<HotKeyEventArgs> KeyPressed;

        public HotkeyHook() {
            _window = new HookWindow();
            _window.CreateHandle(new CreateParams());
            _window.KeyPressed += (sender, args) => {
                if (KeyPressed != null) {
                    KeyPressed(this, args);
                }
            };
        }

        /// <summary>
        /// Регистрация хоткея в системе
        /// </summary>
        /// <param name="modifier">Модификатор хоткея.</param>
        /// <param name="key">Клавиша хоткея.</param>
        public void RegisterHotKey(ModifierKeys modifier, Keys key) {
            _currentId = _currentId + 1;
            if (!NativeMethods.RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key)) {
                throw new InvalidOperationException("Couldn’t register the hot key.");
            }
        }

        /// <summary>
        /// Дерегистрация всех хоткеев
        /// </summary>
        public void UnregisterHotKeys() {
            for (int i = _currentId; i > 0; i--) {
                NativeMethods.UnregisterHotKey(_window.Handle, i);
            }
        }

        #region IDisposable Members

        public void Dispose() {
            UnregisterHotKeys();
            _window.Dispose();
        }

        #endregion IDisposable Members
    }
}