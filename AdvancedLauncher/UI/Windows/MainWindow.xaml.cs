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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.Tools;
using AdvancedLauncher.UI.Pages;
using MahApps.Metro.Controls;
using Ninject;

namespace AdvancedLauncher.UI.Windows {

    public partial class MainWindow : MetroWindow {
        public static int FLYOUT_WIDTH_MIN = 100;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(MainWindow));

        private IntPtr hWnd = IntPtr.Zero;

        private bool IsCloseLocked = true;

        private Settings SettingsWindow = null;
        private About AboutWindow = null;
        private AbstractPage currentTab;

        private static MainWindow _Instance;

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get; set;
        }

        [Inject]
        public Logger Logger {
            get; set;
        }

        [Inject]
        public IUpdateManager UpdateManager {
            get; set;
        }

        [Inject]
        public ILanguageManager LanguageManager {
            get; set;
        }

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        [Inject]
        public IConfigurationManager GameManager {
            get; set;
        }

        public static MainWindow Instance {
            get {
                return _Instance;
            }
        }

        public MainWindow() {
            App.Kernel.Inject(this);
            _Instance = this;
            Splashscreen.SetProgress("Loading...");
            Application.Current.MainWindow = _Instance;
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                RenderOptions.SetBitmapScalingMode(ProfileSwitcher, BitmapScalingMode.HighQuality);
                Logger.WindowClosed += (s, e1) => {
                    transitionLayer.Content = null;
                };
                UpdateManager.CheckUpdates();
                LanguageManager.LanguageChanged += (s, e) => {
                    this.DataContext = LanguageManager.Model;
                };
                ProfileSwitcher.DataContext = ProfileManager;
                ProfileManager.ProfileChanged += OnProfileChanged;
                EnvironmentManager.ClosingLocked += OnClosingLocked;
                EnvironmentManager.FileSystemLocked += OnFileSystemLocked;
                this.Closing += (s, e) => {
                    e.Cancel = IsCloseLocked;
                };
                this.MouseDown += MainWindow_MouseDown;

                MenuFlyout.AboutClick += OnAboutClick;
                MenuFlyout.ProfileSettingsClick += OnProfileSettingsClick;
                MenuFlyout.LoggerClick += OnLoggerClick;
                OnProfileChanged(this, EventArgs.Empty);
#if DEBUG
                this.Title += " (development build " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
#endif
                Splashscreen.HideSplash();
            }
        }

        private void MainWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Point p = e.GetPosition(this);
            if (MenuFlyout.IsOpen && this.Width - p.X > MenuFlyout.Width) {
                MenuFlyout.IsOpen = false;
            }
            if (SettingsFlyout.IsOpen && this.Width - p.X > SettingsFlyout.Width) {
                SettingsFlyout.IsOpen = false;
            }
        }

        private void OnFileSystemLocked(object sender, LockedEventArgs e) {
            if (e.IsLocked) {
                //Выбираем первую вкладку и отключаем персонализацию (на всякий случай)
                NavControl.SelectedIndex = 0;
                NavPersonalization.IsEnabled = false;
            } else {
                //Включаем персонализации обратно если игра определена
                if (GameManager.CheckGame(ProfileManager.CurrentProfile.GameModel)) {
                    NavPersonalization.IsEnabled = true;
                }
            }
        }

        private void OnClosingLocked(object sender, LockedEventArgs e) {
            if (hWnd == IntPtr.Zero) {
                hWnd = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle;
            }
            //Заблокировать закрытие окна
            IsCloseLocked = e.IsLocked;
            this.IsCloseButtonEnabled = !e.IsLocked;
            //Отключим кнопку "Х"
            NativeMethods.EnableMenuItem(NativeMethods.GetSystemMenu(hWnd, false),
                NativeMethods.SC_CLOSE,
                e.IsLocked ? NativeMethods.MF_DISABLED | NativeMethods.MF_GRAYED : NativeMethods.MF_ENABLED);
        }

        public void ReloadTabs() {
            //Выбираем первую вкладку и отключаем все модули
            NavControl.SelectedIndex = 0;
            NavGallery.IsEnabled = false;
            NavCommunity.IsEnabled = false;
            NavPersonalization.IsEnabled = false;

            IGameModel model = ProfileManager.CurrentProfile.GameModel;

            //Если доступен веб-профиль, включаем вкладку сообщества
            if (GameManager.GetConfiguration(model).IsWebAvailable) {
                NavCommunity.IsEnabled = true;
            }

            //Если путь до игры верен, включаем вкладку галереи и персонализации
            if (GameManager.CheckGame(model)) {
                NavGallery.IsEnabled = true;
                NavPersonalization.IsEnabled = true;
            }
        }

        private void OnProfileChanged(object sender, EventArgs e) {
            ReloadTabs();
            MenuFlyout.Width = ProfileSwitcher.ActualWidth + FLYOUT_WIDTH_MIN;
            SettingsFlyout.Width = ProfileSwitcher.ActualWidth + FLYOUT_WIDTH_MIN;
        }

        private void OnTabChanged(object sender, SelectionChangedEventArgs e) {
            TabItem selectedTab = (TabItem)NavControl.SelectedValue;
            AbstractPage selectedPage = (AbstractPage)selectedTab.Content;
            //Prevent handling over changing inside tab item
            if (currentTab == selectedPage) {
                return;
            }
            if (currentTab != null) {
                currentTab.PageClose();
            }
            currentTab = selectedPage;
            currentTab.PageActivate();
            selectedTab.Focus();
        }

        private void OnProfileSettingsClick(object sender, RoutedEventArgs e) {
            if (SettingsWindow == null) {
                SettingsWindow = App.Kernel.Get<Settings>();
                SettingsWindow.WindowClosed += (s, e1) => {
                    transitionLayer.Content = null;
                };
            }
            transitionLayer.Content = SettingsWindow;
            SettingsWindow.Show();
        }

        private void OnAboutClick(object sender, RoutedEventArgs e) {
            if (AboutWindow == null) {
                AboutWindow = App.Kernel.Get<About>();
                AboutWindow.WindowClosed += (s, e1) => {
                    transitionLayer.Content = null;
                };
            }
            transitionLayer.Content = AboutWindow;
            AboutWindow.Show();
        }

        private void OnLoggerClick(object sender, RoutedEventArgs e) {
            transitionLayer.Content = Logger;
            Logger.Show();
        }

        private void ShowSettings(object sender, RoutedEventArgs e) {
            MenuFlyout.Width = ProfileSwitcher.ActualWidth + FLYOUT_WIDTH_MIN;
            MenuFlyout.IsOpen = !MenuFlyout.IsOpen;
            if (MenuFlyout.IsOpen == false) {
                SettingsFlyout.IsOpen = false;
            }
        }
    }
}