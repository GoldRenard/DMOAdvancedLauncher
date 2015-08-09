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
using System.Windows.Navigation;
using AdvancedLauncher.Tools;

namespace AdvancedLauncher.UI.Windows {

    public partial class About : AbstractWindowControl {

        public About() {
            InitializeComponent();
            UpdataVersionText();
            LanguageManager.LanguageChanged += (s, e) => {
                UpdataVersionText();
            };
        }

        private void UpdataVersionText() {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            VersionBlock.Text = string.Format("{0}: {1}.{2} (build {3})",
                LanguageManager.Model.About_Version, version.Major, version.Minor, version.Build);
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            URLUtils.OpenSite(e.Uri.AbsoluteUri);
        }
    }
}