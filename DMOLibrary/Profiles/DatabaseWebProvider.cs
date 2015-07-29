using System;
using System.Threading.Tasks;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Web;
using DMOLibrary.Database.Context;

namespace DMOLibrary.Profiles {

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