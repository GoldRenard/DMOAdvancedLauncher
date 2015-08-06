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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdvancedLauncher.Management;
using AdvancedLauncher.Model.Proxy;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.UI.Extension;
using Ninject;

namespace AdvancedLauncher.UI.Controls {

    public partial class MenuFlyout : AbstractFlyout, INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _IsChangeEnabled = true;

        private Windows.Settings SettingsWindow = null;

        [Inject]
        public IWindowManager WindowManager {
            get; set;
        }

        public MenuFlyout() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                ProfileSettings.ItemsSource = new List<object>() { new object() };
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
                ProfileManager.ProfileChanged += ReloadCurrentProfile;
                ProfileManager.ProfileLocked += OnProfileLocked;
                ProfileManager.CollectionChanged += ReloadProfiles;
                ReloadProfiles(this, BaseEventArgs.Empty);
                WindowManager WM = WindowManager as WindowManager;
                var values = WM.MenuItems.GetLinkedProxy<SDK.Model.MenuItem, MenuItemViewModel>(LanguageManager);
                CommandList.ItemsSource = values;
            }
        }

        #region Profile Selection

        private void OnProfileLocked(object sender, LockedEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new LockedChangedHandler((s, e2) => {
                    OnProfileLocked(sender, e2);
                }), sender, e);
                return;
            }
            IsChangeEnabled = !e.IsLocked;
        }

        /* We must prevent updating current profile in ProfileList_SelectionChanged by updating
         * ProfileList.ItemsSource or ProfileList.SelectedItem by outside profiles reloading */
        private bool IsPreventChange = false;

        private void ReloadProfiles(object sender, BaseEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new BaseEventHandler((s, e2) => {
                    ReloadProfiles(sender, e2);
                }), sender, e);
                return;
            }
            IsPreventChange = true;
            ProfileList.ItemsSource = ProfileManager.Profiles;
            ProfileList.SelectedItem = ProfileManager.CurrentProfile;
            IsPreventChange = false;
        }

        private void ReloadCurrentProfile(object sender, BaseEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new BaseEventHandler((s, e2) => {
                    ReloadCurrentProfile(sender, e2);
                }), sender, e);
                return;
            }
            IsPreventChange = true;
            ProfileList.SelectedItem = ProfileManager.CurrentProfile;
            IsPreventChange = false;
        }

        private void OnProfileSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!IsPreventChange && ProfileList.SelectedItem != null) {
                ProfileManager.CurrentProfile = (Profile)ProfileList.SelectedItem;
                IsOpen = false;
            }
        }

        #endregion Profile Selection

        #region Service

        public bool IsChangeEnabled {
            set {
                _IsChangeEnabled = value;
                NotifyPropertyChanged("IsChangeEnabled");
                NotifyPropertyChanged("IsChangeDisabled");
            }
            get {
                return _IsChangeEnabled;
            }
        }

        public bool IsChangeDisabled {
            set {
            }
            get {
                return !_IsChangeEnabled;
            }
        }

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Service

        private void ProfileSettings_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ProfileSettings.SelectedIndex == -1) {
                return;
            }
            ProfileSettings.SelectedIndex = -1;
            if (SettingsWindow == null) {
                SettingsWindow = App.Kernel.Get<Windows.Settings>();
            }
            WindowManager.ShowWindow(SettingsWindow.Container);
            this.IsOpen = false;
        }
    }
}