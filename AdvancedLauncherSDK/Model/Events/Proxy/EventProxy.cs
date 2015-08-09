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

using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Model.Events.Proxy {

    /// <summary>
    /// The remote API doesn't allow to subscribe to event handlers directly,
    /// so we should use CrossDomain wrapper.
    /// </summary>
    /// <typeparam name="T">Type of event arguments</typeparam>
    public abstract class EventProxy<T> : CrossDomainObject where T : BaseEventArgs {

        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        public abstract void Handler(object sender, T e);
    }
}