// ======================================================================
// DMOLibrary
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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

namespace DMOLibrary {

    #region Structs

    public struct DownloadStatus {
        public DMODownloadStatusCode Code;
        public string Info;
        public int Progress;
        public int MaxProgress;
    }

    public enum DMODownloadStatusCode {
        GETTING_GUILD = 0,
        GETTING_TAMER = 1
    }

    public enum DMODownloadResultCode {
        OK = 0,
        DB_CONNECT_ERROR = 1,
        WEB_ACCESS_ERROR = 2,
        NOT_FOUND = 404,
        CANT_GET = 3
    }

    public enum LoginCode {
        SUCCESS = 0,
        WRONG_USER = 1,
        WRONG_PAGE = 2,
        UNKNOWN_URL = 3,
        EXECUTE_ERROR = 4
    }

    public enum LoginState {
        LOGINNING = 0,
        GETTING_DATA = 1,
        WAS_ERROR = 2
    }

    #endregion Structs
}