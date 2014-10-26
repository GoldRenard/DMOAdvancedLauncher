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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using AdvancedLauncher.Environment;

namespace AdvancedLauncher.Pages {
    public partial class MainPage : UserControl {
        Storyboard ShowWindow;
        private delegate void DoChangeTextNBool(string text, bool bool_);

        public MainPage() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
                NewsBlock_.TabChanged += NewsTabChanged;
                Twitter.Click += NewsBlock_.OnShowTwitter;
                Joymax.Click += NewsBlock_.OnShowJoymax;
                LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
                LauncherEnv.Settings.ProfileChanged += ProfileChanged;
                ProfileChanged();
            }
        }

        public void Activate() {
            ShowWindow.Begin();
        }

        void ProfileChanged() {
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
