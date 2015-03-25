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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace AdvancedLauncher.Environment.Containers {

    [XmlType(TypeName = "Settings")]
    public class Settings : INotifyPropertyChanged {

        [XmlElement("Language")]
        public string LanguageFile;

        [XmlElement("AppTheme")]
        public string AppTheme;

        [XmlElement("ThemeAccent")]
        public string ThemeAccent;

        [XmlElement("DefaultProfile")]
        public int DefaultProfile;

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
        public ObservableCollection<Profile> Profiles {
            get;
            set;
        }

        #region Constructors

        public Settings() {
            this.Profiles = new ObservableCollection<Profile>();
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
                this.Profiles = new ObservableCollection<Profile>();
                foreach (Profile p in source.Profiles) {
                    Profiles.Add(new Profile(p));
                }
            }
        }

        public void MergeProfiles(Settings source) {
            this.DefaultProfile = source.DefaultProfile;
            this.Profiles = new ObservableCollection<Profile>();

            //Add clones of instances
            foreach (Profile p in source.Profiles) {
                Profiles.Add(new Profile(p));
            }
            OnCollectionChanged();

            // Update currentProfiles
            Profile prof = Profiles.FirstOrDefault(i => i.Id == CurrentProfile.Id);
            if (prof == null) {
                CurrentProfile = Profiles[0];
            } else {
                CurrentProfile = prof;
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

        #region Collection Manipulating Section

        public void AddProfile() {
            Profile pNew = new Profile() {
                Name = "NewProfile"
            };
            foreach (Profile p in Profiles) {
                if (p.Id > pNew.Id) {
                    pNew.Id = p.Id;
                }
            }
            pNew.Id++;
            Profiles.Add(pNew);
            OnCollectionChanged();
        }

        public void RemoveProfile(Profile profile) {
            if (Profiles.Count > 1) {
                bool IsCurrent = profile.Id == CurrentProfile.Id;
                bool IsDefault = profile.Id == DefaultProfile;
                Profiles.Remove(profile);
                OnCollectionChanged();
                if (IsCurrent)
                    CurrentProfile = Profiles[0];
                if (IsDefault)
                    DefaultProfile = Profiles[0].Id;
            }
        }

        private Profile _CurrentProfile = null;

        [XmlIgnore]
        public Profile CurrentProfile {
            get {
                if (_CurrentProfile == null) {
                    Profile profile = Profiles.FirstOrDefault(i => i.Id == DefaultProfile);
                    if (profile == null) {
                        _CurrentProfile = Profiles[0];
                    } else {
                        _CurrentProfile = profile;
                    }
                }
                return _CurrentProfile;
            }
            set {
                _CurrentProfile = value;
                OnCurrentChanged();
            }
        }

        #endregion Collection Manipulating Section

        #region Events Section

        public event EventHandler ConfigurationChanged;

        public void OnConfigurationChanged(object sender, EventArgs args) {
            if (ConfigurationChanged != null) {
                ConfigurationChanged(sender, args);
            }
        }

        public delegate void LockedChangedHandler(object sender, LockedEventArgs e);

        public event LockedChangedHandler ProfileLocked;

        public event LockedChangedHandler FileSystemLocked;

        public event LockedChangedHandler ClosingLocked;

        public void OnProfileLocked(bool IsLocked) {
            if (ProfileLocked != null) {
                ProfileLocked(this, new LockedEventArgs(IsLocked));
            }
        }

        public void OnFileSystemLocked(bool IsLocked) {
            if (FileSystemLocked != null) {
                FileSystemLocked(this, new LockedEventArgs(IsLocked));
            }
        }

        public void OnClosingLocked(bool IsLocked) {
            if (ClosingLocked != null) {
                ClosingLocked(this, new LockedEventArgs(IsLocked));
            }
        }

        public event EventHandler ProfileChanged;

        protected void OnCurrentChanged() {
            if (ProfileChanged != null) {
                ProfileChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CollectionChanged;

        protected void OnCollectionChanged() {
            if (CollectionChanged != null) {
                CollectionChanged(this, EventArgs.Empty);
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