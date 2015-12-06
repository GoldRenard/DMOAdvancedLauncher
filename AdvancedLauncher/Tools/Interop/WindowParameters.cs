// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
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
using System.Windows;
using AdvancedLauncher.Tools.Win32.User32;

namespace AdvancedLauncher.Tools.Interop {

    public class WindowParameters {

        public object Tag {
            get; set;
        }

        public IntPtr HINSTANCE {
            get; set;
        }

        public Int32Rect WindowRect {
            get; set;
        }

        public String Name {
            get; set;
        }

        public WS Style {
            get; set;
        }

        public WS_EX ExtendedStyle {
            get; set;
        }

        public HWND Parent {
            get {
                // never return null
                return _parent ?? HWND.NULL;
            }

            set {
                _parent = value;
            }
        }

        private HWND _parent;
    }
}