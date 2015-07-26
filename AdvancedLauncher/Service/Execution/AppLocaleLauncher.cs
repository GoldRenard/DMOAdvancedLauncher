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

using System.Globalization;
using System.IO;

namespace AdvancedLauncher.Service.Execution {

    /// <summary>
    /// AppLocale application launcher
    /// </summary>
    public class AppLocaleLauncher : SteamSensitiveLauncher {

        /// <summary>
        /// Name of this launcher
        /// </summary>
        public override string Name {
            get {
                return "AppLocale";
            }
        }

        public static bool IsInstalled {
            get {
                string appLocalePath = System.Environment.GetEnvironmentVariable("windir") + "\\apppatch\\AppLoc.exe";
                return File.Exists(appLocalePath);
            }
        }

        public static bool IsKoreanSupported {
            get {
                foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures))
                    if (ci.TwoLetterISOLanguageName == "ko") {
                        return true;
                    }
                return false;
            }
        }

        /// <summary>
        /// Is AppLocale supported in the envronment
        /// </summary>
        public override bool IsSupported {
            get {
                return IsInstalled && IsKoreanSupported;
            }
        }

        /// <summary>
        /// Execute process with arguments
        /// </summary>
        /// <param name="application">Path to executable</param>
        /// <param name="arguments">Arguments</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        public override bool ExecuteInternal(string application, string arguments) {
            string appLocalePath = System.Environment.GetEnvironmentVariable("windir") + "\\apppatch\\AppLoc.exe";
            if (!string.IsNullOrEmpty(arguments)) {
                return StartProcess(appLocalePath, "\"" + application + "\" \"" + arguments + "\" \"/L0412\"");
            } else {
                return StartProcess(appLocalePath, "\"" + application + "\" \"/L0412\"");
            }
        }
    }
}