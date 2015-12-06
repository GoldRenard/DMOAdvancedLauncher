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

using System.Windows;
using System.Windows.Interop;
using AdvancedLauncher.Tools.Win32.User32;

namespace AdvancedLauncher.Tools.Extensions {

    public static class HwndSourceExtensions {

        /// <summary>
        ///     Transform a point from "screen" coordinate space into the
        ///     "client" coordinate space of the window.
        /// </summary>
        public static Point TransformScreenToClient(this HwndSource hwndSource, Point point) {
            HWND hwnd = new HWND(hwndSource.Handle);

            POINT pt = new POINT();
            pt.x = (int)point.X;
            pt.y = (int)point.Y;

            NativeMethods.ScreenToClient(hwnd, ref pt);

            return new Point(pt.x, pt.y);
        }

        /// <summary>
        ///     Transform a point from "client" coordinate space of a window
        ///     into the "screen" coordinate space.
        /// </summary>
        public static Point TransformClientToScreen(this HwndSource hwndSource, Point point) {
            HWND hwnd = new HWND(hwndSource.Handle);

            POINT pt = new POINT();
            pt.x = (int)point.X;
            pt.y = (int)point.Y;

            NativeMethods.ClientToScreen(hwnd, ref pt);

            return new Point(pt.x, pt.y);
        }
    }
}