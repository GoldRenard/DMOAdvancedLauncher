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
using System.ComponentModel;

namespace DMOLibrary.Profiles {

    public abstract class DMOWebProfile {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(DMOWebProfile));
        protected static int[] STARTER_IDS = { 31003, 31002, 31004, 31001 };
        protected DMODatabase Database = null;
        protected DownloadStatus downloadStatus = new DownloadStatus();
        protected bool IsBusy = false;

        #region EVENTS

        /* Complete results:
         * 0 - all good
         * 1 - can't connect to database
         * 2 - web access error
         * 404 - guild not found
         * 3 - can't get guild info
         * 4 - web page is not supported or guild not found
         * */

        protected System.Windows.Threading.Dispatcher OwnerDispatcher;

        public delegate void DownloadHandler(object sender);

        public delegate void DownloadCompleteHandler(object sender, DMODownloadResultCode code, Guild result);

        public delegate void DownloadStatusChangedHandler(object sender, DownloadStatus status);

        public event DownloadHandler DownloadStarted;

        public event DownloadCompleteHandler DownloadCompleted;

        public event DownloadStatusChangedHandler StatusChanged;

        protected virtual void OnStarted() {
            LOGGER.Info("GuildInfo obtaining started.");
            IsBusy = true;
            if (DownloadStarted != null) {
                if (OwnerDispatcher != null && !OwnerDispatcher.CheckAccess()) {
                    OwnerDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadHandler((sender) => {
                        DownloadStarted(sender);
                    }), this);
                } else {
                    DownloadStarted(this);
                }
            }
        }

        protected virtual void OnCompleted(DMODownloadResultCode code, Guild result) {
            LOGGER.Info(String.Format("GuildInfo obtaining completed: code={0}, result={1}", code, result));
            if (DownloadCompleted != null) {
                if (OwnerDispatcher != null && !OwnerDispatcher.CheckAccess()) {
                    OwnerDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadCompleteHandler((sender, code_, result_) => {
                        DownloadCompleted(sender, code_, result_);
                    }), this, code, result);
                } else
                    DownloadCompleted(this, code, result);
            }
            IsBusy = false;
        }

        protected virtual void OnStatusChanged(DMODownloadStatusCode code, string info, int p, int pm) {
            LOGGER.Info(String.Format("GuildInfo obtaining status changed: code={0}, info={1}, p={2}, pm={3}", code, info, p, pm));
            downloadStatus.Code = code;
            downloadStatus.Info = info;
            downloadStatus.Progress = p;
            downloadStatus.MaxProgress = pm;

            if (StatusChanged != null) {
                if (OwnerDispatcher != null && !OwnerDispatcher.CheckAccess()) {
                    OwnerDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadStatusChangedHandler((sender, status_) => {
                        StatusChanged(sender, status_);
                    }), this, downloadStatus);
                } else
                    StatusChanged(this, downloadStatus);
            }
        }

        #endregion EVENTS

        public void GetGuildAsync(System.Windows.Threading.Dispatcher ownerDispatcher, string guildName, Server serv, bool isDetailed, int actualDays) {
            this.OwnerDispatcher = ownerDispatcher;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s1, e2) => {
                GetGuild(guildName, serv, isDetailed, actualDays);
            };
            bw.RunWorkerAsync();
        }

        public Digimon GetRandomDigimon(Server serv, string guildName, int minlvl) {
            Digimon d = new Digimon();
            if (Database.OpenConnection()) {
                d = Database.RandomDigimon(serv, guildName, minlvl);
                Database.CloseConnection();
            }
            return d;
        }

        public Digimon GetRandomDigimon(Server serv, string guildName, string tamerName, int minlvl) {
            Digimon d = new Digimon();
            if (Database.OpenConnection()) {
                d = Database.RandomDigimon(serv, guildName, tamerName, minlvl);
                Database.CloseConnection();
            }
            return d;
        }

        public DigimonType GetRandomDigimonType() {
            DigimonType d = new DigimonType();
            if (Database.OpenConnection()) {
                d = Database.RandomDigimonType();
                Database.CloseConnection();
            }
            return d;
        }

        public abstract Guild GetGuild(string guildName, Server serv, bool isDetailed, int actualDays);

        protected abstract bool GetGuildInfo(ref Guild g, bool isDetailed);

        protected abstract List<Digimon> GetDigimons(Tamer tamer, bool isDetailed);

        protected abstract bool StarterInfo(ref Digimon digimon, string tamerName);

        protected abstract bool DigimonInfo(ref Digimon digimon, string tamerName);
    }
}