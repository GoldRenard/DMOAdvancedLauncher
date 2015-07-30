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

namespace AdvancedLauncher.SDK.Model.Events {

    /// <summary>
    /// Login state
    /// </summary>
    public enum LoginState {
        LOGINNING = 0,
        GETTING_DATA = 1,
        WAS_ERROR = 2
    }

    /// <summary>
    /// Login state event handler
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    public delegate void LoginStateEventHandler(object sender, LoginStateEventArgs e);

    /// <summary>
    /// Login state event arguments
    /// </summary>
    public class LoginStateEventArgs : EventArgs {

        /// <summary>
        /// Login state code
        /// </summary>
        public LoginState Code {
            get;
            private set;
        }

        /// <summary>
        /// Login try number
        /// </summary>
        public int TryNumber {
            get;
            private set;
        }

        /// <summary>
        /// Login last error code
        /// </summary>
        public int LastError {
            get;
            private set;
        }

        public LoginStateEventArgs(LoginState Code, int TryNumber, int LastError) {
            this.Code = Code;
            this.TryNumber = TryNumber;
            this.LastError = LastError;
        }
    }
}