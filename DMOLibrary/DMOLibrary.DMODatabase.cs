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
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;

namespace DMOLibrary {

    public class DMODatabase {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(DMODatabase));
        private static string DatabaseFile;
        private SQLiteConnection connection;
        private SQLiteTransaction transaction;
        private static string SQL_CANT_PROC_QUERY = "SQLite Query Error:" + Environment.NewLine + "{0}" + Environment.NewLine + "Query:" + Environment.NewLine + "{1}";
        private static string SQL_CANT_CONNECT = "SQLite Error: Cant't connect to database.";
        private static string SQL_CANT_DELETE_DB = "SQLite Error: Cant't delete old database.";

        private static bool isConnected = false;

        #region Query list

        private static string Q_DTYPE_ALL = "SELECT * FROM Digimon_types;";
        private static string Q_DTYPE_BY_NAME = "SELECT * FROM Digimon_types WHERE [name] = '{0}';";
        private static string Q_DTYPE_BY_KNAME = "SELECT * FROM Digimon_types WHERE [name_korean] = '{0}';";
        private static string Q_DTYPE_BY_ID = "SELECT * FROM Digimon_types WHERE [id] = '{0}';";
        private static string Q_DTYPE_BY_SEARCH_GDMO = "SELECT * FROM Digimon_types WHERE [search_gdmo] = '{0}';";
        private static string Q_DTYPE_BY_SEARCH_KDMO = "SELECT * FROM Digimon_types WHERE [search_kdmo] = '{0}';";
        private static string Q_DTYPE_COUNT = "SELECT count([key]) FROM Digimon_types WHERE [id] = {0};";
        private static string Q_DTYPE_INSERT = "INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '{5}');";
        private static string Q_DTYPE_GUPDATE = "UPDATE Digimon_types SET [name] = '{1}', [search_gdmo] = '{2}' WHERE [id] = {0};";
        private static string Q_DTYPE_KUPDATE = "UPDATE Digimon_types SET [name_korean] = '{1}', [search_kdmo] = '{2}' WHERE [id] = {0};";
        private static string Q_TTYPE_BY_ID = "SELECT * FROM Tamer_types WHERE [id] = '{0}';";
        private static string Q_S_BY_NAME = "SELECT * FROM Servers;";

        private static string Q_G_COUNT = "SELECT count([key]) FROM Guilds WHERE [id] = {0} AND [serv_id] = {1};";
        private static string Q_G_SELECT_BY_NAME = "SELECT * FROM Guilds WHERE [name] = '{0}' AND [serv_id] = {1};";
        private static string Q_G_INSERT = "INSERT INTO Guilds ([id], [serv_id], [name], [rep], [master_id], [master_name], [rank], [update_date], [isDetailed]) VALUES ({0}, {1}, '{2}', {3}, {4}, '{5}', {6}, '{7}', {8});";
        private static string Q_G_UPDATE = "UPDATE Guilds SET [name] = '{2}', [rep] = {3}, [master_id] = {4}, [master_name] = '{5}', [rank] = {6}, [update_date] = '{7}' WHERE [id] = {0} AND [serv_id] = {1};";
        private static string Q_G_UPDATE_WD = "UPDATE Guilds SET [name] = '{2}', [rep] = {3}, [master_id] = {4}, [master_name] = '{5}', [rank] = {6}, [update_date] = '{7}', [isDetailed] = {8} WHERE [id] = {0} AND [serv_id] = {1};";

        private static string Q_T_SET_INACTIVE = "UPDATE Tamers SET [isActive] = 0 WHERE [serv_id] = {0} AND [guild_id] = {1};";
        private static string Q_T_COUNT = "SELECT count([key]) FROM Tamers WHERE [id] = {0} AND [serv_id] = {1};";
        private static string Q_T_SELECT = "SELECT * FROM Tamers WHERE [serv_id] = {0} AND [guild_id] = {1} AND [isActive] = 1;";
        private static string Q_T_INSERT = "INSERT INTO Tamers ([id], [serv_id], [type_id], [guild_id], [partner_key], [isActive], [name], [rank], [lvl]) VALUES ({0}, {1}, {2}, {3}, {4}, 1, '{5}', {6}, {7});";
        private static string Q_T_UPDATE = "UPDATE Tamers SET [type_id] = {2}, [guild_id] = {3}, [partner_key] = {4}, [isActive] = 1, [name] = '{5}', [rank] = {6}, [lvl] = {7} WHERE [id] = {0} AND [serv_id] = {1};";
        private static string Q_T_GET_PKEY = "SELECT key FROM Digimons WHERE [serv_id] = {0} AND [tamer_id] = {1} AND [type_id] >= 31001 AND [type_id] <= 31004;";

