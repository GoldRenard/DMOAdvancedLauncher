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

namespace AdvancedLauncher.Tools {

    public static class NativeMacros {

        public static ushort HIWORD(uint dword) {
            return (ushort)((dword >> 16) & 0xFFFF);
        }

        public static ushort LOWORD(uint dword) {
            return (ushort)dword;
        }

        public static int GET_X_LPARAM(uint dword) {
            return unchecked((int)(short)LOWORD(dword));
        }

        public static int GET_Y_LPARAM(uint dword) {
            return unchecked((int)(short)HIWORD(dword));
        }
    }
}