
using System;
using AdvancedLauncher.Management.Interfaces;
using DMOLibrary.Profiles;

namespace AdvancedLauncher.Management.Configuration {
    public class AeriaConfiguration : AbstractConfiguration {

        #region Common
        public override string LauncherExecutable {
            get {
                return "DMLauncher.exe";
            }
        }

        public override string LauncherPathRegKey {
            get {
                return "Software\\Aeria Games\\DMO";
            }
        }

        public override string LauncherPathRegVal {
            get {
                return "Path";
            }
        }

        public override string GameExecutable {
            get {
                return "GDMO.exe";
            }
        }

        public override string GamePathRegKey {
            get {
                return "Software\\Aeria Games\\DMO";
            }
        }

        public override string GamePathRegVal {
            get {
                return "Path";
            }
        }

        public override GameManager.GameType GameType {
            get {
                return GameManager.GameType.ADMO;
            }
        }

        public override bool IsLastSessionAvailable {
            get {
                return false;
            }
        }

        public override string PatchRemoteURL {
            get {
                return "http://patch.dmo.joymax.com/Aeria/GDMO{0}.zip";
            }
        }

        public override string VarsionLocalPath {
            get {
                return @"LauncherLib\vGDMO.ini";
            }
        }

        public override string VersionRemoteURL {
            get {
                return "http://patch.dmo.joymax.com/Aeria/PatchInfo_GDMO.ini";
            }
        }
        #endregion

        protected override DMOProfile CreateProfile() {
            return new DMOLibrary.Profiles.Aeria.DMOAeria();
        }
    }
}
