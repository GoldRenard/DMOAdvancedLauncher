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
using AdvancedLauncher.Providers.Database.Context;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Web;

namespace AdvancedLauncher.Providers {

    public abstract class DatabaseWebProvider : AbstractWebProvider {

        public DatabaseWebProvider(ILogManager logManager) : base(logManager) {
        }

        public override Guild GetActualGuild(Server server, string guildName, bool isDetailed, int actualInterval) {
            bool fetchCurrent = false;
            using (MainContext context = new MainContext()) {
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

        public override void GetActualGuildAsync(System.Windows.Threading.Dispatcher ownerDispatcher,
            Server server, string guildName, bool isDetailed, int actualInterval) {
            bool fetchCurrent = false;

            using (MainContext context = new MainContext()) {
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
                    using (MainContext context = new MainContext()) {
                        OnStarted();
                        OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, guildName, 0, 50);
                        Guild storedGuild = context.FetchGuild(server, guildName);
                        OnCompleted(DMODownloadResultCode.OK, storedGuild);
                    }
                });
                return;
            }
            GetGuildAsync(ownerDispatcher, server, guildName, isDetailed);
        }

        protected string DownloadContent(string url) {
            return WebClientEx.DownloadContent(LogManager, url, 5, 15000);
        }
    }
}