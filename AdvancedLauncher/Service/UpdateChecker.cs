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
using AdvancedLauncher.Environment;
using AdvancedLauncher.Environment.Containers;

namespace AdvancedLauncher.Service {

    public static class UpdateChecker {

        public static void Check() {
            BackgroundWorker updateWorker = new BackgroundWorker();
            updateWorker.DoWork += async (s1, e2) => {
                RemoteVersion remote = RemoteVersion.Instance;
                if (remote != null) {
                    Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    if (remote.Version.CompareTo(currentVersion) > 0) {
                        string content = string.Format(LanguageEnv.Strings.UpdateAvailableText, remote.Version)
                            + System.Environment.NewLine
                            + System.Environment.NewLine
                            + remote.ChangeLog
                            + System.Environment.NewLine
                            + System.Environment.NewLine
                            + LanguageEnv.Strings.UpdateDownloadQuestion;
                        string caption = string.Format(LanguageEnv.Strings.UpdateAvailableCaption, remote.Version);
                        if (await Utils.ShowYesNoDialog(caption, content)) {
                            Utils.OpenSite(remote.DownloadUrl);
                        }
                    }
                }
            };
            updateWorker.RunWorkerAsync();
        }
    }
}