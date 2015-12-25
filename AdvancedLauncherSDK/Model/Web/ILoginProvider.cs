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
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.Model.Web {

    /// <summary>
    /// Login provider interface
    /// </summary>
    public interface ILoginProvider : ILoggable {

        /// <summary>
        /// Login completed event handler
        /// </summary>
        event LoginCompleteEventHandler LoginCompleted;

        /// <summary>
        /// Login state changed event handler
        /// </summary>
        event LoginStateEventHandler LoginStateChanged;

        /// <summary>
        /// Try to login with specified user and password
        /// </summary>
        /// <param name="UserId">User login</param>
        /// <param name="Password">User password</param>
        void TryLogin(string UserId, string Password);

        /// <summary>
        /// Try to manual login with specified user and password
        /// </summary>
        /// <param name="UserId">User login</param>
        /// <param name="Password">User password</param>
        void TryManualLogin(string UserId, string Password);

        /// <summary>
        /// Cancel login operation
        /// </summary>
        void CancelLogin();
    }
}