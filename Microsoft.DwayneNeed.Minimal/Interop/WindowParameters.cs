﻿using System;
using System.Windows;
using Microsoft.DwayneNeed.Win32.User32;

namespace Microsoft.DwayneNeed.Interop {

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