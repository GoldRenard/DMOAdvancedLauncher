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

namespace AdvancedLauncher.Tools.Interop {

    public enum CopyBitsBehavior {

        /// <summary>
        ///    Do nothing regarding the SWP_NOCOPYBITS flag.
        /// </summary>
        Default,

        /// <summary>
        /// Never copy the bits from the old location to the new location.
        /// </summary>
        /// <remarks>
        /// In our handler for WM_WINDOWPOSCHANGING, we will always set the
        /// SWP_NOCOPYBITS flag.  Windows will send a WM_PAINT message to
        /// the window to paint itself in the new location.
        /// </remarks>
        NeverCopyBits,

        /// <summary>
        /// Always copy the bits from the old location to the new location.
        /// </summary>
        /// <remarks>
        /// In our handler for WM_WINDOWPOSCHANGING, we will always clear the
        /// SWP_NOCOPYBITS flag.  Windows will copy the bits from the old
        /// location to the new location, and only send a WM_PAINT message for
        /// the area that was obscured.
        /// </remarks>
        AlwaysCopyBits,

        /// <summary>
        /// Always copy the bits from the old location to the new location,
        /// but also cause the window to repaint in the new location.
        /// </summary>
        /// <remarks>
        /// In our handler for WM_WINDOWPOSCHANGING, we will always clear the
        /// SWP_NOCOPYBITS flag.  Windows will copy the bits from the old
        /// location to the new location, and only send a WM_PAINT message for
        /// the area that was obscured.  We also manually invalidate the window
        /// in the new location so that it will repaint.
        ///
        /// Flicker is reduced because Windows will quickly copy the bits for
        /// us, and any visual artifacts from the bit copy will be repaired by
        /// repainting the window.
        /// </remarks>
        CopyBitsAndRepaint
    }
}