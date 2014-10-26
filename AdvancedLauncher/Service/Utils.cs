// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

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

using System;
using System.Windows;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using AdvancedLauncher.Environment;

namespace AdvancedLauncher.Service {
    public static class Utils {
        /// <summary> Error MessageBox </summary>
        /// <param name="text">Content of error</param>
        public static void MSG_ERROR(string text) {
            MessageBox.Show(text, LanguageEnv.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary> Opens URL with default browser </summary>
        /// <param name="url">URL to web</param>
        public static void OpenSite(string url) {
            try { System.Diagnostics.Process.Start(System.Web.HttpUtility.UrlDecode(url)); } catch (Exception ex) {
                MSG_ERROR(LanguageEnv.Strings.CantOpenLink + ex.Message);
            }
        }

        /// <summary> Opens URL with default browser (without URL decode) </summary>
        /// <param name="url">URL to web</param>
        public static void OpenSiteNoDecode(string url) {
            try { System.Diagnostics.Process.Start(url); } catch (Exception ex) {
                MSG_ERROR(LanguageEnv.Strings.CantOpenLink + ex.Message);
            }
        }

        /// <summary> Checks access to file </summary>
        /// <param name="file">Full path to file</param>
        /// <returns> <see langword="True"/> if file is locked </returns>
        public static bool IsFileLocked(string file) {
            FileStream stream = null;

            try { stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None); } catch (IOException) { return true; } finally {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}