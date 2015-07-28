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

using AdvancedLauncher.SDK.Model.Web;
using Microsoft.Win32;

namespace AdvancedLauncher.SDK.Management.Configuration {

    public abstract class AbstractConfiguration : IGameConfiguration {
        private IGameProfile _Profile;

        public abstract string GameExecutable {
            get;
        }

        public abstract string GamePathRegKey {
            get;
        }

        public abstract string GamePathRegVal {
            get;
        }

        public abstract string GameType {
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

        public abstract string VersionLocalPath {
            get;
        }

        public abstract string VersionRemoteURL {
            get;
        }

        public IGameProfile Profile {
            get {
                if (_Profile == null) {
                    _Profile = CreateProfile();
                }
                return _Profile;
            }
        }

        protected abstract IGameProfile CreateProfile();

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