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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.Management.Windows {

    /// <summary>
    /// WindowsFormsHost Control wrapper. Runs specified program inside this control.
    /// </summary>
    public class ApplicationWindowControl : UserControl {

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        private static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int GWL_STYLE = -16;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;

        private readonly int WaitTimeout = 0;

        private readonly ProcessStartInfo StartInfo;

        private Process Process;

        private System.Windows.Forms.Panel Panel;

        public BaseEventHandler Exited;

        public ApplicationWindowControl(ProcessStartInfo StartInfo, int WaitTimeout = 0) {
            this.StartInfo = StartInfo;
            this.WaitTimeout = WaitTimeout;
            WindowsFormsHost FormsHost = new WindowsFormsHost();
            Panel = new System.Windows.Forms.Panel();
            FormsHost.Child = Panel;
            AddChild(FormsHost);
            this.Loaded += OnLoaded;
            this.SizeChanged += OnSizeChanged;
        }

        protected virtual void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e) {
            ResizeEmbeddedApp();
        }

        protected virtual void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
            this.Process = new Process();
            this.Process.StartInfo = StartInfo;
            this.Process.Exited += OnProcessExited;
            this.Process.Start();
            //this.Process.StartInfo.CreateNoWindow = true;
            this.Process.EnableRaisingEvents = true;
            this.Process.WaitForInputIdle();
            Thread.Sleep(WaitTimeout);
            SetParent(Process.MainWindowHandle, Panel.Handle);

            // remove control box
            int style = GetWindowLong(Process.MainWindowHandle, GWL_STYLE);
            style = style & ~WS_CAPTION & ~WS_THICKFRAME;
            SetWindowLong(Process.MainWindowHandle, GWL_STYLE, style);

            // resize embedded application & refresh
            ResizeEmbeddedApp();
        }

        protected virtual void OnProcessExited(object sender, EventArgs e) {
            if (Exited != null) {
                Exited(sender, BaseEventArgs.Empty);
            }
        }

        protected virtual void ResizeEmbeddedApp() {
            if (Process == null) {
                return;
            }
            SetWindowPos(Process.MainWindowHandle, IntPtr.Zero, 0, 0, (int)Panel.ClientSize.Width, (int)Panel.ClientSize.Height, SWP_NOZORDER | SWP_NOACTIVATE);
        }
    }
}