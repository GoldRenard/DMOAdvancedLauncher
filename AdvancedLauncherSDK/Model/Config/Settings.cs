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
        /// Selected language file
        /// </summary>
        public string LanguageFile {
            get; set;
        }

        /// <summary>
        /// Application theme
        /// </summary>
        public string AppTheme {
            get; set;
        }

        /// <summary>
        /// Application theme accent
        /// </summary>
        public string ThemeAccent {
            get; set;
        }

        /// <summary>
        /// Default profile at app start
        /// </summary>
        public Profile DefaultProfile {
            get;
            set;
        }

        /// <summary>
        /// Profile collection
        /// </summary>
        public List<Profile> Profiles {
            get;
            set;
        }

        /// <summary>
        /// <b>True</b> to allow app update checking
        /// </summary>
        public bool CheckForUpdates {
            get;
            set;
        }

        public Settings() {
            // default constructor
        }

        /// <summary>
        /// Creates new <see cref="Settings"/> based on another
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}