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
using System.Windows.Threading;
using AdvancedLauncher.Service;
using MahApps.Metro.Controls;

namespace AdvancedLauncher.Windows {

    public partial class Splashscreen : MetroWindow {

        public static Splashscreen Instance {
            get;
            private set;
        }

        public Splashscreen() {
            Instance = this;
            InitializeComponent();
        }

        public static void ShowSplash() {
            new Splashscreen().Show();
            // let the window render
            DispatcherHelper.DoEvents();
        }

        public static void HideSplash() {
            Instance.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() => {
                    Instance.Close();
                })
            );
        }

        public static void SetProgress(string title) {
            while (Instance == null) {
                DispatcherHelper.DoEvents();
            }
            Instance.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action<string>((t) => {
                    Instance.Title = t;
                    DispatcherHelper.DoEvents();
                }), title);
        }
    }
}