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

using System.Runtime.InteropServices;

namespace AdvancedLauncher.Tools.Win32.User32 {

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {

        public POINT(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static POINT FromParam(uint param) {
            int x = NativeMacros.GET_X_LPARAM(param);
            int y = NativeMacros.GET_Y_LPARAM(param);

            return new POINT(x, y);
        }

        public uint ToParam() {
            uint param_x = unchecked((ushort)(short)x);
            uint param_y = unchecked((ushort)(short)y);

            return (param_y << 16) | param_x;
        }

        public int x;
        public int y;
    }
}