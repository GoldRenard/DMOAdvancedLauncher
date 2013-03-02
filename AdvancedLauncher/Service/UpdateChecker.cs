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
using System.Net;
using System.ComponentModel;
using System.Windows;

namespace AdvancedLauncher
{
    public class UpdateChecker
    {
        public static void Check()
        {
            BackgroundWorker bw_checkupdates = new BackgroundWorker();
            bw_checkupdates.DoWork += (s1, e2) =>
            {
                string[] res_arr = null;

                WebClient client = new WebClient();
                client.Proxy = (IWebProxy)null;

                try
                {
                    string result = client.DownloadString(new Uri("http://renamon.ru/launcher/check_updates.php"));
                    res_arr = result.Split('|');
                }
                catch {
                    return;
                }

                if (res_arr != null)
                    if (res_arr.Length == 3)
                    {
                        Version current_version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                        Version remote_version = new Version(res_arr[0]);
                        if (remote_version.CompareTo(current_version) > 0)
                        {
                            if (MessageBoxResult.Yes == MessageBox.Show(string.Format(LanguageProvider.strings.LAUNCHER_UPDATE_NEW_AVAILABLE, res_arr[0]) + Environment.NewLine + res_arr[2] +
                                Environment.NewLine + LanguageProvider.strings.LAUNCHER_Q_UPDATE_WANT_DOWNLOAD, string.Format(LanguageProvider.strings.LAUNCHER_UPDATE_NEW_AVAILABLE_CAPTION, res_arr[0]), MessageBoxButton.YesNo, MessageBoxImage.Question))
                                Utils.OpenSite(res_arr[1]);
                        }
                    }
            };

            bw_checkupdates.RunWorkerAsync();
        }
    }
}
