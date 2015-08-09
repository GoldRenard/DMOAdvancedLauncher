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

namespace AdvancedLauncher.SDK.UI {

    /// <summary>
    /// Interface that represents remote UI control
    /// </summary>
    public interface IRemoteControl {

        /// <summary>
        /// If <b>True</b>, <see cref="Management.IWindowManager"/> try to fix the WPF Airspace issue
        /// </summary>
        bool EnableAirspaceFix {
            get;
        }

        /// <summary>
        /// By default returns real control object.
        /// </summary>
        /// <param name="contractAdapter">If <b>True</b>, the <see cref="System.AddIn.Contract.INativeHandleContract"/> instance returned.</param>
        /// <returns>Control object</returns>
        object GetControl(bool contractAdapter = false);

        /// <summary>
        /// Control show handler
        /// </summary>
        void OnShow();

        /// <summary>
        /// Control close handler
        /// </summary>
        void OnClose();
    }
}