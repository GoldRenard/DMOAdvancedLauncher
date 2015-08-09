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
using System.Collections.Generic;
using System.ComponentModel;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.Model.Web {

    /// <summary>
    /// Base implementation of <see cref="IWebProvider"/>
    /// </summary>
    public abstract class AbstractWebProvider : CrossDomainObject, IWebProvider {

        protected ILogManager LogManager {
            get;
            private set;
        }

        public AbstractWebProvider() {
        }

        public AbstractWebProvider(ILogManager logManager) {
            Initialize(logManager);
        }

        public void Initialize(ILogManager logManager) {
            LogManager = logManager;
        }

        #region EVENTS

        /// <summary>
        /// Download started event handler
        /// </summary>
        public event BaseEventHandler DownloadStarted;

        /// <summary>
        /// Download complete event handler
        /// </summary>
        public event DownloadCompleteEventHandler DownloadCompleted;

        /// <summary>
        /// Download status changing event handler
        /// </summary>
        public event DownloadStatusChangedEventHandler StatusChanged;

        protected virtual void OnStarted() {
            if (LogManager != null) {
                LogManager.Info("GuildInfo obtaining started.");
            }
            if (DownloadStarted != null) {
                DownloadStarted(this, BaseEventArgs.Empty);
            }
        }

        protected virtual void OnCompleted(DMODownloadResultCode code, Guild result) {
            if (LogManager != null) {
                LogManager.Info(String.Format("GuildInfo obtaining completed: code={0}, result={1}", code, result));
            }
            if (DownloadCompleted != null) {
                DownloadCompleted(this, new DownloadCompleteEventArgs(code, result));
            }
        }

        protected virtual void OnStatusChanged(DMODownloadStatusCode code, string info, int p, int pm) {
            if (LogManager != null) {
                LogManager.Info(String.Format("GuildInfo obtaining status changed: code={0}, info={1}, p={2}, pm={3}", code, info, p, pm));
            }
            if (StatusChanged != null) {
                StatusChanged(this, new DownloadStatusEventArgs(code, info, p, pm));
            }
        }

        #endregion EVENTS

        /// <summary>
        /// Asynchronously starts guild obtaining
        /// </summary>
        /// <param name="server">Guild server</param>
        /// <param name="guildName">Guild name</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <seealso cref="DownloadStarted"/>
        /// <seealso cref="DownloadCompleted"/>
        /// <seealso cref="StatusChanged"/>
        public void GetGuildAsync(Server server, string guildName, bool isDetailed) {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s1, e2) => {
                GetGuild(server, guildName, isDetailed);
            };
            bw.RunWorkerAsync();
        }

        /// <summary>
        /// Returns digimon types
        /// </summary>
        /// <returns>Digimon types</returns>
        public abstract List<DigimonType> GetDigimonTypes();

        /// <summary>
        /// Returns guild
        /// </summary>
        /// <param name="server">Guild server</param>
        /// <param name="guildName">Guild name</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <returns>Guild</returns>
        public abstract Guild GetGuild(Server server, string guildName, bool isDetailed);

        /// <summary>
        /// Updates guild information
        /// </summary>
        /// <param name="guild">Guild reference</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <returns><b>True</b> on success</returns>
        protected abstract bool GetGuildInfo(ref Guild guild, bool isDetailed);

        /// <summary>
        /// Returns digimon list for tamer
        /// </summary>
        /// <param name="tamer">Tamer to update</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <returns>Digimon list</returns>
        protected abstract List<Digimon> GetDigimons(Tamer tamer, bool isDetailed);

        /// <summary>
        /// Returns tamer's starter detailed information
        /// </summary>
        /// <param name="digimon">Starter reference</param>
        /// <param name="tamer">Tamer</param>
        /// <returns><b>True</b> on success</returns>
        protected abstract bool GetStarterInfo(ref Digimon digimon, Tamer tamer);

        /// <summary>
        /// Returns tamer's mercenary detailed information
        /// </summary>
        /// <param name="digimon">Mercenary reference</param>
        /// <param name="tamer">Tamer</param>
        /// <returns><b>True</b> on success</returns>
        protected abstract bool GetMercenaryInfo(ref Digimon digimon, Tamer tamer);

        /// <summary>
        /// Returns guild
        /// </summary>
        /// <param name="server">Guild server</param>
        /// <param name="guildName">Guild name</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <param name="actualInterval">Interval of actual data in days</param>
        /// <returns>Guild</returns>
        public abstract Guild GetActualGuild(Server server, string guildName, bool isDetailed, int actualInterval);

        /// <summary>
        /// Asynchronously starts guild obtaining
        /// </summary>
        /// <param name="server">Guild server</param>
        /// <param name="guildName">Guild name</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <param name="actualInterval">Interval of actual data in days</param>
        /// <seealso cref="DownloadStarted"/>
        /// <seealso cref="DownloadCompleted"/>
        /// <seealso cref="StatusChanged"/>
        public abstract void GetActualGuildAsync(Server server, string guildName, bool isDetailed, int actualInterval);
    }
}