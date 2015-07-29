﻿// ======================================================================
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
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.Management {

    public interface IProfileManager : IManager {

        IProfile DefaultProfile {
            set; get;
        }

        List<IProfile> Profiles {
            get;
        }

        IProfile CurrentProfile {
            get;
            set;
        }

        ObservableCollection<IProfile> PendingProfiles {
            get;
            set;
        }

        IProfile PendingDefaultProfile {
            get;
            set;
        }

        void RevertChanges();

        void ApplyChanges();

        void AddProfile();

        void RemoveProfile(IProfile profile);

        event EventHandler ProfileChanged;

        event EventHandler CollectionChanged;

        event LockedChangedHandler ProfileLocked;

        void OnProfileLocked(bool IsLocked);
    }
}