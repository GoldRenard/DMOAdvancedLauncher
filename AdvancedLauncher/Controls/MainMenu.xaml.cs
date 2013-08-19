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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Environment.Containers;

namespace AdvancedLauncher.Controls
{
    public partial class MainMenu : UserControl, INotifyPropertyChanged
    {
        public MainMenu()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
                LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
                LauncherEnv.Settings.ProfileChanged += ReloadCurrentProfile;
                LauncherEnv.Settings.CollectionChanged += ReloadProfiles;
                ReloadProfiles();
            }
        }

        #region Buttons Handler
        public event RoutedEventHandler SettingsClick;
        private void NotifySettingsClick(object sender, RoutedEventArgs e)
        {
            if (SettingsClick != null)
                SettingsClick(sender, e);
        }

        public event RoutedEventHandler AboutClick;
        private void NotifyAboutClick(object sender, RoutedEventArgs e)
        {
            if (AboutClick != null)
                AboutClick(sender, e);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            NotifySettingsClick(sender, e);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NotifyAboutClick(sender, e);
        }
        #endregion

        #region Profile Selection

        /* We must prevent updating current profile in ProfileList_SelectionChanged by updating 
         * ProfileList.ItemsSource or ProfileList.SelectedItem by outside profiles reloading */
        private bool IsPreventChange = false;

        void ReloadProfiles()
        {
            IsPreventChange = true;
            ProfileList.ItemsSource = LauncherEnv.Settings.pCollection;
            ProfileList.SelectedItem = LauncherEnv.Settings.pCurrent;
            IsPreventChange = false;
        }

        void ReloadCurrentProfile()
        {
            IsPreventChange = true;
            ProfileList.SelectedItem = LauncherEnv.Settings.pCurrent;
            IsPreventChange = false;
        }

        private void ProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsPreventChange)
                LauncherEnv.Settings.pCurrent = (Profile)ProfileList.SelectedItem;
        }

        #endregion

        #region Service

        private bool _IsOpened = false;
        public bool IsOpened
        {
            set
            {
                _IsOpened = value;
                if (value == true)
                    this.Focus();
                NotifyPropertyChanged("IsOpened");
            }
            get
            {
                return _IsOpened;
            }
        }

        private bool _isEnabled = true;
        public bool isEnabled
        {
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged("isEnabled");
                NotifyPropertyChanged("isDisabled");
            }
            get
            {
                return _isEnabled;
            }
        }

        public bool isDisabled
        {
            set { }
            get
            {
                return !_isEnabled;
            }
        }

        private void ToggleButton_LostFocus(object sender, RoutedEventArgs e)
        {
            IsOpened = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
