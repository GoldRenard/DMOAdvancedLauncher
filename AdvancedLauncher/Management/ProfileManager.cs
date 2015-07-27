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
using System.Collections.ObjectModel;
using System.Linq;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Model.Config;
using Ninject;

namespace AdvancedLauncher.Management {

    public class ProfileManager : IProfileManager {

        #region Properties

        private List<Profile> _Profiles = new List<Profile>();

        public List<Profile> Profiles {
            get {
                return _Profiles;
            }
        }

        public ObservableCollection<Profile> PendingProfiles {
            get;
            set;
        } = new ObservableCollection<Profile>();

        public Profile PendingDefaultProfile {
            get;
            set;
        }

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

        private Profile _DefaultProfile;

        public Profile DefaultProfile {
            get {
                return _DefaultProfile;
            }
            set {
                _DefaultProfile = value;
            }
        }

        #endregion Properties

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get;
            set;
        }

        public void Initialize() {
            ApplyChanges(EnvironmentManager.Settings.Profiles, EnvironmentManager.Settings.DefaultProfile);
        }

        public void RevertChanges() {
            PendingDefaultProfile = new Profile(DefaultProfile);
            PendingProfiles.Clear();
            foreach (Profile profile in Profiles) {
                PendingProfiles.Add(new Profile(profile));
            }
        }

        public void ApplyChanges() {
            ApplyChanges(PendingProfiles, PendingDefaultProfile.Id);
            List<Profile> settingsProfiles = new List<Profile>();
            foreach (Profile p in Profiles) {
                settingsProfiles.Add(new Profile(p));
            }
            EnvironmentManager.Settings.DefaultProfile = DefaultProfile.Id;
            EnvironmentManager.Settings.Profiles = settingsProfiles;
        }

        public void AddProfile() {
            Profile pNew = new Profile() {
                Name = "NewProfile"
            };
            foreach (Profile p in PendingProfiles) {
                if (p.Id > pNew.Id) {
                    pNew.Id = p.Id;
                }
            }
            pNew.Id++;
            PendingProfiles.Add(pNew);
            OnCollectionChanged();
        }

        public void RemoveProfile(Profile profile) {
            if (PendingProfiles.Count > 1) {
                bool IsDefault = profile.Id == PendingDefaultProfile.Id;
                PendingProfiles.Remove(profile);
                OnCollectionChanged();
                if (IsDefault) {
                    PendingDefaultProfile = PendingProfiles.First();
                }
            }
        }

        private void ApplyChanges(ICollection<Profile> profiles, int defaultProfileId) {
            this.Profiles.Clear();
            //Add clones of instances
            foreach (Profile p in profiles) {
                Profiles.Add(new Profile(p));
            }

            DefaultProfile = Profiles.FirstOrDefault(i => i.Id == defaultProfileId);
            if (DefaultProfile == null) {
                DefaultProfile = Profiles.First();
            }
            if (_CurrentProfile != null) {
                _CurrentProfile = Profiles.FirstOrDefault(i => i.Id == _CurrentProfile.Id);
            }
            if (_CurrentProfile == null) {
                _CurrentProfile = DefaultProfile;
            }
            if (_CurrentProfile == null) {
                _CurrentProfile = Profiles.First();
            }
            OnCollectionChanged();
            OnCurrentChanged();
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