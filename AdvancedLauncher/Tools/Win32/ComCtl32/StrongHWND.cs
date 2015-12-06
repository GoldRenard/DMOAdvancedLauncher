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
using AdvancedLauncher.Tools.Win32.User32;

namespace AdvancedLauncher.Tools.Win32.ComCtl32 {

    /// <summary>
    ///     A SafeHandle representing an HWND with strong ownership semantics.
    /// </summary>
    /// <remarks>
    ///     This class is located in the ComCtl32 library because of its
    ///     dependency on WindowSubclass.
    /// </remarks>
    public class StrongHWND : HWND {

        public static StrongHWND CreateWindowEx(WS_EX dwExStyle, string lpClassName, string lpWindowName, WS dwStyle, int x, int y, int nWidth, int nHeight, HWND hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam) {
            HWND hwnd = NativeMethods.CreateWindowEx(dwExStyle, lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);

            return new StrongHWND(hwnd.DangerousGetHandle());
        }

        public StrongHWND()
            : base(true) {
            throw new InvalidOperationException("I need the HWND!");
        }

        public StrongHWND(IntPtr hwnd)
            : base(hwnd, ownsHandle: true) {
            _subclass = new StrongHWNDSubclass(this);
        }

        protected override bool ReleaseHandle() {
            _subclass.Dispose();
            return true;
        }

        // Called from StrongHWNDSubclass
        protected internal virtual void OnHandleReleased() {
            handle = IntPtr.Zero;
        }

        private StrongHWNDSubclass _subclass;
    }
}