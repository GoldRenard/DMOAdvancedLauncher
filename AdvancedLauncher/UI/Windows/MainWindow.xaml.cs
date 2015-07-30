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
using System.Windows;
using System.Windows.Media;
using AdvancedLauncher.Model.Protected;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.Tools;
using MahApps.Metro.Controls;
using Ninject;

namespace AdvancedLauncher.UI.Windows {

    public partial class MainWindow : MetroWindow {
        public static int FLYOUT_WIDTH_MIN = 100;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(MainWindow));

        private IntPtr hWnd = IntPtr.Zero;

        private bool IsCloseLocked = true;

        private NewsWindow NewsWindow = null;

        private Settings SettingsWindow = null;

        private About AboutWindow = null;

        [Inject]
        public Logger Logger {
            get; set;
        }

        [Inject]
        public IEnvironmentManager EnvironmentManager {
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

        public MainWindow() {
            App.Kernel.Inject(this);
            Splashscreen.SetProgress("Loading...");
            Application.Current.MainWindow = this;
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                NewsWindow = new NewsWindow();
                transitionLayer.Content = NewsWindow;
                RenderOptions.SetBitmapScalingMode(ProfileSwitcher, BitmapScalingMode.HighQuality);
                Logger.WindowClosed += (s, e1) => {
                    transitionLayer.Content = NewsWindow;
                };
                LanguageManager.LanguageChanged += (s, e) => {
                    this.DataContext = LanguageManager.Model;
                };
                ProfileSwitcher.DataContext = ProfileManager;
                ProfileManager.ProfileChanged += OnProfileChanged;
                EnvironmentManager.ClosingLocked += OnClosingLocked;
                this.Closing += (s, e) => {
                    e.Cancel = IsCloseLocked;
                    App.Kernel.Get<ITaskManager>().CloseApp(true);
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
                CheckUpdates();
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

        private void OnProfileChanged(object sender, EventArgs e) {
            MenuFlyout.Width = ProfileSwitcher.ActualWidth + FLYOUT_WIDTH_MIN;
            SettingsFlyout.Width = ProfileSwitcher.ActualWidth + FLYOUT_WIDTH_MIN;
        }

        private void OnProfileSettingsClick(object sender, RoutedEventArgs e) {
            if (SettingsWindow == null) {
                SettingsWindow = App.Kernel.Get<Settings>();
                SettingsWindow.WindowClosed += (s, e1) => {
                    transitionLayer.Content = NewsWindow;
                };
            }
            transitionLayer.Content = SettingsWindow;
            SettingsWindow.Show();
        }

        private void OnAboutClick(object sender, RoutedEventArgs e) {
            if (AboutWindow == null) {
                AboutWindow = App.Kernel.Get<About>();
                AboutWindow.WindowClosed += (s, e1) => {
                    transitionLayer.Content = NewsWindow;
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

        public void CheckUpdates() {
            BackgroundWorker updateWorker = new BackgroundWorker();
            updateWorker.DoWork += async (s1, e2) => {
                RemoteVersion remote = RemoteVersion.Instance;
                if (remote != null) {
                    Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    if (remote.Version.CompareTo(currentVersion) > 0) {
                        string content = string.Format(LanguageManager.Model.UpdateAvailableText, remote.Version)
                            + System.Environment.NewLine
                            + System.Environment.NewLine
                            + remote.ChangeLog
                            + System.Environment.NewLine
                            + System.Environment.NewLine
                            + LanguageManager.Model.UpdateDownloadQuestion;
                        string caption = string.Format(LanguageManager.Model.UpdateAvailableCaption, remote.Version);
                        if (await App.Kernel.Get<IDialogManager>().ShowYesNoDialog(caption, content)) {
                            URLUtils.OpenSite(remote.DownloadUrl);
                        }
                    }
                }
            };
            updateWorker.RunWorkerAsync();
        }
    }
}