// ======================================================================
// DMOLibrary
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
using DMOLibrary.Database.Entity;

namespace DMOLibrary.Events {

    /// <summary>
    /// Result of downloading
    /// </summary>
    public enum DMODownloadResultCode {
        OK = 0,
        WEB_ACCESS_ERROR = 1,
        NOT_FOUND = 404,
        CANT_GET = 2
    }

    /// <summary>
    /// Downloading event arguments
    /// </summary>
    public class DownloadCompleteEventArgs : EventArgs {

        /// <summary>
        /// Download Code
        /// </summary>
        public DMODownloadResultCode Code {
            get;
            private set;
        }

        /// <summary>
        /// Target guild
        /// </summary>
        public Guild Guild {
            get;
            private set;
        }

        public DownloadCompleteEventArgs(DMODownloadResultCode Code, Guild Guild) {
            this.Code = Code;
            this.Guild = Guild;
        }
    }
}