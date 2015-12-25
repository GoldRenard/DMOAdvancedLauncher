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
    /// Common configuration implementation
    /// </summary>
    /// <seealso cref="IConfigurationManager"/>
    /// <seealso cref="IConfiguration"/>
    public abstract class AbstractConfiguration : CrossDomainObject, IConfiguration {
        private IServersProvider _ServersProvider;

        private object LockObject = new object();

        /// <summary>
        /// Gets the name of configuration (short server name, server publisher, etc). Used for UI.
        /// </summary>
        public abstract string Name {
            get;
        }

        /// <summary>
        /// Gets full server name, including <see cref="GameType"/> and <see cref="Name"/>. Used for UI game type selection.
        /// </summary>
        public virtual string ServerName {
            get {
                return string.Format("{0} ({1})", GameType, Name);
            }
        }

        /// <summary>
        /// Gets short game type name like <b>GDMO</b>, <b>KDMO</b>, etc.
        /// </summary>
        public abstract string GameType {
            get;
        }

        /// <summary>
        /// Gets game executable name
        /// </summary>
        public abstract string GameExecutable {
            get;
        }

        /// <summary>
        /// Gets registry key name for game path (without HKLM, HKCU).
        /// </summary>
        public abstract string GamePathRegKey {
            get;
        }

        /// <summary>
        /// Gets registry value name for game path (without HKLM, HKCU).
        /// </summary>
        public abstract string GamePathRegVal {
            get;
        }

        /// <summary>
        /// Gets <b>True</b> if this configuration supports "Last session" feature that skips authorization and uses
        /// the last auth token for game start.
        /// </summary>
        public abstract bool IsLastSessionAvailable {
            get;
        }

        /// <summary>
        /// Gets launcher executable name
        /// </summary>
        public abstract string LauncherExecutable {
            get;
        }

        /// <summary>
        /// Gets registry key name for stock launcher path (without HKLM, HKCU).
        /// </summary>
        public abstract string LauncherPathRegKey {
            get;
        }

        /// <summary>
        /// Gets registry value name for stock launcher path (without HKLM, HKCU).
        /// </summary>
        public abstract string LauncherPathRegVal {
            get;
        }

        /// <summary>
        /// Gets patch URL with format <b>http://host/path/{0}.zip</b> where <b>{0}</b> is patch number
        /// </summary>
        public abstract string PatchRemoteURL {
            get;
        }

        /// <summary>
        /// Gets relative path to version ini file (under <see cref="GameExecutable"/> location)
        /// </summary>
        public abstract string VersionLocalPath {
            get;
        }

        /// <summary>
        /// Gets URL for remote version ini file
        /// </summary>
        public abstract string VersionRemoteURL {
            get;
        }

        /// <summary>
        /// Gets <b>True</b> if <see cref="IWebProvider"/> is available.
        /// </summary>
        public virtual bool IsWebAvailable {
            get {
                return false;
            }
        }

        /// <summary>
        /// Gets <b>True</b> if <see cref="INewsProvider"/> is available.
        /// </summary>
        public virtual bool IsNewsAvailable {
            get {
                return false;
            }
        }

        /// <summary>
        /// Gets <b>True</b> if <see cref="ILoginProvider"/> is available.
        /// </summary>
        public virtual bool IsLoginRequired {
            get {
                return false;
            }
        }

        /// <summary>
        /// Gets <b>True</b> if manual login is available.
        /// </summary>
        public virtual bool IsManualLoginSupported {
            get {
                return false;
            }
        }

        /// <summary>
        /// Creates login provider interface.
        /// </summary>
        /// <seealso cref="ILoginProvider"/>
        /// <returns>Login provider interface</returns>
        public virtual ILoginProvider CreateLoginProvider() {
            return null;
        }

        /// <summary>
        /// Creates web provider interface.
        /// </summary>
        /// <seealso cref="IWebProvider"/>
        /// <returns>Web provider interface</returns>
        public virtual IWebProvider CreateWebProvider() {
            return null;
        }

        /// <summary>
        /// Creates news provider interface.
        /// </summary>
        /// <seealso cref="INewsProvider"/>
        /// <returns>News provider interface</returns>
        public virtual INewsProvider CreateNewsProvider() {
            return null;
        }

        /// <summary>
        /// Gets servers provider interface.
        /// </summary>
        /// <seealso cref="IServersProvider"/>
        public IServersProvider ServersProvider {
            get {
                lock (LockObject) {
                    if (_ServersProvider == null) {
                        _ServersProvider = CreateServersProvider();
                    }
                }
                return _ServersProvider;
            }
        }

        /// <summary>
        /// Creates servers provider interface.
        /// </summary>
        /// <seealso cref="IServersProvider"/>
        /// <returns>Servers provider interface</returns>
        protected abstract IServersProvider CreateServersProvider();

        /// <summary>
        /// Converts game parameters to acceptable for game executable
        /// </summary>
        /// <param name="args">Raw parameters</param>
        /// <returns>Converted parameters</returns>
        public virtual string ConvertGameStartArgs(string args) {
            return args;
        }

        /// <summary>
        /// Converts game parameters to acceptable for stock launcher executable
        /// </summary>
        /// <param name="args">Raw parameters</param>
        /// <returns>Converted parameters</returns>
        public virtual string ConvertLauncherStartArgs(string args) {
            return string.Empty;
        }
    }
}