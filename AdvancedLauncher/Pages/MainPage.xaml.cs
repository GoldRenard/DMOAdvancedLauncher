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

using System.Windows;
using AdvancedLauncher.Environment;
using log4net;

namespace AdvancedLauncher.Pages {

    public partial class MainPage : AbstractPage {
        public static readonly ILog LOGGER = LogManager.GetLogger(typeof(MainPage));

        private delegate void DoChangeTextNBool(string text, bool bool_);

        protected override void InitializeAbstractPage() {
            InitializeComponent();
        }

        public MainPage() {
            NewsBlock_.TabChanged += NewsTabChanged;
            Twitter.Click += NewsBlock_.OnShowTwitter;
            Joymax.Click += NewsBlock_.OnShowJoymax;
        }

        protected override void ProfileChanged() {
            if (LauncherEnv.Settings.CurrentProfile.DMOProfile.IsNewsAvailable) {
                Joymax.Visibility = Visibility.Visible;
                NewsTabChanged(this, LauncherEnv.Settings.CurrentProfile.News.FirstTab);
            } else {
                Joymax.Visibility = Visibility.Collapsed;
                NewsTabChanged(this, 0);
            }
        }

        private void NewsTabChanged(object sender, byte tab) {
            LauncherEnv.Settings.CurrentProfile.News.FirstTab = tab;
            if (tab == 0) {
                Twitter.IsEnabled = false;
                Joymax.IsEnabled = true;
            } else {
                Twitter.IsEnabled = true;
                Joymax.IsEnabled = false;
            }
        }
    }
}