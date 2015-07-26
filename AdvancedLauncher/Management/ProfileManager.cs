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
using System.Linq;
using AdvancedLauncher.Model.Config;

namespace AdvancedLauncher.Management {

    internal class ProfileManager {

        public ObservableCollection<Profile> Profiles {
            get;
            set;
        } = new ObservableCollection<Profile>();

        private Profile _CurrentProfile = null;

        public Profile CurrentProfile {
            get {
                return _CurrentProfile;
            }
            set {
                _CurrentProfile = value;
                OnCurrentChanged();
            }
        }

        public Profile DefaultProfile {
            set; get;
        }

        private static ProfileManager _Instance;

        public static ProfileManager Instance {
            get {
                if (_Instance == null) {
                    _Instance = new ProfileManager(EnvironmentManager.Settings);
                }
                return _Instance;
            }
        }

        private ProfileManager(ProfileManager profileManager) {
            Reload(profileManager);
        }

        private ProfileManager(Settings settings) {
            Reload(settings);
        }

        public void Reload(Settings settings) {
            this.Profiles = new ObservableCollection<Profile>();
            //Add clones of instances
            foreach (Profile p in settings.Profiles) {
                Profiles.Add(new Profile(p));
            }

            DefaultProfile = Profiles.FirstOrDefault(i => i.Id == settings.DefaultProfile);
            if (DefaultProfile == null) {
                DefaultProfile = Profiles.First();
            }
            if (CurrentProfile != null) {
                CurrentProfile = Profiles.FirstOrDefault(i => i.Id == CurrentProfile.Id);
            }
            if (CurrentProfile == null) {
                CurrentProfile = DefaultProfile;
            }
            if (CurrentProfile == null) {
                CurrentProfile = Profiles.First();
            }
            OnCollectionChanged();
        }

        public void Reload(ProfileManager profileManager) {
            this.Profiles = new ObservableCollection<Profile>();
            //Add clones of instances
            foreach (Profile p in profileManager.Profiles) {
                Profiles.Add(new Profile(p));
            }

            DefaultProfile = Profiles.FirstOrDefault(i => i.Id == profileManager.DefaultProfile.Id);
            if (DefaultProfile == null) {
                DefaultProfile = Profiles.First();
            }
            if (CurrentProfile != null) {
                CurrentProfile = Profiles.FirstOrDefault(i => i.Id == CurrentProfile.Id);
            }
            if (CurrentProfile == null) {
                CurrentProfile = DefaultProfile;
            }
            if (CurrentProfile == null) {
                CurrentProfile = Profiles.First();
            }
            OnCollectionChanged();
        }

        public ProfileManager Clone() {
            return new ProfileManager(this);
        }

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
                bool IsDefault = profile.Id == DefaultProfile.Id;
                Profiles.Remove(profile);
                OnCollectionChanged();
                if (IsCurrent) {
                    CurrentProfile = Profiles.First();
                }
                if (IsDefault) {
                    DefaultProfile = Profiles.First();
                }
            }
        }

        #region EventHandlers

        public event LockedChangedHandler ProfileLocked;

        public void OnProfileLocked(bool IsLocked) {
            if (ProfileLocked != null) {
                ProfileLocked(this, new LockedEventArgs(IsLocked));
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

        #endregion EventHandlers
    }
}