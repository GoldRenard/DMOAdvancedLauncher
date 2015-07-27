using AdvancedLauncher.Management.Interfaces;
using DMOLibrary.Profiles;
using Microsoft.Win32;

namespace AdvancedLauncher.Management.Configuration {

    public abstract class AbstractConfiguration : IGameConfiguration {
        private DMOProfile _Profile;

        public abstract string GameExecutable {
            get;
        }

        public abstract string GamePathRegKey {
            get;
        }

        public abstract string GamePathRegVal {
            get;
        }

        public abstract GameManager.GameType GameType {
            get;
        }

        public abstract bool IsLastSessionAvailable {
            get;
        }

        public abstract string LauncherExecutable {
            get;
        }

        public abstract string LauncherPathRegKey {
            get;
        }

        public abstract string LauncherPathRegVal {
            get;
        }

        public abstract string PatchRemoteURL {
            get;
        }

        public abstract string VarsionLocalPath {
            get;
        }

        public abstract string VersionRemoteURL {
            get;
        }

        public DMOProfile Profile {
            get {
                if (_Profile == null) {
                    _Profile = CreateProfile();
                }
                return _Profile;
            }
        }

        protected abstract DMOProfile CreateProfile();

        public string GetGamePathFromRegistry() {
            using (RegistryKey reg = Registry.CurrentUser.CreateSubKey(GamePathRegKey)) {
                return (string)reg.GetValue(GamePathRegVal);
            }
        }

        public string GetLauncherPathFromRegistry() {
            using (RegistryKey reg = Registry.CurrentUser.CreateSubKey(LauncherPathRegKey)) {
                return (string)reg.GetValue(LauncherPathRegVal);
            }
        }
    }
}