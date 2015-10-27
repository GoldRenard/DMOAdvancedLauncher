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

using System.IO;
using AdvancedLauncher.SDK.Management;
using Ninject;

namespace AdvancedLauncher.Management.Execution {

    /// <summary>
    /// LocaleEmulator application launcher
    /// </summary>
    public class LELauncher : SteamSensitiveLauncher {

        private const string CONFIG_FORMAT = @"<?xml version=""1.0"" encoding=""utf-8""?>
<LEConfig>
  <Profiles>
    <Profile Name = ""DMO"" Guid=""c34766ff-d847-4f6c-a986-4f0e3c37a852"" MainMenu=""false"">
      <Parameter>{0}</Parameter>
      <Location>ko-KR</Location>
      <Timezone>Tokyo Standard Time</Timezone>
      <RunAsAdmin>true</RunAsAdmin>
      <RedirectRegistry>true</RedirectRegistry>
      <RunWithSuspend>false</RunWithSuspend>
    </Profile>
  </Profiles>
</LEConfig>";

        private const string CONFIG_FILE = "LEConfig.xml";

        private const string RUN_PARAMS = "-runas c34766ff-d847-4f6c-a986-4f0e3c37a852 \"{0}\"";

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get; set;
        }

        /// <summary>
        /// Name of this launcher
        /// </summary>
        public override string Name {
            get {
                return "Locale Emulator";
            }
        }

        /// <summary>
        /// Is NTLEA supported in the envronment
        /// </summary>
        public override bool IsSupported {
            get {
                return File.Exists(EnvironmentManager.LEFile);
            }
        }

        /// <summary>
        /// Execute process with arguments
        /// </summary>
        /// <param name="application">Path to executable</param>
        /// <param name="arguments">Arguments</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        public override bool ExecuteInternal(string application, string arguments) {
            File.WriteAllText(Path.Combine(EnvironmentManager.AppPath, CONFIG_FILE), string.Format(CONFIG_FORMAT, arguments));
            return StartProcess(EnvironmentManager.LEFile, string.Format(RUN_PARAMS, application));
        }
    }
}