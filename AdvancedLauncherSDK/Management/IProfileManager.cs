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

    /// <summary>
    /// Profile manager API interface
    /// </summary>
    /// <seealso cref="Profile"/>
    public interface IProfileManager : IManager {

        /// <summary>
        /// Gets default profile on application start
        /// </summary>
        Profile DefaultProfile {
            get;
        }

        /// <summary>
        /// Gets current profiles collection
        /// </summary>
        ObservableCollection<Profile> Profiles {
            get;
        }

        /// <summary>
        /// Gets or sets current selected profile
        /// </summary>
        Profile CurrentProfile {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets profile collection with pending changes.
        /// </summary>
        ObservableCollection<Profile> PendingProfiles {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets pending default profile.
        /// </summary>
        Profile PendingDefaultProfile {
            get;
            set;
        }

        /// <summary>
        /// Resets <see cref="PendingProfiles"/> and <see cref="PendingDefaultProfile"/>
        /// </summary>
        void RevertChanges();

        /// <summary>
        /// Applies changes from <see cref="PendingProfiles"/> and <see cref="PendingDefaultProfile"/>
        /// to <see cref="Profiles"/> and <see cref="DefaultProfile"/>.
        /// </summary>
        void ApplyChanges();

        /// <summary>
        /// Creates new profile in <see cref="PendingProfiles"/> collection
        /// </summary>
        /// <returns>Created profile</returns>
        Profile CreateProfile();

        /// <summary>
        /// Removes specified profile from <see cref="PendingProfiles"/> collection.
        /// </summary>
        /// <param name="profile">Profile to remove</param>
        /// <returns><b>True</b> on success</returns>
        bool RemoveProfile(Profile profile);

        #region Event Handlers

        /// <summary>
        /// Registers new event listener for profile changing event.
        /// </summary>
        /// <param name="proxy"><see cref="EventProxy{T}"/> instance</param>
        /// <param name="subscribe"><b>True</b> if you want to subscribe, <b>false</b> otherwise.</param>
        void ProfileChangedProxy(EventProxy<BaseEventArgs> proxy, bool subscribe = true);

        /// <summary>
        /// Current profile changing event. You should not use this directly, is doesn't work correctly for
        /// cross-domain transparent proxy instances.
        /// Use <see cref="ProfileChangedProxy(EventProxy{BaseEventArgs}, bool)"/> instead.
        /// </summary>
        event BaseEventHandler ProfileChanged;

        /// <summary>
        /// Registers new event listener for <see cref="Profiles"/> collection changing event.
        /// </summary>
        /// <param name="proxy"><see cref="EventProxy{T}"/> instance</param>
        /// <param name="subscribe"><b>True</b> if you want to subscribe, <b>false</b> otherwise.</param>
        void CollectionChangedProxy(EventProxy<BaseEventArgs> proxy, bool subscribe = true);

        /// <summary>
        /// <see cref="Profiles"/> collection changing event. You should not use this directly, is doesn't work correctly for
        /// cross-domain transparent proxy instances.
        /// Use <see cref="CollectionChangedProxy(EventProxy{BaseEventArgs}, bool)"/> instead.
        /// </summary>
        event BaseEventHandler CollectionChanged;

        /// <summary>
        /// Registers new event listener for profile change locking event.
        /// </summary>
        /// <param name="proxy"><see cref="EventProxy{T}"/> instance</param>
        /// <param name="subscribe"><b>True</b> if you want to subscribe, <b>false</b> otherwise.</param>
        void LockedChangedProxy(EventProxy<LockedEventArgs> proxy, bool subscribe = true);

        /// <summary>
        /// Profile change locking event. You should not use this directly, is doesn't work correctly for
        /// cross-domain transparent proxy instances.
        /// Use <see cref="LockedChangedProxy(EventProxy{LockedEventArgs}, bool)"/> instead.
        /// </summary>
        event LockedChangedHandler ProfileLocked;

        /// <summary>
        /// Fires <see cref="ProfileLocked"/>
        /// </summary>
        /// <param name="IsLocked">Should it be locked or not</param>
        void OnProfileLocked(bool IsLocked);

        #endregion Event Handlers
    }
}