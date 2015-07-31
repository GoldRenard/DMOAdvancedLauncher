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
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Controls;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Windows;
using AdvancedLauncher.UI.Windows;
using Ninject;

namespace AdvancedLauncher.Management {

    public class WindowManager : IWindowManager {

        private ConcurrentStack<IWindow> WindowStack {
            get;
            set;
        } = new ConcurrentStack<IWindow>();

        private IWindow CurrentWindow {
            get;
            set;
        }

        private MainWindow MainWindow {
            get;
            set;
        }

        [Inject]
        public Logger Logger {
            get; set;
        }

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get;
            set;
        }

        public void Initialize() {
            Splashscreen.ShowSplash();
            Splashscreen.SetProgress("Loading...");
            this.MainWindow = App.Kernel.Get<MainWindow>(); // do not inject it directly, we should not export it as public property
            Application.Current.MainWindow = MainWindow;
            Splashscreen.HideSplash();
            ShowWindow(new NewsWindow());
            MainWindow.Show();
        }

        public void ShowWindow(IWindow window) {
            if (window == null) {
                throw new ArgumentException("Window argument cannot be null");
            }
            UserControl control = window as UserControl;
            if (control == null) {
                throw new ArgumentException("Window must inherit from WPF UserControl");
            }
            if (CurrentWindow != null) {
                WindowStack.Push(CurrentWindow);
            }
            CurrentWindow = window;
            CurrentWindow.OnShow();
            MainWindow.transitionLayer.Content = control;
        }

        public void GoHome() {
            IWindow homeWindow = CurrentWindow;
            while (WindowStack.Count > 0) {
                WindowStack.TryPop(out homeWindow);
            }
            this.CurrentWindow = null;
            ShowWindow(homeWindow);
        }

        public void GoBack() {
            if (WindowStack.Count > 0) {
                IWindow previous;
                WindowStack.TryPop(out previous);
                this.CurrentWindow = null;
                ShowWindow(previous);
            }
        }

        public void GoBack(IWindow window) {
            if (window == null) {
                throw new ArgumentException("Window argument cannot be null");
            }
            if (window.Equals(CurrentWindow) && WindowStack.Count > 0) {
                WindowStack.TryPop(out window);
                this.CurrentWindow = null;
                ShowWindow(window);
            }
        }
    }
}