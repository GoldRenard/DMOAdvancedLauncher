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

namespace AdvancedLauncher.Tools.Win32.ComCtl32 {

    // It would be great to use the HWND type for hwnd, but this is not
    // possible because you will get a MarshalDirectiveException complaining
    // that the unmanaged code cannot pass in a SafeHandle.  This is because
    // native code is calling this callback, and native code doesn't know how
    // to create a managed SafeHandle for the native handle.  I was a little
    // surprised that the marshaller can't do this automatically, but
    // apparently it can't.
    //
    // Instead, most classes that use a SUBCLASSPROC will expose their own
    // virtual that creates new HWND instances for the incomming handles.
    public delegate IntPtr SUBCLASSPROC(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, IntPtr id, IntPtr data);
}