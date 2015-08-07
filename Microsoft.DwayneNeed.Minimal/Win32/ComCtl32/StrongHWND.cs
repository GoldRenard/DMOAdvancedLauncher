﻿using System;
using Microsoft.DwayneNeed.Win32.User32;

namespace Microsoft.DwayneNeed.Win32.ComCtl32 {

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