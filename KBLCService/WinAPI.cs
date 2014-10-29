// ======================================================================
// DMO KEYBOARD LAYOUT CHANGER SERVICE
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Runtime.InteropServices;
using System.Text;

namespace KBLCService {

    /// <summary>
    /// Функции WinAPI
    /// </summary>
    public static class WinAPI {

        [DllImport("user32.dll")]
        public static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, int uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowText);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentHwnd, IntPtr childAfterHwnd, IntPtr className, string windowText);

        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hwnd, int command);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        public static IntPtr INPUTLANGCHANGE_FORWARD = new IntPtr(0x0002);
        public static IntPtr HKL_NEXT = new IntPtr(1);

        public const string LANG_EN_US = "00000409";
        public const string LANG_RU_RU = "00000419";

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 1;

        public static IntPtr TaskBarHandle {
            get {
                return FindWindow("Shell_TrayWnd", "");
            }
        }

        public static string GetWindowTitle(IntPtr hWnd) {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);

            if (GetWindowText(hWnd, Buff, nChars) > 0) {
                return Buff.ToString();
            }
            return null;
        }
    }
}