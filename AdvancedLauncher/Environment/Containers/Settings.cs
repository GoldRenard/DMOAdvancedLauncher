// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace AdvancedLauncher.Environment.Containers {
    [XmlType(TypeName = "Settings")]
    public class Settings : INotifyPropertyChanged {
        [XmlElement("Language")]
        public string LanguageFile;
        [XmlElement("DefaultProfile")]
        public int DefaultProfile;

        [XmlArray("Profiles"), XmlArrayItem(typeof(Profile), ElementName = "Profile")]
        public ObservableCollection<Profile> Profiles {
            get;
            set;
        }

        #region Constructors

        public Settings() {
            this.Profiles = new ObservableCollection<Profile>();
        }

        public Settings(Settings source) {
            this.LanguageFile = source.LanguageFile;
            this.DefaultProfile = source.DefaultProfile;
            this.Profiles = new ObservableCollection<Profile>();
            foreach (Profile p in source.Profiles) {
                Profiles.Add(new Profile(p));
            }
        }

        public void Merge(Settings source) {
            this.LanguageFile = source.LanguageFile;
            this.DefaultProfile = source.DefaultProfile;
            this.Profiles = new ObservableCollection<Profile>();

            //Add clones of instances
            foreach (Profile p in source.Profiles) {
                Profiles.Add(new Profile(p));
            }
            OnCollectionChanged();

            //Updations corrent Profile
            Profile prof = Profiles.FirstOrDefault(i => i.Id == CurrentProfile.Id);
            if (prof == null) {
                CurrentProfile = Profiles[0];
            } else {
                CurrentProfile = prof;
            }
        }

        #endregion

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

        #endregion

        #region Events Section

        public delegate void ProfileLockedChangedHandler(bool IsLocked);
        public event ProfileLockedChangedHandler ProfileLocked;
        public void OnProfileLocked(bool IsLocked) {
            if (ProfileLocked != null) {
                ProfileLocked(IsLocked);
            }
        }

        public delegate void FileSystemLockedChangedHandler(bool IsLocked);
        public event FileSystemLockedChangedHandler FileSystemLocked;
        public void OnFileSystemLocked(bool IsLocked) {
            if (FileSystemLocked != null) {
                FileSystemLocked(IsLocked);
            }
        }

        public delegate void ClosingLockedChangedHandler(bool IsLocked);
        public event ClosingLockedChangedHandler ClosingLocked;
        public void OnClosingLocked(bool IsLocked) {
            if (ClosingLocked != null) {
                ClosingLocked(IsLocked);
            }
        }

        public delegate void ProfileChangedHandler();
        public event ProfileChangedHandler ProfileChanged;
        protected void OnCurrentChanged() {
            if (ProfileChanged != null) {
                ProfileChanged();
            }
        }

        public event ProfileChangedHandler CollectionChanged;
        protected void OnCollectionChanged() {
            if (CollectionChanged != null) {
                CollectionChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
