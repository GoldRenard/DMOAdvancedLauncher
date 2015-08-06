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
using System.AddIn.Pipeline;
using System.Windows.Controls;

namespace AdvancedLauncher.SDK.Management.Windows {

    public class WindowContainer : CrossDomainObject, IWindow {
        protected readonly IWindowManager WindowManager;

        private Control Control;

        public WindowContainer(Control Control, IWindowManager WindowManager) {
            if (Control == null) {
                throw new ArgumentException("WindowManager cannot be null");
            }
            if (WindowManager == null) {
                throw new ArgumentException("WindowManager cannot be null");
            }
            this.Control = Control;
            this.WindowManager = WindowManager;
        }

        public object GetControl(bool contractAdapter = false) {
            if (contractAdapter) {
                return FrameworkElementAdapters.ViewToContractAdapter(Control);
            }
            return Control;
        }

        public virtual void OnShow() {
            AbstractWindowControl windowControl = Control as AbstractWindowControl;
            if (windowControl != null) {
                windowControl.OnShow();
            }
        }

        /// <summary>
        /// Returns to last opened window. You're free to override this method.
        /// </summary>
        public virtual void Close() {
            WindowManager.GoBack(this);
        }
    }
}