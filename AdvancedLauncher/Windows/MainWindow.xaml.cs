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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Environment.Containers;
using AdvancedLauncher.Pages;
using MahApps.Metro.Controls;

namespace AdvancedLauncher.Windows {

    public partial class MainWindow : MetroWindow {
        public static int FLYOUT_WIDTH_MIN = 100;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(MainWindow));
        private const int SC_CLOSE = 0xF060;
        private const int MF_ENABLED = 0x0000;
        private const int MF_GRAYED = 0x0001;
        private const int MF_DISABLED = 0x0002;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, int wEnable);

        private IntPtr hWnd = IntPtr.Zero;

        private bool IsCloseLocked = true;

        private Settings SettingsWindow = null;
        private About AboutWindow = null;
        private AbstractPage currentTab;

        public event PropertyChangedEventHandler PropertyChanged;

        private static MainWindow _Instance;

        public static MainWindow Instance {
            get {
                return _Instance;
            }
        }

        public MainWindow() {
            _Instance = this;
            // force initialize default profile in current thread
            var profile = LauncherEnv.Settings.CurrentProfile;
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                //Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 60 });
                RenderOptions.SetBitmapScalingMode(ProfileSwitcher, BitmapScalingMode.HighQuality);
                Logger.Instance.WindowClosed += (s, e1) => {
                    transitionLayer.Content = null;
                };
                AdvancedLauncher.Service.UpdateChecker.Check();
                LanguageEnv.LanguageChanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
                LauncherEnv.Settings.ProfileChanged += OnProfileChanged;
                LauncherEnv.Settings.ClosingLocked += OnClosingLocked;
                LauncherEnv.Settings.FileSystemLocked += OnFileSystemLocked;
                this.Closing += (s, e) => {
                    e.Cancel = IsCloseLocked;
                };
                this.MouseDown += MainWindow_MouseDown;

                MenuFlyout.AboutClick += OnAboutClick;
                MenuFlyout.ProfileSettingsClick += OnProfileSettingsClick;
                MenuFlyout.LoggerClick += OnLoggerClick;
                OnProfileChanged();
                try {
                    App.splash.Close(TimeSpan.FromSeconds(1));
                } catch {
                }
#if DEBUG
                this.Title += " (development build " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
#endif
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

        private void OnFileSystemLocked(bool IsLocked) {
            if (IsLocked) {
                //Выбираем первую вкладку и отключаем персонализацию (на всякий случай)
                NavControl.SelectedIndex = 0;
                NavPersonalization.IsEnabled = false;
            } else {
                //Включаем персонализации обратно если игра определена
                if (LauncherEnv.Settings.CurrentProfile.GameEnv.CheckGame()) {
                    NavPersonalization.IsEnabled = true;
                }
            }
        }

        private void OnClosingLocked(bool IsLocked) {
            if (hWnd == IntPtr.Zero)
                hWnd = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle;
            //Заблокировать закрытие окна
            IsCloseLocked = IsLocked;
            this.IsCloseButtonEnabled = !IsLocked;
            //Отключим кнопку "Х"
            EnableMenuItem(GetSystemMenu(hWnd, false), SC_CLOSE, IsLocked ? MF_DISABLED | MF_GRAYED : MF_ENABLED);
        }

        public void ReloadTabs() {
            //Выбираем первую вкладку и отключаем все модули
            NavControl.SelectedIndex = 0;
            NavGallery.IsEnabled = false;
            NavCommunity.IsEnabled = false;
            NavPersonalization.IsEnabled = false;

            //Если доступен веб-профиль, включаем вкладку сообщества
            if (LauncherEnv.Settings.CurrentProfile.DMOProfile.IsWebAvailable) {
                NavCommunity.IsEnabled = true;
            }

            //Если путь до игры верен, включаем вкладку галереи и персонализации
            if (LauncherEnv.Settings.CurrentProfile.GameEnv.CheckGame()) {
                NavGallery.IsEnabled = true;
                NavPersonalization.IsEnabled = true;
            }
        }

        private void OnProfileChanged() {
            ReloadTabs();
            MenuFlyout.Width = ProfileSwitcher.ActualWidth + FLYOUT_WIDTH_MIN;
            SettingsFlyout.Width = ProfileSwitcher.ActualWidth + FLYOUT_WIDTH_MIN;
            //NotifyPropertyChanged("CurrentProfile");
            ProfileSwitcher.DataContext = LauncherEnv.Settings.CurrentProfile;
        }

        public Profile CurrentProfile {
            get {
                return LauncherEnv.Settings.CurrentProfile;
            }
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
                SettingsWindow = new Settings();
                SettingsWindow.WindowClosed += (s, e1) => {
                    transitionLayer.Content = null;
                };
            }
            transitionLayer.Content = SettingsWindow;
            SettingsWindow.Show();
        }

        private void OnAboutClick(object sender, RoutedEventArgs e) {
            if (AboutWindow == null) {
                AboutWindow = new About();
                AboutWindow.WindowClosed += (s, e1) => {
                    transitionLayer.Content = null;
                };
            }
            transitionLayer.Content = AboutWindow;
            AboutWindow.Show();
        }

        private void OnLoggerClick(object sender, RoutedEventArgs e) {
            transitionLayer.Content = Logger.Instance;
            Logger.Instance.Show();
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