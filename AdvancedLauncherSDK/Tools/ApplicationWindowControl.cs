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
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Tools;

namespace AdvancedLauncher.SDK.Management.Windows {

    /// <summary>
    /// WindowsFormsHost Control wrapper. Runs specified program inside this control.
    /// </summary>
    public class ApplicationWindowControl : UserControl, IDisposable {
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int GWL_STYLE = -16;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;

        private readonly int WaitTimeout = 0;

        private readonly ProcessStartInfo StartInfo;

        private Process Process;

        private System.Windows.Forms.Panel Panel;

        /// <summary>
        /// Process exit event handler
        /// </summary>
        public BaseEventHandler Exited;

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationWindowControl"/> for specified <see cref="ProcessStartInfo"/> and wait timeout.
        /// </summary>
        /// <param name="StartInfo">Process start information</param>
        /// <param name="WaitTimeout">Event action (0 by default)</param>
        public ApplicationWindowControl(ProcessStartInfo StartInfo, int WaitTimeout = 0) {
            this.StartInfo = StartInfo;
            this.WaitTimeout = WaitTimeout;
            this.Loaded += OnLoaded;
            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e) {
            ResizeEmbeddedApp();
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
            WindowsFormsHost FormsHost = new WindowsFormsHost();
            Panel = new System.Windows.Forms.Panel();
            FormsHost.Child = Panel;
            AddChild(FormsHost);
            this.Process = new Process();
            this.Process.StartInfo = StartInfo;
            this.Process.Exited += OnProcessExited;
            this.Process.Start();
            //this.Process.StartInfo.CreateNoWindow = true;
            this.Process.EnableRaisingEvents = true;
            this.Process.WaitForInputIdle();
            Thread.Sleep(WaitTimeout);
            NativeMethods.SetParent(Process.MainWindowHandle, Panel.Handle);

            // remove control box
            int style = NativeMethods.GetWindowLong(Process.MainWindowHandle, GWL_STYLE);
            style = style & ~WS_CAPTION & ~WS_THICKFRAME;
            NativeMethods.SetWindowLong(Process.MainWindowHandle, GWL_STYLE, style);

            // resize embedded application & refresh
            ResizeEmbeddedApp();
        }

        private void OnProcessExited(object sender, EventArgs e) {
            if (Exited != null) {
                Exited(sender, BaseEventArgs.Empty);
            }
        }

        private void ResizeEmbeddedApp() {
            if (Process == null) {
                return;
            }
            NativeMethods.SetWindowPos(Process.MainWindowHandle, IntPtr.Zero, 0, 0, (int)Panel.ClientSize.Width, (int)Panel.ClientSize.Height, SWP_NOZORDER | SWP_NOACTIVATE);
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        ///  When overridden in a derived class, releases the unmanaged resources used by
        ///  the <see cref="ApplicationWindowControl"/>, and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    if (Process != null) {
                        Process.Kill();
                        Process.Dispose();
                        Panel.Dispose();
                    }
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="ApplicationWindowControl"/> class.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}