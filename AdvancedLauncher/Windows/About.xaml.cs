// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Windows.Navigation;
using AdvancedLauncher.Service;

namespace AdvancedLauncher.Windows {

    public partial class About : AbstractWindow {
        private const string LICENSE_FILE = "Docs\\LICENSE.txt";

        protected override void InitializeAbstractWindow() {
            InitializeComponent();
        }

        public About() {
            if (File.Exists(LICENSE_FILE)) {
                Licence.Text = File.ReadAllText(LICENSE_FILE);
            } else {
                Licence.Text = string.Format(AdvancedLauncher.Environment.LanguageEnv.Strings.About_Licence404, LICENSE_FILE);
            }
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            Utils.OpenSite(e.Uri.AbsoluteUri);
        }
    }
}