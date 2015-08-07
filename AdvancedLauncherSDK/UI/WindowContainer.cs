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

    public class WindowContainer : ControlContainer {
        protected readonly IWindowManager WindowManager;

        public WindowContainer(Control Control, IWindowManager WindowManager) : base(Control) {
            if (WindowManager == null) {
                throw new ArgumentException("WindowManager cannot be null");
            }
            this.WindowManager = WindowManager;
        }

        public override void OnShow() {
            AbstractWindowControl windowControl = this.Control as AbstractWindowControl;
            if (windowControl != null) {
                windowControl.OnShow();
            }
        }

        public override void OnClose() {
            WindowManager.GoBack(this);
        }
    }
}