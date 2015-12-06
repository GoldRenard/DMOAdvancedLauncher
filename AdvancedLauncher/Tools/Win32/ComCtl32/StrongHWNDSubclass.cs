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

using AdvancedLauncher.Tools.Win32.User32;

namespace AdvancedLauncher.Tools.Win32.ComCtl32 {

    internal class StrongHWNDSubclass : WindowSubclass {

        public StrongHWNDSubclass(StrongHWND strongHwnd) : base(new HWND(strongHwnd.DangerousGetHandle())) {
            // Note that we passed a new "weak" HWND handle to the base class.
            // This is because we don't want the StrongHWNDSubclass processing
            // a partially disposed handle in its own Dispose methods.
            _strongHwnd = strongHwnd;
        }

        protected override void Dispose(bool disposing) {
            // call the base class to let it disconnect the window proc.
            HWND hwnd = Hwnd;
            base.Dispose(disposing);

            NativeMethods.DestroyWindow(hwnd);
            _strongHwnd.OnHandleReleased();
        }

        private StrongHWND _strongHwnd;
    }
}