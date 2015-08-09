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
using System.Windows.Controls;
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.UI {

    /// <summary>
    /// <see cref="ControlContainer"/> implementation for <see cref="IWindowManager"/>'s windows.
    /// </summary>
    /// <seealso cref="IRemoteControl"/>
    /// <seealso cref="ControlContainer"/>
    /// <seealso cref="PageContainer"/>
    public class WindowContainer : ControlContainer {

        /// <summary>
        /// Gets <see cref="IWindowManager"/> API
        /// </summary>
        protected readonly IWindowManager WindowManager;

        /// <summary>
        /// Initializes a new instance of <see cref="ControlContainer"/> for specified <see cref="Control"/>
        /// and <see cref="IWindowManager"/>.
        /// </summary>
        /// <param name="Control">Control</param>
        /// <param name="WindowManager">WindowManager API</param>
        public WindowContainer(Control Control, IWindowManager WindowManager) : base(Control) {
            if (WindowManager == null) {
                throw new ArgumentException("WindowManager cannot be null");
            }
            this.WindowManager = WindowManager;
        }

        /// <summary>
        /// Window show handler
        /// </summary>
        public override void OnShow() {
            AbstractWindowControl windowControl = this.Control as AbstractWindowControl;
            if (windowControl != null) {
                windowControl.OnShow();
            }
        }

        /// <summary>
        /// Window close handler
        /// </summary>
        public override void OnClose() {
            WindowManager.GoBack(this);
        }
    }
}