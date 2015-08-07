using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.DwayneNeed.Win32.ComCtl32;
using Microsoft.DwayneNeed.Win32.Gdi32;
using Microsoft.DwayneNeed.Win32.Kernel32;
using Microsoft.DwayneNeed.Win32.User32;

namespace Microsoft.DwayneNeed.Win32 {

    /// <summary>
    ///     This class contains the PInvoke signatures for functions in
    ///     ComCtl32.dll.
    /// </summary>
    public static class NativeMethods {

        [DllImport("comctl32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetWindowSubclass(HWND hWnd, SUBCLASSPROC pfnSubclass, IntPtr uIdSubclass, ref IntPtr pdwRefData);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetWindowSubclass(HWND hwnd, SUBCLASSPROC callback, IntPtr id, IntPtr data);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RemoveWindowSubclass(HWND hwnd, SUBCLASSPROC callback, IntPtr id);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr DefSubclassProc(HWND hwnd, WM msg, IntPtr wParam, IntPtr lParam);

        [DllImport("dwmapi.dll", CharSet = CharSet.Auto, PreserveSig = true)]
        [return: MarshalAs(UnmanagedType.Error)]
        public static extern int DwmIsCompositionEnabled(out bool pfEnabled);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GdiFlush();

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        // TODO: HDC
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi, DIB iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetStockObject(int stockObject);

        // TODO: HDC
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, ROP dwRop);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string modName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryProcessCycleTime(IntPtr processHandle, ref Int64 processCycleTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryIdleProcessorCycleTime(ref int bufferLength, Int64[] processorIdleCycleTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentProcessId();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentThreadId();

        // TODO: HANDLE
        // TODO: SECURITY_ATTRIBUTES
        [DllImport("kernel32.dll", EntryPoint = "CreateFileMapping", SetLastError = true)]
        private static extern IntPtr _CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, int flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        public static IntPtr CreateFileMapping(PAGE pageProtect, SEC secProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName) {
            // This is a messy function to wrap.
            // GetLastError returns ERROR_ALREADY_EXISTS if the mapping already exists, and the handle is returned.
            // not all PAGE and SEC combos are legit

            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1); // SafeFileHandle.Invalid?
            return _CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (int)pageProtect | (int)secProtect, dwMaximumSizeHigh, dwMaximumSizeLow, lpName);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        // TODO: HANDLE
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// Multiplies two 32-bit values and then divides the 64-bit result by
        /// a third 32-bit value. The final result is rounded to the nearest
        /// integer.
        /// </summary>
        /// <param name="nNumber">
        /// The multiplicand.
        /// </param>
        /// <param name="nNumerator">
        /// The multiplier.
        /// </param>
        /// <param name="nDenominator">
        /// The number by which the result of the multiplication operation is
        /// to be divided.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the result of the
        /// multiplication and division, rounded to the nearest integer. If
        /// the result is a positive half integer (ends in .5), it is rounded
        /// up. If the result is a negative half integer, it is rounded down.
        /// If either an overflow occurred or nDenominator was 0, the return
        /// value is -1.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);

        #region Window Class Functions

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetClassInfoEx(IntPtr hinst, string lpszClass, ref WNDCLASSEX lpwcx);

        public static int GetClassName(HWND hwnd, StringBuilder lpClassName) {
            return _GetClassName(hwnd, lpClassName, lpClassName.Capacity);
        }

        [DllImport("user32.dll", EntryPoint = "GetClassName", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int _GetClassName(HWND hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowLong(HWND hwnd, GWL nIndex);

        public static IntPtr GetWindowLongPtr(HWND hwnd, GWL nIndex) {
            if (IntPtr.Size == 4) {
                // The SetWindowLongPtr entrypoint may not exist on 32-bit
                // OSes, so use the legacy SetWindowLong function instead.
                return new IntPtr(GetWindowLong(hwnd, nIndex));
            } else {
                return _GetWindowLongPtr(hwnd, nIndex);
            }
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr _GetWindowLongPtr(HWND hwnd, GWL nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern short RegisterClassEx([In] ref WNDCLASSEX wcex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetWindowLong(HWND hwnd, GWL nIndex, int dwNewLong);

        public static IntPtr SetWindowLongPtr(HWND hwnd, GWL nIndex, IntPtr dwNewLong) {
            if (IntPtr.Size == 4) {
                // The SetWindowLongPtr entrypoint may not exist on 32-bit
                // OSes, so use the legacy SetWindowLong function instead.
                return new IntPtr(SetWindowLong(hwnd, nIndex, dwNewLong.ToInt32()));
            } else {
                return _SetWindowLongPtr(hwnd, nIndex, dwNewLong);
            }
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr _SetWindowLongPtr(HWND hwnd, GWL nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

        #endregion Window Class Functions

        #region Message Functions

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern WM RegisterWindowMessage(string messageName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(HWND hwnd, WM msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr PostMessage(HWND hwnd, WM msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetMessagePos();

        #endregion Message Functions

        #region Window Functions

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DestroyWindow(HWND hwnd);

        // TODO: CreateChildWindow, CreateTopLevelWindow... note hwndParent and hmenu mean different things.
        // TODO: HMODULE
        // TODO: HMENU
        // TODO: object lparam
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HWND CreateWindowEx(WS_EX dwExStyle, string lpClassName, string lpWindowName, WS dwStyle, int x, int y, int nWidth, int nHeight, HWND hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        // TODO: COLORREF
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(HWND hwnd, uint crKey, byte bAlpha, LWA dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ShowWindow(HWND hWnd, SW nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(HWND hWnd, out int processId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HWND GetParent(HWND hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HWND GetAncestor(HWND hwnd, GA gaFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetWindowRect(HWND hwnd, ref RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetClientRect(HWND hwnd, ref RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HWND GetDesktopWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool AdjustWindowRectEx(ref RECT lpRect, WS style, bool bMenu, WS_EX exStyle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HWND SetParent(HWND hwndChild, HWND hwndNewParent);

        #endregion Window Functions

        #region Window Procedure Functions

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallWindowProc(WNDPROC lpPrevWndFunc, HWND hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr DefWindowProc(HWND hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool IsChild(HWND hWndParent, HWND hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetWindowPos(HWND hwnd, HWND hWndInsertAfter, int X, int Y, int cx, int cy, SWP uFlags);

        #endregion Window Procedure Functions

        #region Painting and Drawing Functions

        // TODO: optional RECT
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RedrawWindow(HWND hwnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RDW flags);

        #endregion Painting and Drawing Functions

        #region Coordinate Space and Transformation Functions

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ScreenToClient(HWND hwnd, ref POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ClientToScreen(HWND hwnd, ref POINT lpPoint);

        #endregion Coordinate Space and Transformation Functions

        #region Keyboard Input Functions

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HWND GetFocus();

        #endregion Keyboard Input Functions

        #region Device Context Functions

        //TODO: HDC
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetDC(HWND hWnd);

        //TODO: HDC
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ReleaseDC(HWND hWnd, IntPtr hDC);

        #endregion Device Context Functions

        #region Mouse Input Functions

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HWND GetCapture();

        #endregion Mouse Input Functions
    }
}