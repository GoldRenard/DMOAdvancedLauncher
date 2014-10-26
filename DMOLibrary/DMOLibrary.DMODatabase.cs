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
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace DMOLibrary {
    public class DMODatabase {
        static string DB_FILE;
        SQLiteConnection connection;
        SQLiteTransaction transaction;
        static string SQL_CANT_PROC_QUERY = "SQLite Query Error:" + Environment.NewLine + "{0}" + Environment.NewLine + "Query:" + Environment.NewLine + "{1}";
        static string SQL_CANT_CONNECT = "SQLite Error: Cant't connect to database.";
        static string SQL_CANT_DELETE_DB = "SQLite Error: Cant't delete old database.";

        static bool isConnected = false;

        #region Query list
        static string Q_DTYPE_ALL = "SELECT * FROM Digimon_types;";
        static string Q_DTYPE_BY_NAME = "SELECT * FROM Digimon_types WHERE name = '{0}';";
        static string Q_DTYPE_BY_KNAME = "SELECT * FROM Digimon_types WHERE name_korean = '{0}';";
        static string Q_DTYPE_BY_ID = "SELECT * FROM Digimon_types WHERE id = '{0}';";
        static string Q_TTYPE_BY_ID = "SELECT * FROM Tamer_types WHERE id = '{0}';";
        static string Q_S_BY_NAME = "SELECT * FROM Servers;";

        static string Q_G_COUNT = "SELECT count([key]) FROM Guilds WHERE [id] = {0} AND [serv_id] = {1};";
        static string Q_G_SELECT_BY_NAME = "SELECT * FROM Guilds WHERE [name] = '{0}' AND [serv_id] = {1};";
        static string Q_G_INSERT = "INSERT INTO Guilds ([id], [serv_id], [name], [rep], [master_id], [master_name], [rank], [update_date], [isDetailed]) VALUES ({0}, {1}, '{2}', {3}, {4}, '{5}', {6}, '{7}', {8});";
        static string Q_G_UPDATE = "UPDATE Guilds SET [name] = '{2}', [rep] = {3}, [master_id] = {4}, [master_name] = '{5}', [rank] = {6}, [update_date] = '{7}' WHERE [id] = {0} AND [serv_id] = {1};";
        static string Q_G_UPDATE_WD = "UPDATE Guilds SET [name] = '{2}', [rep] = {3}, [master_id] = {4}, [master_name] = '{5}', [rank] = {6}, [update_date] = '{7}', [isDetailed] = {8} WHERE [id] = {0} AND [serv_id] = {1};";

        static string Q_T_SET_INACTIVE = "UPDATE Tamers SET [isActive] = 0 WHERE [serv_id] = {0} AND [guild_id] = {1};";
        static string Q_T_COUNT = "SELECT count([key]) FROM Tamers WHERE [id] = {0} AND [serv_id] = {1};";
        static string Q_T_SELECT = "SELECT tamer.*, Digimons.name as 'partner_name' FROM (SELECT * FROM Tamers WHERE [serv_id] = {0} AND [guild_id] = {1} AND [isActive] = 1) as tamer JOIN Digimons ON tamer.partner_key = Digimons.key;";
        static string Q_T_INSERT = "INSERT INTO Tamers ([id], [serv_id], [type_id], [guild_id], [partner_key], [isActive], [name], [rank], [lvl]) VALUES ({0}, {1}, {2}, {3}, {4}, 1, '{5}', {6}, {7});";
        static string Q_T_UPDATE = "UPDATE Tamers SET [type_id] = {2}, [guild_id] = {3}, [partner_key] = {4}, [isActive] = 1, [name] = '{5}', [rank] = {6}, [lvl] = {7} WHERE [id] = {0} AND [serv_id] = {1};";
        static string Q_T_GET_PKEY = "SELECT key FROM Digimons WHERE [serv_id] = {0} AND [tamer_id] = {1} AND [type_id] >= 31001 AND [type_id] <= 31004;";

        static string Q_D_SET_INACTIVE = "UPDATE Digimons SET [isActive] = 0 WHERE [serv_id] = {0} AND [tamer_id] = {1};";
        static string Q_D_COUNT = "SELECT count([key]) FROM Digimons WHERE [serv_id] = {0} AND [tamer_id] = {1} AND [type_id] = {2};";
        static string Q_D_SELECT = "SELECT * FROM Digimons WHERE [serv_id] = {0} AND [tamer_id] = {1} AND [isActive] = 1;";
        static string Q_D_INSERT = "INSERT INTO Digimons ([serv_id], [tamer_id], [type_id], [name], [rank], [isActive], [lvl], [size_cm], [size_pc], [size_rank]) VALUES ({0}, {1}, {2}, '{3}', {4}, 1, {5}, '{6}', {7}, {8});";
        static string Q_D_UPDATE_PART = "UPDATE Digimons SET [rank] = {3}, [isActive] = 1, [lvl] = {4} WHERE [serv_id] = {0} AND [tamer_id] = {1} AND [type_id] = {2};";
        static string Q_D_UPDATE_FULL = "UPDATE Digimons SET [name] = '{3}', [rank] = {4}, [isActive] = 1, [lvl] = {5}, [size_cm] = '{6}', [size_pc] = {7}, [size_rank] = {8} WHERE [serv_id] = {0} AND [tamer_id] = {1} AND [type_id] = {2};";

        static string Q_D_SELECT_RANDOM = @"
SELECT * FROM (
    SELECT *, Tamers.name as 'tamer_name', Tamers.lvl as 'tamer_lvl' FROM (
	    SELECT * FROM Digimons WHERE
	    [serv_id] = {0} AND
	    [tamer_id] IN (
		    SELECT [id] FROM Tamers WHERE [serv_id] = {0} AND [isActive] = 1 AND [guild_id] = (
			    SELECT [id] FROM Guilds WHERE [name] == '{1}'
		    )
	    ) AND
	    [lvl] >= {2} AND
	    [isActive] = 1
    ) as digimon JOIN Tamers ON digimon.tamer_id = Tamers.id AND Tamers.serv_id = {0}
) ORDER BY RANDOM() LIMIT 1";

        static string Q_D_SELECT_RANDOM2 = @"
SELECT * FROM (
    SELECT *, Tamers.name as 'tamer_name', Tamers.lvl as 'tamer_lvl' FROM (
	    SELECT * FROM Digimons WHERE
	    [serv_id] = {0} AND
	    [tamer_id] IN (
		    SELECT [id] FROM Tamers WHERE [serv_id] = {0} AND [name] = '{2}' AND [isActive] = 1 AND [guild_id] = (
			    SELECT [id] FROM Guilds WHERE [name] = '{1}'
		    )
	    ) AND
	    [lvl] >= {3} AND
	    [isActive] = 1
    ) as digimon JOIN Tamers ON digimon.tamer_id = Tamers.id AND Tamers.serv_id = {0}
) ORDER BY RANDOM() LIMIT 1";

        static string Q_D_SELECT_RANDOM_TYPE = @"SELECT * FROM Digimon_types ORDER BY RANDOM() LIMIT 1";

        #endregion

        #region Connection creating, opening, closing
        public DMODatabase(string DB_FILE_, string cInitQuery) {
            DB_FILE = DB_FILE_;
            if (!File.Exists(DB_FILE)) {
                if (!RecreateDB(cInitQuery))
                    System.Windows.Application.Current.Shutdown();
                else
                    return;
            } else
                connection = new SQLiteConnection("Data Source = " + DB_FILE);
        }

        public bool OpenConnection() {
            while (isConnected) DispatcherHelper.DoEvents();
            try {
                connection.Open();
                transaction = connection.BeginTransaction();
                isConnected = true;
                return true;
            } catch (SQLiteException ex) {
                MSG_ERROR(SQL_CANT_CONNECT + ex.Message);
                return false;
            }
        }
        public bool CloseConnection() {
            try {
                transaction.Commit();
                connection.Close();
                isConnected = false;
                return true;
            } catch { return false; }
        }
        #endregion

        #region First initialization
        public static string CREATE_DATABASE_QUERY = @"CREATE TABLE Servers(
	[id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
	[name] CHAR(100) NOT NULL
);
CREATE TABLE Guilds(
	[key] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
	[id] INTEGER NOT NULL, 
	[serv_id] INTEGER NOT NULL, 
	[name] CHAR(100) NOT NULL,
	[rep] BIGINT NOT NULL,
	[master_id] INT NOT NULL,
	[master_name] CHAR(100) NOT NULL,
	[rank] BIGINT NOT NULL,
	[update_date] CHAR(100) NOT NULL,
	[isDetailed] INTEGER NOT NULL,
	FOREIGN KEY ([serv_id]) REFERENCES Servers([id])
);

CREATE TABLE Tamer_types(
	[id] INTEGER PRIMARY KEY NOT NULL,
	[name] CHAR(100) NOT NULL
);

CREATE TABLE Tamers(
	[key] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
	[id] INTEGER NOT NULL,
	[serv_id] INTEGER NOT NULL, 
	[type_id] INT NOT NULL,
	[guild_id] INTEGER NOT NULL, 
	[partner_key] INTEGER NOT NULL,
	[isActive] INTEGER NOT NULL,
	[name] CHAR(100) NOT NULL,
	[rank] BIGINT NOT NULL,
	[lvl] INT NOT NULL, 
	FOREIGN KEY ([serv_id]) REFERENCES Servers([id]),
	FOREIGN KEY ([guild_id]) REFERENCES Guilds([id]),
	FOREIGN KEY ([type_id]) REFERENCES Tamer_types([id])
);

CREATE TABLE Digimon_types(
	[id] INTEGER PRIMARY KEY NOT NULL,
	[name] CHAR(100) NOT NULL,
	[name_alt] CHAR(100),
    [name_korean] CHAR(100)
);
CREATE TABLE Digimons(
	[key] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
	[serv_id] INTEGER NOT NULL, 
	[tamer_id] INT NOT NULL, 
	[type_id] INT NOT NULL,
	[name] CHAR(100) NOT NULL,
	[rank] BIGINT NOT NULL,
	[isActive] INTEGER NOT NULL,
	[lvl] INT NOT NULL, 
	[size_cm] REAL NOT NULL,
	[size_pc] INT NOT NULL,
	[size_rank] BIGINT NOT NULL,
	FOREIGN KEY ([serv_id]) REFERENCES Servers([id]),
	FOREIGN KEY ([tamer_id]) REFERENCES Tamers([id]),
	FOREIGN KEY ([type_id]) REFERENCES Digimon_types([id])
);

INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31003, 'Gaomon', NULL, '가오몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31002, 'Lalamon', NULL, '라라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31004, 'Falcomon', NULL, '팰코몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31001, 'Agumon', NULL, '아구몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31007, 'Agumon(Classic)', NULL, '아구몬클래식');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31043, 'Agumon(Black)', NULL, '아구몬(흑)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (32019, 'Salamon(BlackGatomon)', NULL, '플롯트몬(블랙가트몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31019, 'Salamon', NULL, '플롯트몬(가트몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31138, 'Gabumon', NULL, '파피몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31068, 'Gabumon(Black)', NULL, '파피몬(흑)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31008, 'ExV-mon', NULL, '브이몬(엑스브이몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (32008, 'Veedramon', NULL, '브이몬(브이드라몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31013, 'PawnChessmonWhite', NULL, '폰체스몬W');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31009, 'PawnChessmonBlack', NULL, '폰체스몬B');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31067, 'DemiDevimon', 'Devimon', '피코데블몬(고스몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (32067, 'DemiDevimon', 'Soulmon', '피코데블몬(소울몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (32021, 'Mosyamon', NULL, '코테몬(무사몬계열)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31021, 'Kotemon', NULL, '코테몬(그라디몬계열)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31014, 'Dracomon', 'Blue', '드라코몬(청)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (32014, 'Dracomon', 'Green', '드라코몬(녹)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31041, 'Palmon', 'Woodmon', '팔몬(우드몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (32041, 'Palmon', 'Togemon', '팔몬(니드몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31033, 'Goblimon', NULL, '고부리몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31032, 'Sharmamon', NULL, '원시고부리몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31029, 'Kunemon', NULL, '쿠네몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31038, 'Dokunemon', NULL, '도쿠네몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (32034, 'MechaNorimon', NULL, '톱니몬(메카노몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31034, 'Gardromon', NULL, '톱니몬(가드로몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31036, 'Dorumon', NULL, '돌몬(돌가몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (32036, 'Dorumon[Dex]', NULL, '돌몬(데크스돌가몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (33036, 'Dorumon(Reptiledramon)', NULL, '돌몬(라프타드라몬계열)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31006, 'Renamon', NULL, '레나몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31010, 'Terriermon', NULL, '테리어몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31049, 'Elecmon', NULL, '에렉몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31050, 'Gomamon', NULL, '쉬라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31028, 'Drimogemon', NULL, '두리몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31039, 'Dracumon', NULL, '드라큐몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31031, 'Tentomon', NULL, '텐타몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31018, 'Gotsumon', NULL, '울퉁몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31048, 'Biyomon', NULL, '피요몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31066, 'Impmon', NULL, '임프몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31017, 'Keramon', NULL, '케라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31020, 'Hawkmon', NULL, '호크몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (21134, 'DemiMeramon', NULL, '페티메라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31011, 'Monodramon', NULL, '모노드라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (41132, 'Kiwimon', NULL, '키위몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31012, 'Patamon', NULL, '파닥몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31027, 'Ryuudamon', NULL, '류우다몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (41033, 'Dobermon', NULL, '도베르몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (41159, 'Deputymon', NULL, '카우보이몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31121, 'Bearmon', NULL, '베어몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31005, 'Guilmon', NULL, '길몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31022, 'Candlemon', NULL, '캔들몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31035, 'CommanDramon', NULL, '코만드라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31015, 'Lopmon', NULL, '로프몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (41088, 'Starmon', NULL, '스타몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31142, 'Wormmon', NULL, '추추몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (41075, 'Gizumon', NULL, '기즈몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31026, 'Betamon', NULL, '베타몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (41146, 'Doggymon', NULL, '도그몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (21137, 'Tanemon', NULL, '시드몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31016, 'FanBeemon', NULL, '아기벌몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31051, 'Kamemon', NULL, '카메몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31023, 'Kudamon', NULL, '쿠다몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31030, 'Armadillomon', NULL, '아르마몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31042, 'Mushroomon', NULL, '머슈몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31037, 'Arkadimon', NULL, '알카디몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (33067, 'Myotismon', NULL, '피코데블몬(묘티스몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (33069, 'Tsukaimon', NULL, '츄카이몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (33068, 'Gazimon', NULL, '가지몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean]) VALUES (31040, 'Swimmon', NULL, '스윔몬');

INSERT INTO Tamer_types([id], [name]) VALUES (80001, 'Marcus Damon');
INSERT INTO Tamer_types([id], [name]) VALUES (80002, 'Thomas H. Norstein');
INSERT INTO Tamer_types([id], [name]) VALUES (80003, 'Yoshino Fujieda');
INSERT INTO Tamer_types([id], [name]) VALUES (80004, 'Keenan Krier');
INSERT INTO Tamer_types([id], [name]) VALUES (80005, 'Taichi Kamiya');
INSERT INTO Tamer_types([id], [name]) VALUES (80006, 'Tachikawa Mimi');
INSERT INTO Tamer_types([id], [name]) VALUES (80007, 'Ishida Yamato');
INSERT INTO Tamer_types([id], [name]) VALUES (80008, 'Takaishi Takeru');
";
        private bool RecreateDB(string cInitQuery) {
            if (File.Exists(DB_FILE)) {
                try { File.Delete(DB_FILE); } catch (Exception ex) { MSG_ERROR(SQL_CANT_DELETE_DB + ex.Message); return false; }
            }
            SQLiteConnection.CreateFile(DB_FILE);
            connection = new SQLiteConnection("Data Source = " + DB_FILE);
            OpenConnection();
            if (!Query(CREATE_DATABASE_QUERY)) {
                CloseConnection();
                return false;
            }
            if (!Query(cInitQuery)) {
                CloseConnection();
                return false;
            }
            CloseConnection();
            return true;
        }

        #endregion

        #region Simple queries and utils
        private bool Query(string query) {
            try {
                SQLiteTransaction transaction = connection.BeginTransaction();
                SQLiteCommand sqlComm = new SQLiteCommand(query, connection);
                sqlComm.ExecuteNonQuery();
                transaction.Commit();
                sqlComm.Dispose();
            } catch (SQLiteException ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return false;
            }
            return true;
        }

        private int QueryIntRes(string query) {
            int result = 0;
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            } catch (SQLiteException ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return 0;
            }
            return result;
        }

        private string DateTime2String(DateTime datetime) {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF");
        }
        private DateTime String2DateTime(string datetime) {
            return DateTime.ParseExact(datetime, "yyyy-MM-dd HH:mm:ss.FFFFFFF", System.Globalization.CultureInfo.CurrentCulture);
        }

        public static void MSG_ERROR(string text) {
            System.Windows.MessageBox.Show(text, "SQLite error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        #endregion

        #region Read section
        public List<digimon_type> Digimon_GetTypes() {
            List<digimon_type> types = new List<digimon_type>();

            string query = Q_DTYPE_ALL;
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    digimon_type type = new digimon_type();
                    if (dataReader["name_alt"] != DBNull.Value)
                        type.Name_alt = (string)dataReader["name_alt"];
                    type.Name = (string)dataReader["name"];
                    type.Id = Convert.ToInt32(dataReader["id"]);
                    types.Add(type);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return null;
            }

            if (types.Count > 0)
                return types;
            return null;
        }

        public List<digimon_type> Digimon_GetTypesByName(string name) {
            List<digimon_type> types = new List<digimon_type>();

            string query = string.Format(Q_DTYPE_BY_NAME, name);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    digimon_type type = new digimon_type();
                    if (dataReader["name_alt"] != DBNull.Value)
                        type.Name_alt = (string)dataReader["name_alt"];
                    type.Name = (string)dataReader["name"];
                    type.Id = Convert.ToInt32(dataReader["id"]);
                    types.Add(type);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return null;
            }

            if (types.Count > 0)
                return types;
            return null;
        }

        public List<digimon_type> Digimon_GetTypesByKoreanName(string name) {
            List<digimon_type> types = new List<digimon_type>();

            string query = string.Format(Q_DTYPE_BY_KNAME, name);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    digimon_type type = new digimon_type();
                    if (dataReader["name_alt"] != DBNull.Value)
                        type.Name_alt = (string)dataReader["name_alt"];
                    type.Name = (string)dataReader["name"];
                    type.Id = Convert.ToInt32(dataReader["id"]);
                    types.Add(type);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return null;
            }

            if (types.Count > 0)
                return types;
            return null;
        }

        public digimon_type Digimon_GetTypeById(int id) {
            digimon_type type = new digimon_type();
            type.Id = -1;

            string query = string.Format(Q_DTYPE_BY_ID, id.ToString());
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    if (dataReader["name_alt"] != DBNull.Value)
                        type.Name_alt = (string)dataReader["name_alt"];
                    else
                        type.Name_alt = null;
                    type.Name = (string)dataReader["name"];
                    type.Id = Convert.ToInt32(dataReader["id"]);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return type;
            }

            return type;
        }

        public tamer_type Tamer_GetTypeById(int id) {
            tamer_type type = new tamer_type();
            type.Id = -1;

            string query = string.Format(Q_TTYPE_BY_ID, id.ToString());
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    type.Name = (string)dataReader["name"];
                    type.Id = Convert.ToInt32(dataReader["id"]);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return type;
            }

            return type;
        }

        public ObservableCollection<server> GetServers() {
            ObservableCollection<server> servers = new ObservableCollection<server>();
            string query = Q_S_BY_NAME;
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    server serv = new server();
                    serv.Name = (string)dataReader["name"];
                    serv.Id = Convert.ToInt32(dataReader["id"]);
                    servers.Add(serv);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return null;
            }
            return servers;
        }

        public guild ReadOnlyGuild(string g_name, server serv, int ActualDays) {
            guild g = new guild();
            g.Id = -1;

            string query = string.Format(Q_G_SELECT_BY_NAME, g_name, serv.Id);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    g.Id = Convert.ToInt32(dataReader["id"]);
                    g.Serv_id = Convert.ToInt32(dataReader["serv_id"]);
                    g.Name = (string)dataReader["name"];
                    g.Rep = Convert.ToInt32(dataReader["rep"]);
                    g.Master_id = Convert.ToInt32(dataReader["master_id"]);
                    g.Master_name = (string)dataReader["master_name"];
                    g.Rank = Convert.ToInt32(dataReader["rank"]);
                    g.isDetailed = Convert.ToInt32(dataReader["isDetailed"]) == 1 ? true : false;
                    g.Update_time = String2DateTime((string)dataReader["update_date"]);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                g.Id = -1;
                return g;
            }

            return g;
        }

        public List<tamer> ReadTamers(guild g) {
            List<tamer> tamers = new List<tamer>();
            string query = string.Format(Q_T_SELECT, g.Serv_id, g.Id);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    tamer t = new tamer();
                    t.Id = Convert.ToInt32(dataReader["id"]);
                    t.Serv_id = Convert.ToInt32(dataReader["serv_id"]);
                    t.Type_id = Convert.ToInt32(dataReader["type_id"]);
                    t.Guild_id = Convert.ToInt32(dataReader["guild_id"]);
                    t.Partner_key = Convert.ToInt32(dataReader["partner_key"]);
                    t.Partner_name = (string)dataReader["partner_name"];
                    t.Name = (string)dataReader["name"];
                    t.Rank = Convert.ToInt32(dataReader["rank"]);
                    t.Lvl = Convert.ToInt32(dataReader["lvl"]);
                    t.Digimons = ReadDigimons(t);
                    tamers.Add(t);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                tamers.Clear();
                return tamers;
            }
            return tamers;
        }

        public List<digimon> ReadDigimons(tamer t) {
            List<digimon> digimons = new List<digimon>();
            string query = string.Format(Q_D_SELECT, t.Serv_id, t.Id);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    digimon d = new digimon();
                    d.Serv_id = Convert.ToInt32(dataReader["serv_id"]);
                    d.Tamer_id = Convert.ToInt32(dataReader["tamer_id"]);
                    d.Type_id = Convert.ToInt32(dataReader["type_id"]);
                    d.Name = (string)dataReader["name"];
                    d.Rank = Convert.ToInt32(dataReader["rank"]);
                    d.Lvl = Convert.ToInt32(dataReader["lvl"]);
                    d.Size_cm = Convert.ToDouble(dataReader["size_cm"]);
                    d.Size_pc = Convert.ToInt32(dataReader["size_pc"]);
                    d.Size_rank = Convert.ToInt32(dataReader["size_rank"]);
                    digimons.Add(d);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                digimons.Clear();
                return digimons;
            }
            return digimons;
        }

        public guild ReadGuild(string g_name, server serv, int ActualDays) {
            guild g = ReadOnlyGuild(g_name, serv, ActualDays);
            if (g.Id == -1)
                return g;
            TimeSpan time_diff = DateTime.Now - g.Update_time;
            if (time_diff.Days >= ActualDays) {
                g.Id = -1;
                return g;
            }

            g.Members = ReadTamers(g);

            return g;
        }
        #endregion

        #region Write Section
        public bool WriteGuildInfo(guild g, bool isDetailed) {
            if (QueryIntRes(string.Format(Q_G_COUNT, g.Id, g.Serv_id)) > 0) {
                if (isDetailed)
                    return Query(string.Format(Q_G_UPDATE_WD, g.Id, g.Serv_id, g.Name, g.Rep, g.Master_id, g.Master_name, g.Rank, DateTime2String(g.Update_time), 1));
                else
                    return Query(string.Format(Q_G_UPDATE, g.Id, g.Serv_id, g.Name, g.Rep, g.Master_id, g.Master_name, g.Rank, DateTime2String(g.Update_time)));
            } else
                return Query(string.Format(Q_G_INSERT, g.Id, g.Serv_id, g.Name, g.Rep, g.Master_id, g.Master_name, g.Rank, DateTime2String(g.Update_time), isDetailed ? 1 : 0));
        }

        public bool WriteTamer(tamer t) {
            return Query(WriteTamer_GetQuery(t));
        }

        private string WriteTamer_GetQuery(tamer t) {
            int partner_key = QueryIntRes(string.Format(Q_T_GET_PKEY, t.Serv_id, t.Id));
            if (QueryIntRes(string.Format(Q_T_COUNT, t.Id, t.Serv_id)) > 0)
                return string.Format(Q_T_UPDATE, t.Id, t.Serv_id, t.Type_id, t.Guild_id, partner_key, t.Name, t.Rank, t.Lvl);
            else
                return string.Format(Q_T_INSERT, t.Id, t.Serv_id, t.Type_id, t.Guild_id, partner_key, t.Name, t.Rank, t.Lvl);
        }

        public bool WriteDigimon(digimon d, bool isDetailed) {
            return Query(WriteDigimon_GetQuery(d, isDetailed));
        }

        private string WriteDigimon_GetQuery(digimon d, bool isDetailed) {
            if (QueryIntRes(string.Format(Q_D_COUNT, d.Serv_id, d.Tamer_id, d.Type_id)) > 0) {
                if (isDetailed)
                    return string.Format(Q_D_UPDATE_FULL, d.Serv_id, d.Tamer_id, d.Type_id, d.Name, d.Rank, d.Lvl, d.Size_cm, d.Size_pc, d.Size_rank);
                else
                    return string.Format(Q_D_UPDATE_PART, d.Serv_id, d.Tamer_id, d.Type_id, d.Rank, d.Lvl);
            } else
                return string.Format(Q_D_INSERT, d.Serv_id, d.Tamer_id, d.Type_id, d.Name, d.Rank, d.Lvl, d.Size_cm, d.Size_pc, d.Size_rank);
        }

        public bool WriteGuild(guild g, bool isDetailed) {
            //set all current tamers of guild to inactive (maybe they aren't in that guild)
            if (!Query(string.Format(Q_T_SET_INACTIVE, g.Serv_id, g.Id)))
                return false;
            foreach (tamer t in g.Members) {
                if (!Query(string.Format(Q_D_SET_INACTIVE, t.Serv_id, t.Id)))
                    return false;
                foreach (digimon d in t.Digimons)
                    if (!WriteDigimon(d, isDetailed))
                        return false;
                if (!WriteTamer(t))
                    return false;
            }
            if (!WriteGuildInfo(g, isDetailed))
                return false;
            return true;
        }
        #endregion

        #region Additional Section
        public digimon RandomDigimon(server serv, string g_name, int minlvl) {
            digimon d = new digimon();
            string query = string.Format(Q_D_SELECT_RANDOM, serv.Id, g_name, minlvl);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    d.Serv_id = Convert.ToInt32(dataReader["serv_id"]);
                    d.Tamer_id = Convert.ToInt32(dataReader["tamer_id"]);
                    d.Type_id = Convert.ToInt32(dataReader["type_id"]);
                    d.Custom_Tamer_Name = (string)dataReader["tamer_name"];
                    d.Custom_Tamer_lvl = Convert.ToInt32(dataReader["tamer_lvl"]);
                    d.Name = (string)dataReader["name"];
                    d.Rank = Convert.ToInt32(dataReader["rank"]);
                    d.Lvl = Convert.ToInt32(dataReader["lvl"]);
                    d.Size_cm = Convert.ToDouble(dataReader["size_cm"]);
                    d.Size_pc = Convert.ToInt32(dataReader["size_pc"]);
                    d.Size_rank = Convert.ToInt32(dataReader["size_rank"]);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return d;
            }
            return d;
        }

        public digimon RandomDigimon(server serv, string g_name, string t_name, int minlvl) {
            digimon d = new digimon();
            bool IsLoaded = false;
            string query = string.Format(Q_D_SELECT_RANDOM2, serv.Id, g_name, t_name, minlvl);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    d.Serv_id = Convert.ToInt32(dataReader["serv_id"]);
                    d.Tamer_id = Convert.ToInt32(dataReader["tamer_id"]);
                    d.Type_id = Convert.ToInt32(dataReader["type_id"]);
                    d.Custom_Tamer_Name = (string)dataReader["tamer_name"];
                    d.Custom_Tamer_lvl = Convert.ToInt32(dataReader["tamer_lvl"]);
                    d.Name = (string)dataReader["name"];
                    d.Rank = Convert.ToInt32(dataReader["rank"]);
                    d.Lvl = Convert.ToInt32(dataReader["lvl"]);
                    d.Size_cm = Convert.ToDouble(dataReader["size_cm"]);
                    d.Size_pc = Convert.ToInt32(dataReader["size_pc"]);
                    d.Size_rank = Convert.ToInt32(dataReader["size_rank"]);
                    IsLoaded = true;
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return d;
            }
            if (!IsLoaded)
                return null;
            return d;
        }

        public digimon_type RandomDigimonType() {
            digimon_type d = new digimon_type();
            try {
                SQLiteCommand cmd = new SQLiteCommand(Q_D_SELECT_RANDOM_TYPE, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    d.Id = Convert.ToInt32(dataReader["Id"]);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, Q_D_SELECT_RANDOM_TYPE));
                return d;
            }
            return d;
        }
        #endregion
    }
}
