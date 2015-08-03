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

using AdvancedLauncher.Providers.Aeria;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Model.Web;
using Ninject;

namespace AdvancedLauncher.Management.Configuration {

    public class AeriaConfiguration : AbstractConfiguration {

        [Inject]
        public ILogManager LogManager {
            get; set;
        }

        [Inject]
        public IDatabaseManager DatabaseManager {
            get; set;
        }

        #region Common

        public override string Name {
            get {
                return "Aeria Games";
            }
        }

        public override string GameType {
            get {
                return "ADMO";
            }
        }

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

        public override string VersionLocalPath {
            get {
                return @"LauncherLib\vGDMO.ini";
            }
        }

        public override string VersionRemoteURL {
            get {
                return "http://patch.dmo.joymax.com/Aeria/PatchInfo_GDMO.ini";
            }
        }

        #endregion Common

        #region Providers

        public override ILoginProvider CreateLoginProvider() {
            return new AeriaLoginProvider(LogManager);
        }

        protected override IServersProvider CreateServersProvider() {
            return new AeriaServersProvider(DatabaseManager);
        }

        public override bool IsLoginRequired {
            get {
                return true;
            }
        }

        #endregion Providers
    }
}