        private static string Q_D_SET_INACTIVE = "UPDATE Digimons SET [isActive] = 0 WHERE [serv_id] = {0} AND [tamer_id] = {1};";
        private static string Q_D_COUNT = "SELECT count([key]) FROM Digimons WHERE [serv_id] = {0} AND [tamer_id] = {1} AND [type_id] = {2};";
        private static string Q_D_SELECT = "SELECT * FROM Digimons WHERE [serv_id] = {0} AND [tamer_id] = {1} AND [isActive] = 1;";
        private static string Q_D_INSERT = "INSERT INTO Digimons ([serv_id], [tamer_id], [type_id], [name], [rank], [isActive], [lvl], [size_cm], [size_pc], [size_rank]) VALUES ({0}, {1}, {2}, '{3}', {4}, 1, {5}, '{6}', {7}, {8});";
        private static string Q_D_UPDATE_PART = "UPDATE Digimons SET [rank] = {3}, [isActive] = 1, [lvl] = {4} WHERE [serv_id] = {0} AND [tamer_id] = {1} AND [type_id] = {2};";
        private static string Q_D_UPDATE_FULL = "UPDATE Digimons SET [name] = '{3}', [rank] = {4}, [isActive] = 1, [lvl] = {5}, [size_cm] = '{6}', [size_pc] = {7}, [size_rank] = {8} WHERE [serv_id] = {0} AND [tamer_id] = {1} AND [type_id] = {2};";

        private static string Q_D_SELECT_RANDOM = @"
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

        private static string Q_D_SELECT_RANDOM2 = @"
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

        private static string Q_D_SELECT_RANDOM_TYPE = @"SELECT * FROM Digimon_types ORDER BY RANDOM() LIMIT 1";

        #endregion Query list

        #region Connection creating, opening, closing

        public DMODatabase(string databaseFile, string cInitQuery) {
            DatabaseFile = databaseFile;
            if (!File.Exists(DatabaseFile)) {
                if (!RecreateDB(cInitQuery)) {
                    System.Windows.Application.Current.Shutdown();
                } else {
                    return;
                }
            } else {
                connection = new SQLiteConnection("Data Source = " + DatabaseFile);
            }
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
                LOGGER.Error(ex);
                return false;
            }
        }

        public bool CloseConnection() {
            try {
                transaction.Commit();
                connection.Close();
                isConnected = false;
                return true;
            } catch {
                return false;
            }
        }

        #endregion Connection creating, opening, closing

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
    [name_korean] CHAR(100),
    [search_gdmo] CHAR(100),
    [search_kdmo] CHAR(100)
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

INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31003, 'Gaomon', NULL, '가오몬', 'gaomon', '가오몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31002, 'Lalamon', NULL, '라라몬', 'lalamon', '라라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31004, 'Falcomon', NULL, '팰코몬', 'falcomon', '팰코몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31001, 'Agumon', NULL, '아구몬', 'agumon', '아구몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31007, 'Agumon(Classic)', NULL, '아구몬클래식', 'agumonclassic', '아구몬클래식');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31043, 'Agumon(Black)', NULL, '아구몬(흑)', 'agumonblack', '아구몬흑');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (32019, 'Salamon(BlackGatomon)', NULL, '플롯트몬(블랙가트몬)', 'salamonblackgatomon', '플롯트몬블랙가트몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31019, 'Salamon', NULL, '플롯트몬(가트몬)', 'salamon', '플롯트몬가트몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31138, 'Gabumon', NULL, '파피몬', 'gabumon', '파피몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31068, 'Gabumon(Black)', NULL, '파피몬(흑)', 'gabumonblack', '파피몬흑');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31008, 'ExV-mon', NULL, '브이몬(엑스브이몬)', 'exvmon', '브이몬엑스브이몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (32008, 'Veedramon', NULL, '브이몬(브이드라몬)', 'veedramon', '브이몬브이드라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31013, 'PawnChessmonWhite', NULL, '폰체스몬W', 'pawnchessmonwhite', '폰체스몬W');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31009, 'PawnChessmonBlack', NULL, '폰체스몬B', 'pawnchessmonblack', '폰체스몬B');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31067, 'DemiDevimon', 'Devimon', '피코데블몬(고스몬)', 'demidevimon', '피코데블몬고스몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (32067, 'DemiDevimon', 'Soulmon', '피코데블몬(소울몬)', 'demidevimon', '피코데블몬소울몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (32021, 'Mosyamon', NULL, '코테몬(무사몬계열)', 'mosyamon', '코테몬무사몬계열');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31021, 'Kotemon', NULL, '코테몬(그라디몬계열)', 'kotemon', '코테몬그라디몬계열');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31014, 'Dracomon', 'Blue', '드라코몬(청)', 'dracomonblue', '드라코몬청');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (32014, 'Dracomon', 'Green', '드라코몬(녹)', 'dracomongreen', '드라코몬녹');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31041, 'Palmon', 'Woodmon', '팔몬(우드몬)', 'palmon', '팔몬우드몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (32041, 'Palmon', 'Togemon', '팔몬(니드몬)', 'palmon', '팔몬니드몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31033, 'Goblimon', NULL, '고부리몬', 'goblimon', '고부리몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31032, 'Sharmamon', NULL, '원시고부리몬', 'sharmamon', '원시고부리몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31029, 'Kunemon', NULL, '쿠네몬', 'kunemon', '쿠네몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31038, 'Dokunemon', NULL, '도쿠네몬', 'dokunemon', '도쿠네몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (32034, 'MechaNorimon', NULL, '톱니몬(메카노몬)', 'mechanorimon', '톱니몬메카노몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31034, 'Gardromon', NULL, '톱니몬(가드로몬)', 'gardromon', '톱니몬가드로몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31036, 'Dorumon', NULL, '돌몬(돌가몬)', 'dorumon', '돌몬(돌가몬)');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (32036, 'Dorumon[Dex]', NULL, '돌몬(데크스돌가몬)', 'dorumondex', '돌몬데크스돌가몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (33036, 'Dorumon(Reptiledramon)', NULL, '돌몬(라프타드라몬계열)', 'dorumonreptiledramon', '돌몬라프타드라몬계열');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31006, 'Renamon', NULL, '레나몬', 'renamon', '레나몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31010, 'Terriermon', NULL, '테리어몬', 'terriermon', '테리어몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31049, 'Elecmon', NULL, '에렉몬', 'elecmon', '에렉몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31050, 'Gomamon', NULL, '쉬라몬', 'gomamon', '쉬라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31028, 'Drimogemon', NULL, '두리몬', 'drimogemon', '두리몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31039, 'Dracumon', NULL, '드라큐몬', 'dracumon', '드라큐몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31031, 'Tentomon', NULL, '텐타몬', 'tentomon', '텐타몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31018, 'Gotsumon', NULL, '울퉁몬', 'gotsumon', '울퉁몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31048, 'Biyomon', NULL, '피요몬', 'biyomon', '피요몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31066, 'Impmon', NULL, '임프몬', 'impmon', '임프몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31017, 'Keramon', NULL, '케라몬', 'keramon', '케라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31020, 'Hawkmon', NULL, '호크몬', 'hawkmon', '호크몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (21134, 'DemiMeramon', NULL, '페티메라몬', 'demimeramon', '페티메라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31011, 'Monodramon', NULL, '모노드라몬', 'monodramon', '모노드라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (41132, 'Kiwimon', NULL, '키위몬', 'kiwimon', '키위몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31012, 'Patamon', NULL, '파닥몬', 'patamon', '파닥몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31027, 'Ryuudamon', NULL, '류우다몬', 'ryuudamon', '류우다몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (41033, 'Dobermon', NULL, '도베르몬', 'dobermon', '도베르몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (41159, 'Deputymon', NULL, '카우보이몬', 'deputymon', '카우보이몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31121, 'Bearmon', NULL, '베어몬', 'bearmon', '베어몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31005, 'Guilmon', NULL, '길몬', 'guilmon', '길몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31022, 'Candlemon', NULL, '캔들몬', 'candlemon', '캔들몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31035, 'CommanDramon', NULL, '코만드라몬', 'commandramon', '코만드라몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31015, 'Lopmon', NULL, '로프몬', 'lopmon', '로프몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (41088, 'Starmon', NULL, '스타몬', 'starmon', '스타몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31142, 'Wormmon', NULL, '추추몬', 'wormmon', '추추몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (41075, 'Gizumon', NULL, '기즈몬', 'gizumon', '기즈몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31026, 'Betamon', NULL, '베타몬', 'betamon', '베타몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (41146, 'Doggymon', NULL, '도그몬', 'doggymon', '도그몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (21137, 'Tanemon', NULL, '시드몬', 'tanemon', '시드몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31016, 'FanBeemon', NULL, '아기벌몬', 'fanbeemon', '아기벌몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31051, 'Kamemon', NULL, '카메몬', 'kamemon', '카메몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31023, 'Kudamon', NULL, '쿠다몬', 'kudamon', '쿠다몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31030, 'Armadillomon', NULL, '아르마몬', 'armadillomon', '아르마몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31042, 'Mushroomon', NULL, '머슈몬', 'mushroomon', '머슈몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31037, 'Arkadimon', NULL, '알카디몬', 'arkadimon', '알카디몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (33067, 'Myotismon', NULL, '피코데블몬(묘티스몬)', 'myotismon', '피코데블몬묘티스몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (33069, 'Tsukaimon', NULL, '츄카이몬', 'tsukaimon', '츄카이몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (33068, 'Gazimon', NULL, '가지몬', 'gazimon', '가지몬');
INSERT INTO Digimon_types([id], [name], [name_alt], [name_korean], [search_gdmo], [search_kdmo]) VALUES (31040, 'Swimmon', NULL, '스윔몬', 'swimmon', '스윔몬');

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
            if (File.Exists(DatabaseFile)) {
                try {
                    File.Delete(DatabaseFile);
                } catch (Exception ex) {
                    MSG_ERROR(SQL_CANT_DELETE_DB + ex.Message);
                    LOGGER.Error(ex);
                    return false;
                }
            }
            SQLiteConnection.CreateFile(DatabaseFile);
            connection = new SQLiteConnection("Data Source = " + DatabaseFile);
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

        #endregion First initialization

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
                LOGGER.Error(ex);
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
                LOGGER.Error(ex);
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

        #endregion Simple queries and utils

        #region Read section

        public List<DigimonType> GetDigimonTypes() {
            List<DigimonType> types = new List<DigimonType>();

            string query = Q_DTYPE_ALL;
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    DigimonType type = new DigimonType();
                    if (dataReader["name_alt"] != DBNull.Value) {
                        type.NameAlt = (string)dataReader["name_alt"];
                    }
                    type.Name = (string)dataReader["name"];
                    type.Id = Convert.ToInt32(dataReader["id"]);
                    types.Add(type);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                LOGGER.Error(ex);
                return null;
            }

            if (types.Count > 0) {
                return types;
            }
            return null;
        }

        public List<DigimonType> FindDigimonTypes(string query, object value) {
            query = string.Format(query, value.ToString());
            List<DigimonType> types = new List<DigimonType>();
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    DigimonType type = new DigimonType();
                    if (dataReader["name_alt"] != DBNull.Value) {
                        type.NameAlt = (string)dataReader["name_alt"];
                    }
                    type.Name = (string)dataReader["name"];
                    type.Id = Convert.ToInt32(dataReader["id"]);
                    if (dataReader["search_gdmo"] != DBNull.Value) {
                        type.SearchGDMO = (string)dataReader["search_gdmo"];
                    }
                    if (dataReader["search_kdmo"] != DBNull.Value) {
                        type.SearchKDMO = (string)dataReader["search_kdmo"];
                    }
                    types.Add(type);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                LOGGER.Error(ex);
                return null;
            }
            if (types.Count > 0) {
                return types;
            }
            LOGGER.WarnFormat("Unknown Digimon: {0}", value);
            return null;
        }

        public List<DigimonType> GetDigimonTypesByName(string name) {
            return FindDigimonTypes(Q_DTYPE_BY_NAME, name);
        }

        public List<DigimonType> GetDigimonTypesByKoreanName(string name) {
            return FindDigimonTypes(Q_DTYPE_BY_KNAME, name);
        }

        public List<DigimonType> GetDigimonTypesBySearchGDMO(string name) {
            return FindDigimonTypes(Q_DTYPE_BY_SEARCH_GDMO, name);
        }

        public List<DigimonType> GetDigimonTypesBySearchKDMO(string name) {
            return FindDigimonTypes(Q_DTYPE_BY_SEARCH_KDMO, name);
        }

        public DigimonType? GetDigimonTypeById(int id) {
            List<DigimonType> types = FindDigimonTypes(Q_DTYPE_BY_ID, id);
            if (types != null) {
                return types[0];
            }
            return null;
        }

        public TamerType GetTamerTypeById(int id) {
            TamerType type = new TamerType();
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
                LOGGER.Error(ex);
                return type;
            }
            if (type.Id == -1) {
                LOGGER.WarnFormat("Unknown tamer: id={0}", id);
            }
            return type;
        }

        public ObservableCollection<Server> GetServers() {
            ObservableCollection<Server> servers = new ObservableCollection<Server>();
            string query = Q_S_BY_NAME;
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    Server serv = new Server();
                    serv.Name = (string)dataReader["name"];
                    serv.Id = Convert.ToInt32(dataReader["id"]);
                    servers.Add(serv);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                LOGGER.Error(ex);
                return null;
            }
            return servers;
        }

        public Guild ReadOnlyGuild(string guildName, Server server, int actualDays) {
            Guild guild = new Guild();
            guild.Id = -1;

            string query = string.Format(Q_G_SELECT_BY_NAME, guildName, server.Id);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    guild.Id = Convert.ToInt32(dataReader["id"]);
                    guild.ServId = Convert.ToInt32(dataReader["serv_id"]);
                    guild.Name = (string)dataReader["name"];
                    guild.Rep = Convert.ToInt32(dataReader["rep"]);
                    guild.MasterId = Convert.ToInt32(dataReader["master_id"]);
                    guild.MasterName = (string)dataReader["master_name"];
                    guild.Rank = Convert.ToInt32(dataReader["rank"]);
                    guild.IsDetailed = Convert.ToInt32(dataReader["isDetailed"]) == 1 ? true : false;
                    guild.UpdateTime = String2DateTime((string)dataReader["update_date"]);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                LOGGER.Error(ex);
                guild.Id = -1;
                return guild;
            }
            return guild;
        }

        public List<Tamer> ReadTamers(Guild guild) {
            List<Tamer> tamers = new List<Tamer>();
            string query = string.Format(Q_T_SELECT, guild.ServId, guild.Id);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    Tamer tamer = new Tamer();
                    tamer.Id = Convert.ToInt32(dataReader["id"]);
                    tamer.ServId = Convert.ToInt32(dataReader["serv_id"]);
                    tamer.TypeId = Convert.ToInt32(dataReader["type_id"]);
                    tamer.GuildId = Convert.ToInt32(dataReader["guild_id"]);
                    tamer.PartnerKey = Convert.ToInt32(dataReader["partner_key"]);
                    tamer.Name = (string)dataReader["name"];
                    tamer.Rank = Convert.ToInt32(dataReader["rank"]);
                    tamer.Lvl = Convert.ToInt32(dataReader["lvl"]);
                    tamer.Digimons = ReadDigimons(tamer);
                    tamer.PartnerName = "-";
                    foreach (Digimon digimon in tamer.Digimons) {
                        if (digimon.Key == tamer.PartnerKey) {
                            tamer.PartnerName = digimon.Name;
                            break;
                        }
                    }
                    tamers.Add(tamer);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                tamers.Clear();
                return tamers;
            }
            return tamers;
        }

        public List<Digimon> ReadDigimons(Tamer tamer) {
            List<Digimon> digimons = new List<Digimon>();
            string query = string.Format(Q_D_SELECT, tamer.ServId, tamer.Id);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    Digimon d = new Digimon();
                    d.ServId = Convert.ToInt32(dataReader["serv_id"]);
                    d.TamerId = Convert.ToInt32(dataReader["tamer_id"]);
                    d.TypeId = Convert.ToInt32(dataReader["type_id"]);
                    d.Name = (string)dataReader["name"];
                    d.Rank = Convert.ToInt32(dataReader["rank"]);
                    d.Lvl = Convert.ToInt32(dataReader["lvl"]);
                    d.SizeCm = Convert.ToDouble(dataReader["size_cm"]);
                    d.SizePc = Convert.ToInt32(dataReader["size_pc"]);
                    d.SizeRank = Convert.ToInt32(dataReader["size_rank"]);
                    d.Key = Convert.ToInt32(dataReader["key"]);
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

        public Guild ReadGuild(string guildName, Server server, int actualDays) {
            Guild guild = ReadOnlyGuild(guildName, server, actualDays);
            if (guild.Id == -1) {
                return guild;
            }
            TimeSpan timeDiff = DateTime.Now - guild.UpdateTime;
            if (timeDiff.Days >= actualDays) {
                guild.Id = -1;
                return guild;
            }
            guild.Members = ReadTamers(guild);
            return guild;
        }

        #endregion Read section

        #region Write Section

        public bool WriteGuildInfo(Guild g, bool isDetailed) {
            if (QueryIntRes(string.Format(Q_G_COUNT, g.Id, g.ServId)) > 0) {
                if (isDetailed) {
                    return Query(string.Format(Q_G_UPDATE_WD, g.Id, g.ServId, g.Name, g.Rep, g.MasterId, g.MasterName, g.Rank, DateTime2String(g.UpdateTime), 1));
                } else {
                    return Query(string.Format(Q_G_UPDATE, g.Id, g.ServId, g.Name, g.Rep, g.MasterId, g.MasterName, g.Rank, DateTime2String(g.UpdateTime)));
                }
            } else {
                return Query(string.Format(Q_G_INSERT, g.Id, g.ServId, g.Name, g.Rep, g.MasterId, g.MasterName, g.Rank, DateTime2String(g.UpdateTime), isDetailed ? 1 : 0));
            }
        }

        public bool WriteTamer(Tamer t) {
            return Query(GetWriteTamerQuery(t));
        }

        private string GetWriteTamerQuery(Tamer t) {
            int pKey = QueryIntRes(string.Format(Q_T_GET_PKEY, t.ServId, t.Id));
            if (QueryIntRes(string.Format(Q_T_COUNT, t.Id, t.ServId)) > 0) {
                return string.Format(Q_T_UPDATE, t.Id, t.ServId, t.TypeId, t.GuildId, pKey, t.Name, t.Rank, t.Lvl);
            } else {
                return string.Format(Q_T_INSERT, t.Id, t.ServId, t.TypeId, t.GuildId, pKey, t.Name, t.Rank, t.Lvl);
            }
        }

        public bool WriteDigimon(Digimon d, bool isDetailed) {
            return Query(GetWriteDigimonQuery(d, isDetailed));
        }

        public bool WriteDigimonType(DigimonType type, bool isKorean) {
            DigimonType? tryType = GetDigimonTypeById(type.Id);
            if (tryType == null) {
                string query;
                if (isKorean) {
                    query = string.Format(Q_DTYPE_INSERT, type.Id, type.Name, String.Empty, type.Name, String.Empty, PrepareDigimonSearch(type.Name));
                } else {
                    query = string.Format(Q_DTYPE_INSERT, type.Id, type.Name, String.Empty, String.Empty, PrepareDigimonSearch(type.Name), String.Empty);
                }
                return Query(query);
            }
            DigimonType digimonType = (DigimonType)tryType;
            return Query(string.Format(isKorean ? Q_DTYPE_KUPDATE : Q_DTYPE_GUPDATE, type.Id, type.Name, PrepareDigimonSearch(type.Name)));
        }

        private string GetWriteDigimonQuery(Digimon d, bool isDetailed) {
            if (QueryIntRes(string.Format(Q_D_COUNT, d.ServId, d.TamerId, d.TypeId)) > 0) {
                if (isDetailed) {
                    return string.Format(Q_D_UPDATE_FULL, d.ServId, d.TamerId, d.TypeId, d.Name, d.Rank, d.Lvl, d.SizeCm, d.SizePc, d.SizeRank);
                } else {
                    return string.Format(Q_D_UPDATE_PART, d.ServId, d.TamerId, d.TypeId, d.Rank, d.Lvl);
                }
            } else {
                return string.Format(Q_D_INSERT, d.ServId, d.TamerId, d.TypeId, d.Name, d.Rank, d.Lvl, d.SizeCm, d.SizePc, d.SizeRank);
            }
        }

        public bool WriteGuild(Guild g, bool isDetailed) {
            //set all current tamers of guild to inactive (maybe they aren't in that guild)
            if (!Query(string.Format(Q_T_SET_INACTIVE, g.ServId, g.Id))) {
                return false;
            }
            foreach (Tamer t in g.Members) {
                if (!Query(string.Format(Q_D_SET_INACTIVE, t.ServId, t.Id))) {
                    return false;
                }
                foreach (Digimon d in t.Digimons) {
                    if (!WriteDigimon(d, isDetailed)) {
                        return false;
                    }
                }
                if (!WriteTamer(t)) {
                    return false;
                }
            }
            if (!WriteGuildInfo(g, isDetailed)) {
                return false;
            }
            return true;
        }

        #endregion Write Section

        #region Additional Section

        public Digimon RandomDigimon(Server serv, string g_name, int minlvl) {
            Digimon d = new Digimon();
            string query = string.Format(Q_D_SELECT_RANDOM, serv.Id, g_name, minlvl);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    d.ServId = Convert.ToInt32(dataReader["serv_id"]);
                    d.TamerId = Convert.ToInt32(dataReader["tamer_id"]);
                    d.TypeId = Convert.ToInt32(dataReader["type_id"]);
                    d.CustomTamerName = (string)dataReader["tamer_name"];
                    d.CustomTamerlvl = Convert.ToInt32(dataReader["tamer_lvl"]);
                    d.Name = (string)dataReader["name"];
                    d.Rank = Convert.ToInt32(dataReader["rank"]);
                    d.Lvl = Convert.ToInt32(dataReader["lvl"]);
                    d.SizeCm = Convert.ToDouble(dataReader["size_cm"]);
                    d.SizePc = Convert.ToInt32(dataReader["size_pc"]);
                    d.SizeRank = Convert.ToInt32(dataReader["size_rank"]);
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return d;
            }
            return d;
        }

        public Digimon RandomDigimon(Server serv, string g_name, string t_name, int minlvl) {
            Digimon d = new Digimon();
            bool IsLoaded = false;
            string query = string.Format(Q_D_SELECT_RANDOM2, serv.Id, g_name, t_name, minlvl);
            try {
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read()) {
                    d.ServId = Convert.ToInt32(dataReader["serv_id"]);
                    d.TamerId = Convert.ToInt32(dataReader["tamer_id"]);
                    d.TypeId = Convert.ToInt32(dataReader["type_id"]);
                    d.CustomTamerName = (string)dataReader["tamer_name"];
                    d.CustomTamerlvl = Convert.ToInt32(dataReader["tamer_lvl"]);
                    d.Name = (string)dataReader["name"];
                    d.Rank = Convert.ToInt32(dataReader["rank"]);
                    d.Lvl = Convert.ToInt32(dataReader["lvl"]);
                    d.SizeCm = Convert.ToDouble(dataReader["size_cm"]);
                    d.SizePc = Convert.ToInt32(dataReader["size_pc"]);
                    d.SizeRank = Convert.ToInt32(dataReader["size_rank"]);
                    IsLoaded = true;
                }
                dataReader.Close();
            } catch (Exception ex) {
                MSG_ERROR(string.Format(SQL_CANT_PROC_QUERY, ex.Message, query));
                return d;
            }
            if (!IsLoaded) {
                return null;
            }
            return d;
        }

        public DigimonType RandomDigimonType() {
            DigimonType d = new DigimonType();
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

        public static String PrepareDigimonSearch(string name) {
            if (name == null) {
                return null;
            }
            return name
                .Replace(" ", String.Empty)
                .Replace("(", String.Empty)
                .Replace(")", String.Empty)
                .Replace("[", String.Empty)
                .Replace("]", String.Empty)
                .Replace("-", String.Empty)
                .ToLower();
        }

        #endregion Additional Section
    }
}