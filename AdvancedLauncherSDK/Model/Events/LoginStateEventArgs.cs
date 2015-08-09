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
    /// Login state
    /// </summary>
    public enum LoginState {

        /// <summary>Logging in state</summary>
        LOGINNING = 0,

        /// <summary>Getting data state</summary>
        GETTING_DATA = 1,

        /// <summary>Error state</summary>
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
    public class LoginStateEventArgs : BaseEventArgs {

        /// <summary>
        /// Gets login state code
        /// </summary>
        public LoginState Code {
            get;
            private set;
        }

        /// <summary>
        /// Gets login attempt number
        /// </summary>
        public int AttemptNumber {
            get;
            private set;
        }

        /// <summary>
        /// Gets login last error code
        /// </summary>
        public int LastError {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LoginStateEventArgs"/> for specified <see cref="LoginState"/>,
        /// AttemptNumber and LastError code.
        /// </summary>
        /// <param name="Code">Login state code</param>
        /// <param name="AttemptNumber">Attempt number</param>
        /// <param name="LastError">Last error code</param>
        public LoginStateEventArgs(LoginState Code, int AttemptNumber, int LastError) {
            this.Code = Code;
            this.AttemptNumber = AttemptNumber;
            this.LastError = LastError;
        }
    }
}