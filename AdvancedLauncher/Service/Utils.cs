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
using System.IO;

namespace AdvancedLauncher
{
  class Utils
  {
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
      catch (Exception ex)
      {
        MSG_ERROR(LanguageProvider.strings.CANT_OPEN_LINK + ex.Message);
      }
    }

    /// <summary> Opens URL with default browser (without URL decode) </summary>
    /// <param name="url">URL to web</param>
    public static void OpenSiteNoDecode(string url)
    {
        try { System.Diagnostics.Process.Start(url); }
        catch (Exception ex)
        {
            MSG_ERROR(LanguageProvider.strings.CANT_OPEN_LINK + ex.Message);
        }
    }

#if DEBUG
    static string DEBUG_FILE = string.Empty;
    public static void SetDebug(string file)
    {
      try { File.Delete(file); }
      catch { }
      DEBUG_FILE = file;
      WriteDebug("Starting new session.");
    }
    public static void WriteDebug(string message)
    {
      if (DEBUG_FILE != string.Empty)
      {
        using (StreamWriter sw = File.AppendText(DEBUG_FILE))
        {
          sw.Write("[" + DateTime.Now + "] ");
          sw.WriteLine(message);
        }
      }
    }
#endif
  }
}
