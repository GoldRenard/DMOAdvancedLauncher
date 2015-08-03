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

using AdvancedLauncher.Providers.Joymax;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Model.Web;
using Ninject;

namespace AdvancedLauncher.Management.Configuration {

    public class JoymaxConfiguration : AbstractConfiguration {

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
                return "Joymax";
            }
        }

        public override string GameType {
            get {
                return "GDMO";
            }
        }

        public override string LauncherExecutable {
            get {
                return "DMLauncher.exe";
            }
        }

        public override string LauncherPathRegKey {
            get {
                return "Software\\Joymax\\DMO";
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
                return "Software\\Joymax\\DMO";
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
                return "http://patch.dmo.joymax.com/GDMO{0}.zip";
            }
        }

        public override string VersionLocalPath {
            get {
                return @"LauncherLib\vGDMO.ini";
            }
        }

        public override string VersionRemoteURL {
            get {
                return "http://patch.dmo.joymax.com/PatchInfo_GDMO.ini";
            }
        }

        public override string ConvertGameStartArgs(string args) {
            return "true";
        }

        #endregion Common

        #region Providers

        public override IWebProvider CreateWebProvider() {
            return new JoymaxWebProvider(DatabaseManager, LogManager);
        }

        public override INewsProvider CreateNewsProvider() {
            return new JoymaxNewsProvider(LogManager);
        }

        protected override IServersProvider CreateServersProvider() {
            return new JoymaxServersProvider(DatabaseManager);
        }

        public override bool IsLoginRequired {
            get {
                return false;
            }
        }

        public override bool IsWebAvailable {
            get {
                return true;
            }
        }

        public override bool IsNewsAvailable {
            get {
                return true;
            }
        }

        #endregion Providers
    }
}