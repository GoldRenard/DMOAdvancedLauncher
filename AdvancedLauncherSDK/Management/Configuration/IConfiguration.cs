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

using AdvancedLauncher.SDK.Model.Web;

namespace AdvancedLauncher.SDK.Management.Configuration {

    /// <summary>
    /// The game configuration interface. It defines all server-specific features.
    /// </summary>
    /// <seealso cref="IConfigurationManager"/>
    /// <seealso cref="AbstractConfiguration"/>
    public interface IConfiguration {

        /// <summary>
        /// Gets the name of configuration (short server name, server publisher, etc). Used for UI.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Gets full server name, including <see cref="GameType"/> and <see cref="Name"/>. Used for UI game type selection.
        /// </summary>
        string ServerName {
            get;
        }

        /// <summary>
        /// Gets short game type name like <b>GDMO</b>, <b>KDMO</b>, etc.
        /// </summary>
        string GameType {
            get;
        }

        /// <summary>
        /// Gets <b>True</b> if this configuration supports "Last session" feature that skips authorization and uses
        /// the last auth token for game start.
        /// </summary>
        bool IsLastSessionAvailable {
            get;
        }

        /// <summary>
        /// Gets registry key name for game path (without HKLM, HKCU).
        /// </summary>
        string GamePathRegKey {
            get;
        }

        /// <summary>
        /// Gets registry value name for game path (without HKLM, HKCU).
        /// </summary>
        string GamePathRegVal {
            get;
        }

        /// <summary>
        /// Gets registry key name for stock launcher path (without HKLM, HKCU).
        /// </summary>
        string LauncherPathRegKey {
            get;
        }

        /// <summary>
        /// Gets registry value name for stock launcher path (without HKLM, HKCU).
        /// </summary>
        string LauncherPathRegVal {
            get;
        }

        /// <summary>
        /// Gets game executable name
        /// </summary>
        string GameExecutable {
            get;
        }

        /// <summary>
        /// Gets launcher executable name
        /// </summary>
        string LauncherExecutable {
            get;
        }

        /// <summary>
        /// Gets relative path to version ini file (under <see cref="GameExecutable"/> location)
        /// </summary>
        string VersionLocalPath {
            get;
        }

        /// <summary>
        /// Gets URL for remote version ini file
        /// </summary>
        string VersionRemoteURL {
            get;
        }

        /// <summary>
        /// Gets patch URL with format <b>http://host/path/{0}.zip</b> where <b>{0}</b> is patch number
        /// </summary>
        string PatchRemoteURL {
            get;
        }

        /// <summary>
        /// Creates login provider interface.
        /// </summary>
        /// <seealso cref="ILoginProvider"/>
        /// <returns>Login provider interface</returns>
        ILoginProvider CreateLoginProvider();

        /// <summary>
        /// Creates web provider interface.
        /// </summary>
        /// <seealso cref="IWebProvider"/>
        /// <returns>Web provider interface</returns>
        IWebProvider CreateWebProvider();

        /// <summary>
        /// Creates news provider interface.
        /// </summary>
        /// <seealso cref="INewsProvider"/>
        /// <returns>News provider interface</returns>
        INewsProvider CreateNewsProvider();

        /// <summary>
        /// Creates servers provider interface.
        /// </summary>
        /// <seealso cref="IServersProvider"/>
        /// <returns>Servers provider interface</returns>
        IServersProvider ServersProvider {
            get;
        }

        /// <summary>
        /// Gets <b>True</b> if <see cref="IWebProvider"/> is available.
        /// </summary>
        bool IsWebAvailable {
            get;
        }

        /// <summary>
        /// Gets <b>True</b> if <see cref="INewsProvider"/> is available.
        /// </summary>
        bool IsNewsAvailable {
            get;
        }

        /// <summary>
        /// Gets <b>True</b> if <see cref="ILoginProvider"/> is available.
        /// </summary>
        bool IsLoginRequired {
            get;
        }

        /// <summary>
        /// Converts game parameters to acceptable for game executable
        /// </summary>
        /// <param name="args">Raw parameters</param>
        /// <returns>Converted parameters</returns>
        string ConvertGameStartArgs(string args);

        /// <summary>
        /// Converts game parameters to acceptable for stock launcher executable
        /// </summary>
        /// <param name="args">Raw parameters</param>
        /// <returns>Converted parameters</returns>
        string ConvertLauncherStartArgs(string args);
    }
}