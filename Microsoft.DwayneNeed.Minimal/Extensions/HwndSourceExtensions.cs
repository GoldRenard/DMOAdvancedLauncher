using System.Windows;
using System.Windows.Interop;
using Microsoft.DwayneNeed.Win32;
using Microsoft.DwayneNeed.Win32.User32;

namespace Microsoft.DwayneNeed.Extensions {

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