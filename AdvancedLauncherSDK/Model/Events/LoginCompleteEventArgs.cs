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
        SUCCESS = 0,
        WRONG_USER = 1,
        WRONG_PAGE = 2,
        UNKNOWN_URL = 3,
        EXECUTE_ERROR = 4,
        CANCELLED = 5
    }

    /// <summary>
    /// Login complete event arguments
    /// </summary>
    public class LoginCompleteEventArgs : EventArgs {

        /// <summary>
        /// Login status code
        /// </summary>
        public LoginCode Code {
            get;
            private set;
        }

        /// <summary>
        /// Login status code
        /// </summary>
        public string UserName {
            get;
            private set;
        }

        /// <summary>
        /// Command line arguments to start the game
        /// </summary>
        public string Arguments {
            get;
            private set;
        }

        public LoginCompleteEventArgs(LoginCode Code)
            : this(Code, string.Empty) {
        }

        public LoginCompleteEventArgs(LoginCode Code, string Arguments)
            : this(Code, Arguments, string.Empty) {
        }

        public LoginCompleteEventArgs(LoginCode Code, string Arguments, string UserName) {
            this.Code = Code;
            this.Arguments = Arguments;
            this.UserName = UserName;
        }
    }
}