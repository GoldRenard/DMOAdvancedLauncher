// ======================================================================
// GLOBAL DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
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
using System.IO;
using System.Runtime.InteropServices;

namespace AdvancedLauncher
{
    class Utils
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary> Parse version file (like vGDMO.ini) </summary>
        /// <param name="text">Version file content</param>
        /// <returns> Version (integer) or -1 if version not found </returns>
        public static int GetVersion(string text)
        {
            string expr = "(version)(=)(\\d+)";
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(expr, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            System.Text.RegularExpressions.Match m = r.Match(text);
            if (m.Success)
                return Convert.ToInt32(m.Groups[3].ToString());
            return -1;
        }

        /// <summary> Error MessageBox </summary>
        /// <param name="text">Content of error</param>
        public static void MSG_ERROR(string text)
        {
                MessageBox.Show(text, LanguageProvider.strings.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary> Opens URL with default browser </summary>
        /// <param name="url">URL to web</param>
        public static void OpenSite(string url)
        {
            try { System.Diagnostics.Process.Start(System.Web.HttpUtility.UrlDecode(url)); }
            catch (Exception ex) {
                MSG_ERROR(LanguageProvider.strings.CANT_OPEN_LINK + ex.Message);
            }
        }

        /// <summary> Checks access to game resources. Checks for working game, default launcher and locked game resources </summary>
        /// <returns> <see langword="True"/> if game resources are accessible </returns>
        public static bool CheckUpdateAccess()
        {
            if (FindWindow("DMO", null) != IntPtr.Zero)
            {
                Utils.MSG_ERROR(LanguageProvider.strings.NEED_CLOSE_GAME);
                return false;
            }
            if (FindWindow(null, "Digimon Masters Launcher") != IntPtr.Zero)
            {
                Utils.MSG_ERROR(LanguageProvider.strings.NEED_CLOSE_DEF_LAUNCHER);
                return false;
            }
            if (IsFileLocked(SettingsProvider.GAME_PATH() + SettingsProvider.PACK_HF_FILE)
                || IsFileLocked(SettingsProvider.GAME_PATH() + SettingsProvider.PACK_PF_FILE))
            {
                Utils.MSG_ERROR(LanguageProvider.strings.GAME_FILES_IN_USE);
                return false;
            }
            return true;
        }

        /// <summary> Checks access to file </summary>
        /// <param name="file">Full path to file</param>
        /// <returns> <see langword="True"/> if file is locked </returns>
        public static bool IsFileLocked(string file)
        {
            FileStream stream = null;

            try { stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None); }
            catch (IOException) { return true; }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}
