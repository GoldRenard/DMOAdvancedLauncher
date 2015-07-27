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
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Model.Config;
using AdvancedLauncher.Tools;
using AdvancedLauncher.UI.Extension;
using Ninject;

namespace AdvancedLauncher.Management {

    public class UpdateManager : IUpdateManager {

        [Inject]
        public ILanguageManager LanguageManager {
            get; set;
        }

        public void CheckUpdates() {
            BackgroundWorker updateWorker = new BackgroundWorker();
            updateWorker.DoWork += async (s1, e2) => {
                RemoteVersion remote = RemoteVersion.Instance;
                if (remote != null) {
                    Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    if (remote.Version.CompareTo(currentVersion) > 0) {
                        string content = string.Format(LanguageManager.Model.UpdateAvailableText, remote.Version)
                            + System.Environment.NewLine
                            + System.Environment.NewLine
                            + remote.ChangeLog
                            + System.Environment.NewLine
                            + System.Environment.NewLine
                            + LanguageManager.Model.UpdateDownloadQuestion;
                        string caption = string.Format(LanguageManager.Model.UpdateAvailableCaption, remote.Version);
                        if (await DialogsHelper.ShowYesNoDialog(caption, content)) {
                            URLUtils.OpenSite(remote.DownloadUrl);
                        }
                    }
                }
            };
            updateWorker.RunWorkerAsync();
        }

        public void Initialize() {
            // nothing to do here
        }
    }
}