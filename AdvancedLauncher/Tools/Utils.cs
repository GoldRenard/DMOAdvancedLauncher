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

using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AdvancedLauncher.Tools {

    public static class Utils {
        private const string RUS_TWITTER = "dmo_russian";

        private const string ROW_TWITTER = "DMOWiki";

        private static Dictionary<CultureInfo, string> DefaultTwitter = new Dictionary<CultureInfo, string>();

        static Utils() {
            DefaultTwitter.Add(CultureInfo.GetCultureInfo("ru"), RUS_TWITTER);
            DefaultTwitter.Add(CultureInfo.GetCultureInfo("ru-RU"), RUS_TWITTER);
            DefaultTwitter.Add(CultureInfo.GetCultureInfo("uk"), RUS_TWITTER);
            DefaultTwitter.Add(CultureInfo.GetCultureInfo("uk-UA"), RUS_TWITTER);
        }

        /// <summary>
        /// Returns default Twitter user for current CultureInfo
        /// </summary>
        /// <returns>Default Twitter user for current CultureInfo</returns>
        public static string GetDefaultTwitter() {
            string twitter = null;
            if (DefaultTwitter.TryGetValue(CultureInfo.CurrentUICulture, out twitter)) {
                return twitter;
            }
            return ROW_TWITTER;
        }

        /// <summary> Checks access to file </summary>
        /// <param name="file">Full path to file</param>
        /// <returns> <see langword="True"/> if file is locked </returns>
        public static bool IsFileLocked(string file) {
            FileStream stream = null;

            try {
                stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            } catch (IOException) {
                return true;
            } finally {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}