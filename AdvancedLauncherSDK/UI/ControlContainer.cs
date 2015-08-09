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
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.UI {

    /// <summary>
    /// Base <see cref="IRemoteControl"/> implementation
    /// </summary>
    /// <seealso cref="IRemoteControl"/>
    /// <seealso cref="WindowContainer"/>
    /// <seealso cref="PageContainer"/>
    public class ControlContainer : CrossDomainObject, IRemoteControl {

        /// <summary>
        /// Gets Control related with this container
        /// </summary>
        protected Control Control {
            get;
            private set;
        }

        /// <summary>
        /// Gets <b>True</b>, so <see cref="IWindowManager"/> try to fix the WPF Airspace issue
        /// </summary>
        public bool EnableAirspaceFix {
            get {
                return true;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ControlContainer"/> for specified <see cref="Control"/>.
        /// </summary>
        /// <param name="Control">Control</param>
        public ControlContainer(Control Control) {
            if (Control == null) {
                throw new ArgumentException("Control cannot be null");
            }
            this.Control = Control;
        }

        /// <summary>
        /// By default returns real control object.
        /// </summary>
        /// <param name="contractAdapter">If <b>True</b>, the <see cref="System.AddIn.Contract.INativeHandleContract"/> instance returned.</param>
        /// <returns>Control object</returns>
        public object GetControl(bool contractAdapter = false) {
            if (contractAdapter) {
                return FrameworkElementAdapters.ViewToContractAdapter(Control);
            }
            return Control;
        }

        /// <summary>
        /// Control show handler
        /// </summary>
        public virtual void OnShow() {
            // nothing to do
        }

        /// <summary>
        /// Control close handler
        /// </summary>
        public virtual void OnClose() {
            // nothing to do
        }
    }
}