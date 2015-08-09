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

using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Events.Proxy;

namespace AdvancedLauncher.SDK.Management {

    /// <summary>
    /// Environment manager. Provides different information of execution environment
    /// </summary>
    public interface IEnvironmentManager : IManager {

        /// <summary>
        /// Gets user settings
        /// </summary>
        Settings Settings {
            get;
        }

        /// <summary>
        /// Gets application path
        /// </summary>
        string AppPath {
            get;
        }

        /// <summary>
        /// Get application data path
        /// </summary>
        string AppDataPath {
            get;
        }

        /// <summary>
        /// Gets settings file path
        /// </summary>
        string SettingsFile {
            get;
        }

        /// <summary>
        /// Gets KBLCService execution file path
        /// </summary>
        string KBLCFile {
            get;
        }

        /// <summary>
        /// Gets NTLEA execution file path
        /// </summary>
        string NTLEAFile {
            get;
        }

        /// <summary>
        /// Gets languages directory path
        /// </summary>
        string LanguagesPath {
            get;
        }

        /// <summary>
        /// Gets 3rd-party resources directory path
        /// </summary>
        string Resources3rdPath {
            get;
        }

        /// <summary>
        /// Gets resources directory path
        /// </summary>
        string ResourcesPath {
            get;
        }

        /// <summary>
        /// Gets plugins directory path
        /// </summary>
        string PluginsPath {
            get;
        }

        /// <summary>
        /// Gets database file path
        /// </summary>
        string DatabaseFile {
            get;
        }

        /// <summary>
        /// Saves environment changes to <see cref="SettingsFile"/>.
        /// </summary>
        void Save();

        /// <summary>
        /// Resolves specifies resource and downloads it to <see cref="Resources3rdPath"/> if it doesn't exist.
        /// </summary>
        /// <param name="folder">Folder name/path</param>
        /// <param name="file">File name</param>
        /// <param name="downloadUrl">File download URL</param>
        /// <returns>Correct resource path</returns>
        string ResolveResource(string folder, string file, string downloadUrl = null);

        /// <summary>
        /// FileSystemLocked event, fires on game archives locking. You should not use this directly, is doesn't work correctly for
        /// cross-domain transparent proxy instances.
        /// Use <see cref="FileSystemLockedProxy(EventProxy{LockedEventArgs}, bool)"/> instead.
        /// </summary>
        event LockedChangedHandler FileSystemLocked;

        /// <summary>
        /// Registers new event listener for game archives locking.
        /// </summary>
        /// <param name="proxy"><see cref="EventProxy{T}"/> instance</param>
        /// <param name="subscribe"><b>True</b> if you want to subscribe, <b>false</b> otherwise.</param>
        void FileSystemLockedProxy(EventProxy<LockedEventArgs> proxy, bool subscribe = true);

        /// <summary>
        /// Fires <see cref="FileSystemLocked"/>
        /// </summary>
        /// <param name="IsLocked">Should it be locked or not</param>
        void OnFileSystemLocked(bool IsLocked);

        /// <summary>
        /// ClosingLocked event, fires on application close locking. You should not use this directly, is doesn't work correctly for
        /// cross-domain transparent proxy instances.
        /// Use <see cref="ClosingLockedProxy(EventProxy{LockedEventArgs}, bool)"/> instead.
        /// </summary>
        event LockedChangedHandler ClosingLocked;

        /// <summary>
        /// Registers new event listener for application close locking.
        /// </summary>
        /// <param name="proxy"><see cref="EventProxy{T}"/> instance</param>
        /// <param name="subscribe"><b>True</b> if you want to subscribe, <b>false</b> otherwise.</param>
        void ClosingLockedProxy(EventProxy<LockedEventArgs> proxy, bool subscribe = true);

        /// <summary>
        /// Fires <see cref="ClosingLocked"/>
        /// </summary>
        /// <param name="IsLocked">Should it be locked or not</param>
        void OnClosingLocked(bool IsLocked);
    }
}