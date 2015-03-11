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
using System.Data.SQLite;
using System.IO;
using DMOLibrary.Database.Entity;

namespace DMOLibrary {

    public class DMODatabase {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(DMODatabase));
        private static string DatabaseFile;
        private SQLiteConnection connection;
        private SQLiteTransaction transaction;
        private static string SQL_CANT_PROC_QUERY = "SQLite Query Error:" + Environment.NewLine + "{0}" + Environment.NewLine + "Query:" + Environment.NewLine + "{1}";
        private static string SQL_CANT_CONNECT = "SQLite Error: Cant't connect to database.";
        private static string SQL_CANT_DELETE_DB = "SQLite Error: Cant't delete old database.";

        private bool isConnected = false;

        public bool IsConnected {
            get {
                return isConnected;
            }
        }

        #region Query list

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

        #endregion Query list

        #region Connection creating, opening, closing

        public DMODatabase(string databaseFile)
            : this(databaseFile, null) {
        }

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
            if (isConnected) {
                return true; // already connected
            }
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
            if (!isConnected) {
                return true; //already disconnected
            }
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
            if (cInitQuery != null) {
                if (!Query(cInitQuery)) {
                    CloseConnection();
                    return false;
                }
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

        public Guild ReadOnlyGuild(string guildName, Server server, int actualDays) {
            Guild guild = new Guild();
            guild.Id = -1;

            string query = string.Format(Q_G_SELECT_BY_NAME, guildName, server.Identifier);
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

        public bool WriteGuildInfo(Guild guild, bool isDetailed) {
            if (QueryIntRes(string.Format(Q_G_COUNT, guild.Id, guild.ServId)) > 0) {
                if (isDetailed) {
                    return Query(string.Format(Q_G_UPDATE_WD, guild.Id, guild.ServId, guild.Name, guild.Rep, guild.MasterId, guild.MasterName, guild.Rank, DateTime2String(guild.UpdateTime), 1));
                } else {
                    return Query(string.Format(Q_G_UPDATE, guild.Id, guild.ServId, guild.Name, guild.Rep, guild.MasterId, guild.MasterName, guild.Rank, DateTime2String(guild.UpdateTime)));
                }
            } else {
                return Query(string.Format(Q_G_INSERT, guild.Id, guild.ServId, guild.Name, guild.Rep, guild.MasterId, guild.MasterName, guild.Rank, DateTime2String(guild.UpdateTime), isDetailed ? 1 : 0));
            }
        }

        public bool WriteTamer(Tamer tamer) {
            return Query(GetWriteTamerQuery(tamer));
        }

        private string GetWriteTamerQuery(Tamer tamer) {
            int pKey = QueryIntRes(string.Format(Q_T_GET_PKEY, tamer.ServId, tamer.Id));
            if (QueryIntRes(string.Format(Q_T_COUNT, tamer.Id, tamer.ServId)) > 0) {
                return string.Format(Q_T_UPDATE, tamer.Id, tamer.ServId, tamer.TypeId, tamer.GuildId, pKey, tamer.Name, tamer.Rank, tamer.Lvl);
            } else {
                return string.Format(Q_T_INSERT, tamer.Id, tamer.ServId, tamer.TypeId, tamer.GuildId, pKey, tamer.Name, tamer.Rank, tamer.Lvl);
            }
        }

        public bool WriteDigimon(Digimon digimon, bool isDetailed) {
            return Query(GetWriteDigimonQuery(digimon, isDetailed));
        }

        private string GetWriteDigimonQuery(Digimon digimon, bool isDetailed) {
            if (QueryIntRes(string.Format(Q_D_COUNT, digimon.ServId, digimon.TamerId, digimon.TypeId)) > 0) {
                if (isDetailed) {
                    return string.Format(Q_D_UPDATE_FULL, digimon.ServId, digimon.TamerId, digimon.TypeId, digimon.Name, digimon.Rank, digimon.Lvl, digimon.SizeCm, digimon.SizePc, digimon.SizeRank);
                } else {
                    return string.Format(Q_D_UPDATE_PART, digimon.ServId, digimon.TamerId, digimon.TypeId, digimon.Rank, digimon.Lvl);
                }
            } else {
                return string.Format(Q_D_INSERT, digimon.ServId, digimon.TamerId, digimon.TypeId, digimon.Name, digimon.Rank, digimon.Lvl, digimon.SizeCm, digimon.SizePc, digimon.SizeRank);
            }
        }

        public bool WriteGuild(Guild guild, bool isDetailed) {
            //set all current tamers of guild to inactive (maybe they aren't in that guild)
            if (!Query(string.Format(Q_T_SET_INACTIVE, guild.ServId, guild.Id))) {
                return false;
            }
            foreach (Tamer tamer in guild.Members) {
                if (!Query(string.Format(Q_D_SET_INACTIVE, tamer.ServId, tamer.Id))) {
                    return false;
                }
                foreach (Digimon digimon in tamer.Digimons) {
                    if (!WriteDigimon(digimon, isDetailed)) {
                        return false;
                    }
                }
                if (!WriteTamer(tamer)) {
                    return false;
                }
            }
            if (!WriteGuildInfo(guild, isDetailed)) {
                return false;
            }
            return true;
        }

        #endregion Write Section

        #region Additional Section

        public Digimon FindRandomDigimon(Server server, string guildName, int minlvl) {
            Digimon d = new Digimon();
            string query = string.Format(Q_D_SELECT_RANDOM, server.Identifier, guildName, minlvl);
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

        public Digimon FindRandonDigimon(Server server, string guildName, string tamerName, int minlvl) {
            Digimon d = new Digimon();
            bool IsLoaded = false;
            string query = string.Format(Q_D_SELECT_RANDOM2, server.Identifier, guildName, tamerName, minlvl);
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

        #endregion Additional Section
    }
}