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

namespace AdvancedLauncher.SDK.Model.Config {

    /// <summary>
    /// User settings
    /// </summary>
    public class Settings : CrossDomainObject, INotifyPropertyChanged {

        /// <summary>
        /// Gets or sets selected language file
        /// </summary>
        public string LanguageFile {
            get; set;
        }

        /// <summary>
        /// Gets or sets application theme
        /// </summary>
        public string AppTheme {
            get; set;
        }

        /// <summary>
        /// Gets or sets application theme accent
        /// </summary>
        public string ThemeAccent {
            get; set;
        }

        /// <summary>
        /// Gets or sets default profile at app start
        /// </summary>
        public Profile DefaultProfile {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets profile collection
        /// </summary>
        public List<Profile> Profiles {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets value to determine is update checking will be enabled
        /// </summary>
        public bool CheckForUpdates {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new <see cref="Settings"/> instance
        /// </summary>
        public Settings() {
            // default constructor
        }

        /// <summary>
        /// Initializes a new <see cref="Settings"/> based on another
        /// </summary>
        /// <param name="source">Source <see cref="Settings"/></param>
        public Settings(Settings source) {
            this.LanguageFile = source.LanguageFile;
            this.AppTheme = source.AppTheme;
            this.ThemeAccent = source.ThemeAccent;
            this.CheckForUpdates = source.CheckForUpdates;
        }

        /// <summary>
        /// Merge data of other <see cref="Settings"/> into current
        /// </summary>
        /// <param name="source">Source <see cref="Settings"/></param>
        public void MergeConfig(Settings source) {
            this.LanguageFile = source.LanguageFile;
            this.AppTheme = source.AppTheme;
            this.ThemeAccent = source.ThemeAccent;
            this.CheckForUpdates = source.CheckForUpdates;
        }

        /// <summary>
        /// Property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}