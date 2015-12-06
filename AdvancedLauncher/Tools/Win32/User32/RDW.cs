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

namespace AdvancedLauncher.Tools.Win32.User32 {

    public enum RDW : int {
        INVALIDATE = 0x0001,
        INTERNALPAINT = 0x0002,
        ERASE = 0x0004,
        VALIDATE = 0x0008,
        NOINTERNALPAINT = 0x0010,
        NOERASE = 0x0020,
        NOCHILDREN = 0x0040,
        ALLCHILDREN = 0x0080,
        UPDATENOW = 0x0100,
        ERASENOW = 0x0200,
        FRAME = 0x0400,
        NOFRAME = 0x0800
    }
}