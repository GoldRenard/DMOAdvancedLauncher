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

using System;
using System.Threading.Tasks;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.Model.Web {

    /// <summary>
    /// WebProvider allows to read and store information to database
    /// </summary>
    public abstract class DatabaseWebProvider : AbstractWebProvider {

        /// <summary>
        /// Gets DatabaseManager API
        /// </summary>
        protected readonly IDatabaseManager DatabaseManager;

        /// <summary>
        /// Initializes a new instance of <see cref="DatabaseWebProvider"/> for specified <see cref="IDatabaseManager"/> and <see cref="ILogManager"/>.
        /// </summary>
        /// <param name="DatabaseManager">DatabaseManager API</param>
        /// <param name="logManager">LogManager API</param>
        public DatabaseWebProvider(IDatabaseManager DatabaseManager, ILogManager logManager) : base(logManager) {
            this.DatabaseManager = DatabaseManager;
        }

        /// <summary>
        /// Returns guild
        /// </summary>
        /// <param name="server">Guild server</param>
        /// <param name="guildName">Guild name</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <param name="actualInterval">Interval of actual data in days</param>
        /// <returns>Guild</returns>
        public override Guild GetActualGuild(Server server, string guildName, bool isDetailed, int actualInterval) {
            bool fetchCurrent = false;
            using (IDatabaseContext context = DatabaseManager.CreateContext()) {
                Guild storedGuild = context.FindGuild(server, guildName);
                if (storedGuild != null && !(isDetailed && !storedGuild.IsDetailed) && storedGuild.UpdateTime != null) {
                    TimeSpan timeDiff = (TimeSpan)(DateTime.Now - storedGuild.UpdateTime);
                    if (timeDiff.Days < actualInterval) {
                        fetchCurrent = true;
                    }
                }
                if (fetchCurrent) {
                    OnStarted();
                    OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, guildName, 0, 50);
                    storedGuild = context.FetchGuild(server, guildName);
                    OnCompleted(DMODownloadResultCode.OK, storedGuild);
                    return storedGuild;
                }
            }
            return GetGuild(server, guildName, isDetailed);
        }

        /// <summary>
        /// Asynchronously starts guild obtaining
        /// </summary>
        /// <param name="server">Guild server</param>
        /// <param name="guildName">Guild name</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <param name="actualInterval">Interval of actual data in days</param>
        /// <seealso cref="AbstractWebProvider.DownloadStarted"/>
        /// <seealso cref="AbstractWebProvider.DownloadCompleted"/>
        /// <seealso cref="AbstractWebProvider.StatusChanged"/>
        public override void GetActualGuildAsync(Server server, string guildName, bool isDetailed, int actualInterval) {
            bool fetchCurrent = false;

            using (IDatabaseContext context = DatabaseManager.CreateContext()) {
                Guild storedGuild = context.FindGuild(server, guildName);
                if (storedGuild != null && !(isDetailed && !storedGuild.IsDetailed) && storedGuild.UpdateTime != null) {
                    TimeSpan timeDiff = (TimeSpan)(DateTime.Now - storedGuild.UpdateTime);
                    if (timeDiff.Days < actualInterval) {
                        fetchCurrent = true;
                    }
                }
            }
            if (fetchCurrent) {
                Task.Factory.StartNew(() => {
                    using (IDatabaseContext context = DatabaseManager.CreateContext()) {
                        OnStarted();
                        OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, guildName, 0, 50);
                        Guild storedGuild = context.FetchGuild(server, guildName);
                        OnCompleted(DMODownloadResultCode.OK, storedGuild);
                    }
                });
                return;
            }
            GetGuildAsync(server, guildName, isDetailed);
        }
    }
}