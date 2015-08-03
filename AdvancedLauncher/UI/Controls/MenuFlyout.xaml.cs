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
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using Ninject;

namespace AdvancedLauncher.UI.Controls {

    public partial class MenuFlyout : AbstractFlyout, INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _IsChangeEnabled = true;

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
                ReloadProfiles(this, SDK.Model.Events.EventArgs.Empty);
                CommandList.ItemsSource = WindowManager.MenuItems;
            }
        }

        #region Commands

        private Windows.Settings SettingsWindow = null;

        #endregion Commands

        #region Profile Selection

        private void OnProfileLocked(object sender, LockedEventArgs e) {
            IsChangeEnabled = !e.IsLocked;
        }

        /* We must prevent updating current profile in ProfileList_SelectionChanged by updating
         * ProfileList.ItemsSource or ProfileList.SelectedItem by outside profiles reloading */
        private bool IsPreventChange = false;

        private void ReloadProfiles(object sender, SDK.Model.Events.EventArgs e) {
            IsPreventChange = true;
            ProfileList.ItemsSource = ProfileManager.Profiles;
            ProfileList.SelectedItem = ProfileManager.CurrentProfile;
            IsPreventChange = false;
        }

        private void ReloadCurrentProfile(object sender, SDK.Model.Events.EventArgs e) {
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
            WindowManager.ShowWindow(SettingsWindow);
            this.IsOpen = false;
        }
    }
}