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

using AdvancedLauncher.Providers.Korea;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Model.Web;
using Ninject;

namespace AdvancedLauncher.Management.Configuration {

    public class KDMODMConfiguration : AbstractConfiguration {

        [Inject]
        public ILogManager LogManager {
            get; set;
        }

        #region Common

        public override string GameType {
            get {
                return "KDMO_DM";
            }
        }

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

        public override string VersionLocalPath {
            get {
                return @"LauncherLib\vDMO.ini";
            }
        }

        public override string VersionRemoteURL {
            get {
                return "http://digimonmasters.nowcdn.co.kr/s1/PatchInfo.ini";
            }
        }

        public override string ConvertGameStartArgs(string args) {
            return args.Replace(" 1 ", " ");
        }

        public override string ConvertLauncherStartArgs(string args) {
            return args;
        }

        #endregion Common

        #region Providers

        public override ILoginProvider CreateLoginProvider() {
            return new KoreaLoginProvider(LogManager);
        }

        public override IWebProvider CreateWebProvider() {
            return new KoreaWebProvider(LogManager);
        }

        protected override IServersProvider CreateServersProvider() {
            return new KoreaServersProvider();
        }

        public override bool IsLoginRequired {
            get {
                return true;
            }
        }

        public override bool IsWebAvailable {
            get {
                return true;
            }
        }

        #endregion Providers
    }
}