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
using AdvancedLauncher.Model.Config;
using AdvancedLauncher.Model.Events;
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.Management.Interfaces {

    public interface IProfileManager : IManager {

        Profile DefaultProfile {
            set; get;
        }

        List<Profile> Profiles {
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

        void AddProfile();

        void RemoveProfile(Profile profile);

        event EventHandler ProfileChanged;

        event EventHandler CollectionChanged;

        event LockedChangedHandler ProfileLocked;

        void OnProfileLocked(bool IsLocked);
    }
}