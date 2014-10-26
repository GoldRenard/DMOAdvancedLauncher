// ======================================================================
// DMOLibrary
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

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
    public class server {
        public int Id { set; get; }
        public string Name { set; get; }
    }

    public struct guild {
        public int Key;
        public int Id;
        public int Serv_id;
        public string Name;
        public long Rep;
        public long Master_id;
        public string Master_name;
        public long Rank;
        public DateTime Update_time;
        public bool isDetailed;
        public List<tamer> Members;
    }

    public struct digimon_type {
        public int Id;
        public string Name;
        public string Name_alt;
    }

    public class digimon {
        public int Key;
        public int Type_id;
        public int Serv_id;
        public int Tamer_id;
        public string Custom_Tamer_Name;
        public int Custom_Tamer_lvl;
        public string Name;
        public long Rank;
        public int Lvl;
        public double Size_cm;
        public int Size_pc;
        public int Size_rank;
    }

    public struct tamer_type {
        public int Id;
        public string Name;
    }

    public struct tamer {
        public int Key;
        public int Id;
        public int Type_id;
        public int Serv_id;
        public int Guild_id;
        public int Partner_key;
        public string Partner_name;
        public string Name;
        public long Rank;
        public int Lvl;
        public List<digimon> Digimons;
    }

    public struct NewsItem {
        public string mode;
        public string subj;
        public string date;
        public string content;
        public string url;
    }

    public struct DownloadStatus {
        public DMODownloadStatusCode code;
        public string info;
        public int progress;
        public int max_progress;
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
    #endregion
}
