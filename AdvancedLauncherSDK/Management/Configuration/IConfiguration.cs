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
        /// The name of configuration (short server name, server publisher, etc). Used for UI.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Full server name, including <see cref="GameType"/> and <see cref="Name"/>. Used for Settings UI gametype selection.
        /// </summary>
        string ServerName {
            get;
        }

        /// <summary>
        /// Short game type name like <b>GDMO</b>, <b>KDMO</b>, etc.
        /// </summary>
        string GameType {
            get;
        }

        /// <summary>
        /// Is this configuration support "Last session" feature that skips authorization and uses
        /// the last auth token for game start.
        /// </summary>
        bool IsLastSessionAvailable {
            get;
        }

        /// <summary>
        /// Defines registry key name for game path (without HKLM, HKCU).
        /// </summary>
        string GamePathRegKey {
            get;
        }

        /// <summary>
        /// Defines registry value name for game path (without HKLM, HKCU).
        /// </summary>
        string GamePathRegVal {
            get;
        }

        /// <summary>
        /// Defines registry key name for stock launcher path (without HKLM, HKCU).
        /// </summary>
        string LauncherPathRegKey {
            get;
        }

        /// <summary>
        /// Defines registry value name for stock launcher path (without HKLM, HKCU).
        /// </summary>
        string LauncherPathRegVal {
            get;
        }

        /// <summary>
        /// Game executable name
        /// </summary>
        string GameExecutable {
            get;
        }

        /// <summary>
        /// Launcher executable name
        /// </summary>
        string LauncherExecutable {
            get;
        }

        /// <summary>
        /// Relative path to version ini file (root is game root with <see cref="GameExecutable"/> location)
        /// </summary>
        string VersionLocalPath {
            get;
        }

        /// <summary>
        /// URL for remote version ini file
        /// </summary>
        string VersionRemoteURL {
            get;
        }

        /// <summary>
        /// Patch URL with format <b>http://host/path/{0}.zip</b> where <b>{0}</b> is patch number
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
        /// Defines is <see cref="IWebProvider"/> available.
        /// </summary>
        bool IsWebAvailable {
            get;
        }

        /// <summary>
        /// Defines is <see cref="INewsProvider"/> available.
        /// </summary>
        bool IsNewsAvailable {
            get;
        }

        /// <summary>
        /// Defines is <see cref="IWILoginProviderebProvider"/> available.
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