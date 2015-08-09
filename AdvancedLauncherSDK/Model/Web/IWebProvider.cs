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

using System.Collections.Generic;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.Model.Web {

    /// <summary>
    /// Web provider interface to gram guild info from remote source
    /// </summary>
    public interface IWebProvider : ILoggable {

        /// <summary>
        /// Download started event handler
        /// </summary>
        event BaseEventHandler DownloadStarted;

        /// <summary>
        /// Download complete event handler
        /// </summary>
        event DownloadCompleteEventHandler DownloadCompleted;

        /// <summary>
        /// Download status changing event handler
        /// </summary>
        event DownloadStatusChangedEventHandler StatusChanged;

        /// <summary>
        /// Asynchronously starts guild obtaining
        /// </summary>
        /// <param name="server">Guild server</param>
        /// <param name="guildName">Guild name</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <seealso cref="DownloadStarted"/>
        /// <seealso cref="DownloadCompleted"/>
        /// <seealso cref="StatusChanged"/>
        void GetGuildAsync(Server server, string guildName, bool isDetailed);

        /// <summary>
        /// Returns digimon types
        /// </summary>
        /// <returns>Digimon types</returns>
        List<DigimonType> GetDigimonTypes();

        /// <summary>
        /// Returns guild
        /// </summary>
        /// <param name="server">Guild server</param>
        /// <param name="guildName">Guild name</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <returns>Guild</returns>
        Guild GetGuild(Server server, string guildName, bool isDetailed);

        /// <summary>
        /// Returns guild
        /// </summary>
        /// <param name="server">Guild server</param>
        /// <param name="guildName">Guild name</param>
        /// <param name="isDetailed">Shoul it be detailed data (like digimon size, real name, etc)</param>
        /// <param name="actualInterval">Interval of actual data in days</param>
        /// <returns>Guild</returns>
        Guild GetActualGuild(Server server, string guildName, bool isDetailed, int actualInterval);

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
        void GetActualGuildAsync(Server server, string guildName, bool isDetailed, int actualInterval);
    }
}