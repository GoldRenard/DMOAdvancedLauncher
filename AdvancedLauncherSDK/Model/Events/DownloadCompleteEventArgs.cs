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

using AdvancedLauncher.SDK.Model.Entity;

namespace AdvancedLauncher.SDK.Model.Events {

    /// <summary>
    /// WebProfile Download Complete event handler
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    public delegate void DownloadCompleteEventHandler(object sender, DownloadCompleteEventArgs e);

    /// <summary>
    /// Result of downloading
    /// </summary>
    public enum DMODownloadResultCode {

        /// <summary>
        /// Success code
        /// </summary>
        OK = 0,

        /// <summary>
        /// Web access error code
        /// </summary>
        WEB_ACCESS_ERROR = 1,

        /// <summary>
        /// Target not found code
        /// </summary>
        NOT_FOUND = 404,

        /// <summary>
        /// Internal error code
        /// </summary>
        CANT_GET = 2
    }

    /// <summary>
    /// Downloading event arguments
    /// </summary>
    public class DownloadCompleteEventArgs : BaseEventArgs {

        /// <summary>
        /// Gets download Code
        /// </summary>
        public DMODownloadResultCode Code {
            get;
            private set;
        }

        /// <summary>
        /// Gets target guild
        /// </summary>
        public Guild Guild {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DownloadCompleteEventArgs"/> for
        /// specified <see cref="DMODownloadResultCode"/> and <see cref="Guild"/>.
        /// </summary>
        /// <param name="Code">Result code</param>
        /// <param name="Guild">Guild instance</param>
        public DownloadCompleteEventArgs(DMODownloadResultCode Code, Guild Guild) {
            this.Code = Code;
            this.Guild = Guild;
        }
    }
}