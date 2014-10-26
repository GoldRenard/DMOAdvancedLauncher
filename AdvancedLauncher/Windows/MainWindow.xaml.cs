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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Pages;
using System.Runtime.InteropServices;

namespace AdvancedLauncher.Windows {
    public partial class MainWindow : Window {
        private const int SC_CLOSE = 0xF060;
        private const int MF_ENABLED = 0x0000;
        private const int MF_GRAYED = 0x0001;
        private const int MF_DISABLED = 0x0002;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, int wEnable);
        IntPtr hWnd = IntPtr.Zero;

        bool IsCloseLocked = true;

        Gallery Gallery_tab = null;
        Personalization Personalization_tab = null;
        Community Community_tab = null;

        public MainWindow() {
            InitializeComponent();
            AdvancedLauncher.Service.UpdateChecker.Check();
            LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
            LauncherEnv.Settings.ProfileChanged += ReloadTabs;
            LauncherEnv.Settings.ProfileLocked += Settings_ProfileLocked;
            LauncherEnv.Settings.ClosingLocked += Settings_ClosingLocked;
            LauncherEnv.Settings.FileSystemLocked += Settings_FileSystemLocked;
            this.Closing += (s, e) => { e.Cancel = IsCloseLocked; };
            MainMenu.AboutClick += MainMenu_AboutClick;
            MainMenu.SettingsClick += MainMenu_SettingsClick;
            ReloadTabs();
            try { App.splash.Close(TimeSpan.FromSeconds(1)); } catch { }

#if DEBUG
            this.Title += " (development build " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
#endif
        }

        void Settings_FileSystemLocked(bool IsLocked) {
            if (IsLocked) {
                //Выбираем первую вкладку и отключаем персонализацию (на всякий случай)
                NavControl.SelectedIndex = 0;
                NavPersonalization.IsEnabled = false;
            } else {
                //Включаем персонализации обратно если игра определена
                if (LauncherEnv.Settings.CurrentProfile.GameEnv.CheckGame())
                    NavPersonalization.IsEnabled = true;
            }
        }

        void Settings_ClosingLocked(bool IsLocked) {
            if (hWnd == IntPtr.Zero)
                hWnd = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle;
            //Заблокировать закрытие окна
            IsCloseLocked = IsLocked;
            //Отключим кнопку "Х"
            EnableMenuItem(GetSystemMenu(hWnd, false), SC_CLOSE, IsLocked ? MF_DISABLED | MF_GRAYED : MF_ENABLED);
        }

        void Settings_ProfileLocked(bool IsLocked) {
            MainMenu.IsEnabled = !IsLocked;
        }

        public void ReloadTabs() {
            //Выбираем первую вкладку и отключаем все модули
            NavControl.SelectedIndex = 0;
            NavGallery.IsEnabled = false;
            NavCommunity.IsEnabled = false;
            NavPersonalization.IsEnabled = false;

            //Если доступен веб-профиль, включаем вкладку сообщества
            if (LauncherEnv.Settings.CurrentProfile.DMOProfile.IsWebAvailable)
                NavCommunity.IsEnabled = true;

            //Если путь до игры верен, включаем вкладку галереи и персонализации
            if (LauncherEnv.Settings.CurrentProfile.GameEnv.CheckGame()) {
                NavGallery.IsEnabled = true;
                NavPersonalization.IsEnabled = true;
            }
        }

        int cTabIndex = 0;
        private void NavControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e) {
            //Prevent handling over changing inside tab item
            if (cTabIndex == NavControl.SelectedIndex)
                return;

            switch (NavControl.SelectedIndex) {
                case 0: {
                        MainPage.Activate();
                        break;
                    }
                case 1: {
                        if (Gallery_tab == null) {
                            Gallery_tab = new Gallery();
                            NavGallery.Content = Gallery_tab;
                        }
                        Gallery_tab.Activate();
                        break;
                    }
                case 2: {
                        if (Community_tab == null) {
                            Community_tab = new Community();
                            NavCommunity.Content = Community_tab;
                        }
                        Community_tab.Activate();
                        break;
                    }
                case 3: {
                        if (Personalization_tab == null) {
                            Personalization_tab = new Personalization();
                            NavPersonalization.Content = Personalization_tab;
                        }
                        Personalization_tab.Activate();
                        break;
                    }
            }

            cTabIndex = NavControl.SelectedIndex;
            ((TabItem)NavControl.Items[cTabIndex]).Focus();
        }

        private Settings SettingsWindow = null;
        void MainMenu_SettingsClick(object sender, RoutedEventArgs e) {
            if (SettingsWindow == null) {
                SettingsWindow = new Settings();
                LayoutRoot.Children.Add(SettingsWindow);
            }
            SettingsWindow.Show(true);
        }

        private About AboutWindow = null;
        void MainMenu_AboutClick(object sender, RoutedEventArgs e) {
            if (AboutWindow == null) {
                AboutWindow = new About();
                LayoutRoot.Children.Add(AboutWindow);
            }
            AboutWindow.Show(true);
        }
    }
}
