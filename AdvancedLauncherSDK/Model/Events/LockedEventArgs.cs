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

namespace AdvancedLauncher.SDK.Model.Events {

    /// <summary>
    /// LockedChanged event handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void LockedChangedHandler(object sender, LockedEventArgs e);

    /// <summary>
    /// Locked Event args
    /// </summary>
    public class LockedEventArgs : BaseEventArgs {

        /// <summary>
        /// Gets <b>True</b> if it locking event
        /// </summary>
        public bool IsLocked {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LockedEventArgs"/> for specified locking state.
        /// </summary>
        /// <param name="IsLocked"><b>True</b> is locking event, otherwide unlocking</param>
        public LockedEventArgs(bool IsLocked) {
            this.IsLocked = IsLocked;
        }
    }
}