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

using System;
using System.Collections.Generic;

namespace DMOLibrary {

    #region Structs

    public struct Guild {
        public int Key;
        public int Id;
        public int ServId;
        public string Name;
        public long Rep;
        public long MasterId;
        public string MasterName;
        public long Rank;
        public DateTime UpdateTime;
        public bool IsDetailed;
        public List<Tamer> Members;
    }

    public class Digimon {
        public int Key;
        public int TypeId;
        public int ServId;
        public int TamerId;
        public string CustomTamerName;
        public int CustomTamerlvl;
        public string Name;
        public long Rank;
        public int Lvl;
        public double SizeCm;
        public int SizePc;
        public int SizeRank;
    }

    public struct Tamer {
        public int Key;
        public int Id;
        public int TypeId;
        public int ServId;
        public int GuildId;
        public int PartnerKey;
        public string PartnerName;
        public string Name;
        public long Rank;
        public int Lvl;
        public List<Digimon> Digimons;
    }

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