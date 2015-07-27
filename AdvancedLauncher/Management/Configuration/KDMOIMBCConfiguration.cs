
using System;
using AdvancedLauncher.Management.Interfaces;
using DMOLibrary.Profiles;

namespace AdvancedLauncher.Management.Configuration {
    public class KDMOIMBCConfiguration : KDMODMConfiguration {

        public new GameManager.GameType GameType {
            get {
                return GameManager.GameType.KDMO_IMBC;
            }
        }

        protected new DMOProfile CreateProfile() {
            return new DMOLibrary.Profiles.Korea.DMOKoreaIMBC();
        }
    }
}
