
using System;
using AdvancedLauncher.Management.Interfaces;
using DMOLibrary.Profiles;

namespace AdvancedLauncher.Management.Configuration {
    public class KDMODMConfiguration : AbstractConfiguration {

        #region Common
        public override string LauncherExecutable {
            get {
                return "D-Player.exe";
            }
        }

        public override string LauncherPathRegKey {
            get {
                return "Software\\Digitalic\\Launcher";
            }
        }

        public override string LauncherPathRegVal {
            get {
                return "Launcher";
            }
        }

        public override string GameExecutable {
            get {
                return "DigimonMasters.exe";
            }
        }

        public override string GamePathRegKey {
            get {
                return "Software\\Digitalic\\DigimonMasters";
            }
        }

        public override string GamePathRegVal {
            get {
                return "Path";
            }
        }

        public override GameManager.GameType GameType {
            get {
                return GameManager.GameType.KDMO_DM;
            }
        }

        public override bool IsLastSessionAvailable {
            get {
                return true;
            }
        }

        public override string PatchRemoteURL {
            get {
                return "http://digimonmasters.nowcdn.co.kr/s1/{0}.zip";
            }
        }

        public override string VarsionLocalPath {
            get {
                return @"LauncherLib\vDMO.ini";
            }
        }

        public override string VersionRemoteURL {
            get {
                return "http://digimonmasters.nowcdn.co.kr/s1/PatchInfo.ini";
            }
        }

        #endregion

        protected override DMOProfile CreateProfile() {
            return new DMOLibrary.Profiles.Korea.DMOKorea();
        }
    }
}
