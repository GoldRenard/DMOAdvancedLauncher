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

using System.Collections;
using System.Collections.Generic;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Events.Proxy;

namespace AdvancedLauncher.SDK.Management {

    /// <summary>
    /// Configuration manager interface
    /// </summary>
    /// <seealso cref="IConfiguration"/>
    /// <seealso cref="AbstractConfiguration"/>
    /// <seealso cref="GameModel"/>
    public interface IConfigurationManager : IManager, IEnumerable, IEnumerable<IConfiguration> {

        /// <summary>
        /// Game validation (existing of currect executable, version file, game archives)
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns><b>True</b> on success</returns>
        bool CheckGame(GameModel model);

        /// <summary>
        /// Stock launcher validation (existing of executable)
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns><b>True</b> on success</returns>
        bool CheckLauncher(GameModel model);

        /// <summary>
        /// Checks for update access (game archive existing and read/write access)
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns><b>True</b> on success</returns>
        bool CheckUpdateAccess(GameModel model);

        /// <summary>
        /// Returns patch import directory path. Can be null on wrong game directory.
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns>Patch import directory path</returns>
        string GetImportPath(GameModel model);

        /// <summary>
        /// Returns local version ini file path. Can be null on wrong game directory.
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns>local version ini file path</returns>
        string GetLocalVersionFile(GameModel model);

        /// <summary>
        /// Returns game package file path. Can be null on wrong game directory.
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns>Game package file path</returns>
        string GetPFPath(GameModel model);

        /// <summary>
        /// Returns game header file path. Can be null on wrong game directory.
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns>Game header file path</returns>
        string GetHFPath(GameModel model);

        /// <summary>
        /// Returns game executable path. Can be null on wrong game directory.
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns>Game executable path</returns>
        string GetGameEXE(GameModel model);

        /// <summary>
        /// Returns stock launcher executable path. Can be null on wrong launcher directory.
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns>Game executable path</returns>
        string GetLauncherEXE(GameModel model);

        /// <summary>
        /// Returns stock launcher root path. First, from the <see cref="GameModel.LauncherPath"/> specified.
        /// Is it is null or empty, tries to get info from registry based on the current
        /// <see cref="IConfiguration.LauncherPathRegKey"/> and <see cref="IConfiguration.LauncherPathRegVal"/>.
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns>Stock launcher path</returns>
        string GetLauncherPath(GameModel model);

        /// <summary>
        /// Returns game root path. First, from the <see cref="GameModel.GamePath"/> specified.
        /// Is it is null or empty, tries to get info from registry based on the current
        /// <see cref="IConfiguration.GamePathRegKey"/> and <see cref="IConfiguration.GamePathRegVal"/>.
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <returns>Game path</returns>
        string GetGamePath(GameModel model);

        /// <summary>
        /// Returns <see cref="IConfiguration"/> for specified <see cref="GameModel.Type"/>.
        /// </summary>
        /// <param name="model"><see cref="GameModel"/> to process</param>
        /// <seealso cref="IConfiguration"/>
        /// <returns><see cref="IConfiguration"/>, acceptable for specified <see cref="GameModel"/>.</returns>
        IConfiguration GetConfiguration(GameModel model);

        /// <summary>
        /// Returns <see cref="IConfiguration"/> for specified game type.
        /// </summary>
        /// <param name="gameType">Game type name. Usually based on <see cref="GameModel.Type"/></param>
        /// <seealso cref="IConfiguration"/>
        /// <returns><see cref="IConfiguration"/>, acceptable for specified game type.</returns>
        IConfiguration GetConfiguration(string gameType);

        /// <summary>
        /// Registers specified configuration
        /// </summary>
        /// <param name="configuration"><see cref="IConfiguration"/> instance.</param>
        /// <returns><b>True</b> on success.</returns>
        bool RegisterConfiguration(IConfiguration configuration);

        /// <summary>
        /// UnRegisters specified configuration
        /// </summary>
        /// <param name="configuration"><see cref="IConfiguration"/> instance.</param>
        /// <returns><b>True</b> on success.</returns>
        bool UnRegisterConfiguration(IConfiguration configuration);

        /// <summary>
        /// Registers new event listener for configuration registration.
        /// </summary>
        /// <param name="proxy"><see cref="EventProxy{T}"/> instance</param>
        /// <param name="subscribe"><b>True</b> if you want to subscribe, <b>false</b> otherwise.</param>
        /// <seealso cref="EventProxy{T}"/>
        void ConfigurationRegisteredProxy(EventProxy<ConfigurationChangedEventArgs> proxy, bool subscribe = true);

        /// <summary>
        /// ConfigurationRegistered event. You should not use this directly, is doesn't work correctly for
        /// cross-domain transparent proxy instances.
        /// Use <see cref="ConfigurationRegisteredProxy(EventProxy{ConfigurationChangedEventArgs}, bool)"/> instead.
        /// </summary>
        event ConfigurationChangedEventHandler ConfigurationRegistered;

        /// <summary>
        /// Registers new event listener for configuration unregistration.
        /// </summary>
        /// <param name="proxy"><see cref="EventProxy{T}"/> instance</param>
        /// <param name="subscribe"><b>True</b> if you want to subscribe, <b>false</b> otherwise.</param>
        /// <seealso cref="EventProxy{T}"/>
        void ConfigurationUnRegisteredProxy(EventProxy<ConfigurationChangedEventArgs> proxy, bool subscribe = true);

        /// <summary>
        /// ConfigurationUnRegistered event. You should not use this directly, is doesn't work correctly for
        /// cross-domain transparent proxy instances.
        /// Use <see cref="ConfigurationUnRegisteredProxy(EventProxy{ConfigurationChangedEventArgs}, bool)"/> instead.
        /// </summary>
        event ConfigurationChangedEventHandler ConfigurationUnRegistered;
    }
}