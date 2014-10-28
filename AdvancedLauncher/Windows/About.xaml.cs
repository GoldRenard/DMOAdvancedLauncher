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

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Service;

namespace AdvancedLauncher.Windows {
    public partial class About : UserControl {
        private Storyboard ShowWindow, HideWindow;
        private const string LICENSE_FILE = "Docs\\LICENSE.txt";
        public About() {
            InitializeComponent();
            LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
            ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
            HideWindow = ((Storyboard)this.FindResource("HideWindow"));
            LoadLicence();
        }

        public void LoadLicence() {
            if (File.Exists(LICENSE_FILE)) {
                Licence.Text = File.ReadAllText(LICENSE_FILE);
            } else {
                Licence.Text = string.Format(AdvancedLauncher.Environment.LanguageEnv.Strings.About_Licence404, LICENSE_FILE);
            }
        }

        public void Show(bool state) {
            if (state) {
                ShowWindow.Begin();
            } else {
                HideWindow.Begin();
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e) {
            Show(false);
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            Utils.OpenSite(e.Uri.AbsoluteUri);
        }
    }
}
