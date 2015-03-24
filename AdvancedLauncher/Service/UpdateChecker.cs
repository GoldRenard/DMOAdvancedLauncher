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

using System;
using System.ComponentModel;
using System.Net;
using AdvancedLauncher.Environment;
using DMOLibrary;

namespace AdvancedLauncher.Service {

    public static class UpdateChecker {

        public static void Check() {
            BackgroundWorker updateWorker = new BackgroundWorker();
            updateWorker.DoWork += async (s1, e2) => {
                string[] resArr = null;

                using (WebClient webClient = new WebClientEx()) {
                    try {
                        string result = webClient.DownloadString(new Uri(LauncherEnv.REMOTE_PATH + "check_updates.php"));
                        resArr = result.Split('|');
                    } catch {
                        return;
                    }
                }

                if (resArr != null)
                    if (resArr.Length == 3) {
                        Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                        Version remoteVersion = new Version(resArr[0]);
                        if (remoteVersion.CompareTo(currentVersion) > 0) {
                            string content = string.Format(LanguageEnv.Strings.UpdateAvailableText, resArr[0]) + System.Environment.NewLine + resArr[2] +
                                System.Environment.NewLine + LanguageEnv.Strings.UpdateDownloadQuestion;
                            string caption = string.Format(LanguageEnv.Strings.UpdateAvailableCaption, resArr[0]);
                            if (await Utils.ShowYesNoDialog(caption, content)) {
                                Utils.OpenSite(resArr[1]);
                            }
                        }
                    }
            };
            updateWorker.RunWorkerAsync();
        }
    }
}