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

using System.Collections.ObjectModel;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Events.Proxy;

namespace AdvancedLauncher.SDK.Management {

    public interface IProfileManager : IManager {

        void Start();

        Profile DefaultProfile {
            get;
        }

        ObservableCollection<Profile> Profiles {
            get;
        }

        Profile CurrentProfile {
            get;
            set;
        }

        ObservableCollection<Profile> PendingProfiles {
            get;
            set;
        }

        Profile PendingDefaultProfile {
            get;
            set;
        }

        void RevertChanges();

        void ApplyChanges();

        Profile CreateProfile();

        bool RemoveProfile(Profile profile);

        #region Event Handlers

        void ProfileChangedProxy(EventProxy<BaseEventArgs> proxy, bool subscribe = true);

        event BaseEventHandler ProfileChanged;

        void CollectionChangedProxy(EventProxy<BaseEventArgs> proxy, bool subscribe = true);

        event BaseEventHandler CollectionChanged;

        void LockedChangedProxy(EventProxy<LockedEventArgs> proxy, bool subscribe = true);

        event LockedChangedHandler ProfileLocked;

        void OnProfileLocked(bool IsLocked);

        #endregion Event Handlers
    }
}