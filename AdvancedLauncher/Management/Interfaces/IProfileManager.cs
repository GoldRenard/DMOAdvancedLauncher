using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AdvancedLauncher.Model.Config;

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