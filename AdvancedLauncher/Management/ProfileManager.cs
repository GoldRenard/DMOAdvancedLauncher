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
using AdvancedLauncher.Model.Config;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.Tools;
using Ninject;

namespace AdvancedLauncher.Management {

    internal class ProfileManager : IProfileManager {

        #region Properties

        private List<IProfile> _Profiles = new List<IProfile>();

        public List<IProfile> Profiles {
            get {
                return _Profiles;
            }
        }

        public ObservableCollection<IProfile> PendingProfiles {
            get;
            set;
        } = new ObservableCollection<IProfile>();

        public IProfile PendingDefaultProfile {
            get;
            set;
        }

        private IProfile _CurrentProfile = null;

        public IProfile CurrentProfile {
            get {
                return _CurrentProfile;
            }
            set {
                _CurrentProfile = value;
                OnCurrentChanged();
            }
        }

        private IProfile _DefaultProfile;

        public IProfile DefaultProfile {
            get {
                return _DefaultProfile;
            }
            set {
                _DefaultProfile = value;
            }
        }

        #endregion Properties

        [Inject]
        public IConfigurationManager ConfigurationManager {
            get; set;
        }

        [Inject]
        public ILauncherManager LauncherManager {
            get; set;
        }

        public void Initialize() {
            // nothing to do here
        }

        public void RevertChanges() {
            PendingDefaultProfile = new Profile(DefaultProfile);
            PendingProfiles.Clear();
            foreach (IProfile profile in Profiles) {
                PendingProfiles.Add(new Profile(profile));
            }
        }

        public void ApplyChanges() {
            ApplyChanges(PendingProfiles, PendingDefaultProfile.Id);
        }

        public IProfile CreateProfile() {
            IProfile pNew = new Profile() {
                Name = "NewProfile",
                Id = PendingProfiles.Count > 0 ? PendingProfiles.Max(p => p.Id) + 1 : 1
            };

            IConfiguration config = ConfigurationManager.FirstOrDefault();
            if (config != null) {
                pNew.GameModel.Type = config.GameType;
                pNew.GameModel.GamePath = config.GetGamePathFromRegistry();
                pNew.GameModel.LauncherPath = config.GetLauncherPathFromRegistry();
                if (config.IsWebAvailable) {
                    Server serv = config.ServersProvider.ServerList.FirstOrDefault();
                    if (serv != null) {
                        pNew.Rotation.ServerId = serv.Identifier;
                    }
                }
            }
            pNew.News.TwitterUrl = URLUtils.DEFAULT_TWITTER_SOURCE;
            pNew.Launcher = LauncherManager.Default;

            PendingProfiles.Add(pNew);
            OnCollectionChanged();
            return pNew;
        }

        public bool RemoveProfile(IProfile profile) {
            bool result = false;
            if (PendingProfiles.Count > 1) {
                bool IsDefault = profile.Id == PendingDefaultProfile.Id;
                result = PendingProfiles.Remove(profile);
                if (result) {
                    OnCollectionChanged();
                    if (IsDefault) {
                        PendingDefaultProfile = PendingProfiles.First();
                    }
                }
            }
            return result;
        }

        private void ApplyChanges(ICollection<IProfile> profiles, int defaultProfileId) {
            this.Profiles.Clear();
            //Add clones of instances
            foreach (IProfile p in profiles) {
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