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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Windows;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.UI.Commands;
using AdvancedLauncher.UI.Windows;
using Ninject;

namespace AdvancedLauncher.Management {

    public class WindowManager : IWindowManager {
        private bool MainMenuSeparatorAdded = false;

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

        public ObservableCollection<SDK.Management.Windows.MenuItem> MenuItems {
            get;
            private set;
        } = new ObservableCollection<SDK.Management.Windows.MenuItem>();

        [Inject]
        public Logger Logger {
            get; set;
        }

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get;
            set;
        }

        [Inject]
        public ILanguageManager LanguageManager {
            get;
            set;
        }

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        public void Initialize() {
            this.MainWindow = App.Kernel.Get<MainWindow>(); // do not inject it directly, we should not export it as public property
            Application.Current.MainWindow = MainWindow;
            ShowWindow(new NewsWindow());
            BuildMenu();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                App.Kernel.Get<Splashscreen>().Close();
                MainWindow.Show();
            }
        }

        private void OnProfileLocked(object sender, LockedEventArgs e) {
            foreach (SDK.Management.Windows.MenuItem item in MenuItems) {
                item.NotifyEnabled();
            }
        }

        private void BuildMenu() {
            MenuItems.Add(new SDK.Management.Windows.MenuItem(LanguageManager, "Settings", FindResource<Canvas>("appbar_settings"), new Thickness(5, 5, 5, 5), new ModelCommand((p) => {
                MainWindow.SettingsFlyout.Width = MainWindow.ProfileSwitcher.ActualWidth + MainWindow.FLYOUT_WIDTH_MIN;
                MainWindow.SettingsFlyout.IsOpen = true;
            })));
            MenuItems.Add(new SDK.Management.Windows.MenuItem(LanguageManager, "Console", FindResource<Canvas>("appbar_app"), new Thickness(5, 7, 5, 7), new ModelCommand((p) => {
                ShowWindow(Logger);
                MainWindow.MenuFlyout.IsOpen = false;
            })));
            MenuItems.Add(new SDK.Management.Windows.MenuItem(LanguageManager, "About", FindResource<Canvas>("appbar_information"), new Thickness(9, 4, 9, 4), new ModelCommand((p) => {
                ShowWindow(App.Kernel.Get<About>());
                MainWindow.MenuFlyout.IsOpen = false;
            })));
        }

        public void ShowWindow(IWindow window) {
            if (window == null) {
                throw new ArgumentException("Window argument cannot be null");
            }
            Control control = window as Control;
            if (control == null) {
                throw new ArgumentException("Window must inherit from WPF Control");
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

        public void AddMenuItem(SDK.Management.Windows.MenuItem menuItem) {
            if (!MainMenuSeparatorAdded) {
                MenuItems.Add(SDK.Management.Windows.MenuItem.Separator);
                MainMenuSeparatorAdded = true;
            }
            MenuItems.Add(menuItem);
        }

        public bool RemoveMenuItem(SDK.Management.Windows.MenuItem menuItem) {
            return MenuItems.Remove(menuItem);
        }

        public T FindResource<T>(string name) {
            return (T)MainWindow.FindResource(name);
        }
    }
}