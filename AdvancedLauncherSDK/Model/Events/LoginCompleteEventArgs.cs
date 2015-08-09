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
    /// Login complete event handler
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    public delegate void LoginCompleteEventHandler(object sender, LoginCompleteEventArgs e);

    /// <summary>
    /// Login status code
    /// </summary>
    public enum LoginCode {

        /// <summary>Success login code</summary>
        SUCCESS = 0,

        /// <summary>Wrong user code</summary>
        WRONG_USER = 1,

        /// <summary>Wrong page code</summary>
        WRONG_PAGE = 2,

        /// <summary>Unknown URL code</summary>
        UNKNOWN_URL = 3,

        /// <summary>Error code</summary>
        EXECUTE_ERROR = 4,

        /// <summary>Cancelled code</summary>
        CANCELLED = 5
    }

    /// <summary>
    /// Login complete event arguments
    /// </summary>
    public class LoginCompleteEventArgs : BaseEventArgs {

        /// <summary>
        /// Gets login status code
        /// </summary>
        public LoginCode Code {
            get;
            private set;
        }

        /// <summary>
        /// Gets username
        /// </summary>
        public string UserName {
            get;
            private set;
        }

        /// <summary>
        /// Gets command line arguments to start the game
        /// </summary>
        public string Arguments {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LoginCompleteEventArgs"/> for specified <see cref="LoginCode"/>.
        /// </summary>
        /// <param name="Code">Login code</param>
        public LoginCompleteEventArgs(LoginCode Code)
            : this(Code, string.Empty) {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LoginCompleteEventArgs"/> for specified <see cref="LoginCode"/> and arguments.
        /// </summary>
        /// <param name="Code">Login code</param>
        /// <param name="Arguments">Arguments</param>
        public LoginCompleteEventArgs(LoginCode Code, string Arguments)
            : this(Code, Arguments, string.Empty) {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LoginCompleteEventArgs"/> for specified <see cref="LoginCode"/>, arguments and user name.
        /// </summary>
        /// <param name="Code">Login code</param>
        /// <param name="Arguments">Arguments</param>
        /// <param name="UserName">User name</param>
        public LoginCompleteEventArgs(LoginCode Code, string Arguments, string UserName) {
            this.Code = Code;
            this.Arguments = Arguments;
            this.UserName = UserName;
        }
    }
}