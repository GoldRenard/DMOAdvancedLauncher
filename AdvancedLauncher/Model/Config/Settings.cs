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
using System.Xml.Serialization;

namespace AdvancedLauncher.Model.Config {

    [XmlType(TypeName = "Settings")]
    public class Settings : INotifyPropertyChanged {

        [XmlElement("Language")]
        public string LanguageFile {
            get; set;
        }

        [XmlElement("AppTheme")]
        public string AppTheme {
            get; set;
        }

        [XmlElement("ThemeAccent")]
        public string ThemeAccent {
            get; set;
        }

        [XmlElement("DefaultProfile")]
        public int DefaultProfile {
            get; set;
        }

        private ProxySetting _Proxy = new ProxySetting();

        [XmlElement("Proxy")]
        public ProxySetting Proxy {
            set {
                _Proxy = value;
                NotifyPropertyChanged("Proxy");
            }
            get {
                return _Proxy;
            }
        }

        [XmlArray("Profiles"), XmlArrayItem(typeof(Profile), ElementName = "Profile")]
        public List<Profile> Profiles {
            get;
            set;
        }

        #region Constructors

        public Settings() {
            this.Profiles = new List<Profile>();
        }

        public Settings(Settings source)
            : this(source, false) {
        }

        public Settings(Settings source, bool copyConfigOnly) {
            this.LanguageFile = source.LanguageFile;
            this.AppTheme = source.AppTheme;
            this.ThemeAccent = source.ThemeAccent;
            this.Proxy = new ProxySetting(source.Proxy);
            if (!copyConfigOnly) {
                this.DefaultProfile = source.DefaultProfile;
                this.Profiles = new List<Profile>();
                foreach (Profile p in source.Profiles) {
                    Profiles.Add(new Profile(p));
                }
            }
        }

        public void MergeConfig(Settings source) {
            this.LanguageFile = source.LanguageFile;
            this.AppTheme = source.AppTheme;
            this.ThemeAccent = source.ThemeAccent;
            this.Proxy = new ProxySetting(source.Proxy);
            OnConfigurationChanged(this, null);
        }

        #endregion Constructors

        #region Events Section

        public event EventHandler ConfigurationChanged;

        public void OnConfigurationChanged(object sender, EventArgs args) {
            if (ConfigurationChanged != null) {
                ConfigurationChanged(sender, args);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Events Section
    }
}