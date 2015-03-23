// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Windows.Input;
using System.Windows.Media;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Environment.Containers;
using MahApps.Metro.Controls;

namespace AdvancedLauncher.Controls {

    public partial class MenuFlyout : Flyout, INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _IsChangeEnabled = true;

        public MenuFlyout() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                ProfileSettings.ItemsSource = new List<object>() { new object() };
                this.SizeChanged += (s, e) => {
                    ProfileList.MaxHeight = e.NewSize.Height - CommandsHolder.ActualHeight - 50;
                };
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
                LanguageEnv.LanguageChanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
                LauncherEnv.Settings.ProfileChanged += ReloadCurrentProfile;
                LauncherEnv.Settings.ProfileLocked += OnProfileLocked;
                LauncherEnv.Settings.CollectionChanged += ReloadProfiles;
                ReloadProfiles();
                BuildCommands();
            }
        }

        #region Commands

        private List<MenuItem> Commands = new List<MenuItem>();

        public class MenuItem : INotifyPropertyChanged {

            public event PropertyChangedEventHandler PropertyChanged;

            private readonly string bindingName;

            private static SolidColorBrush Brush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            public MenuItem(string bindingName, Canvas icon, Thickness iconMargin, ICommand command) {
                this.bindingName = bindingName;
                Command = command;
                IconMargin = iconMargin;
                icon.Resources.Add("BlackBrush", Brush);
                IconBrush = new VisualBrush(icon);
                LanguageEnv.LanguageChanged += () => {
                    this.NotifyPropertyChanged("Name");
                };
            }

            public string Name {
                get {
                    return (string)LanguageEnv.Strings.GetType().GetProperty(bindingName).GetValue(LanguageEnv.Strings, null);
                }
            }

            public ICommand Command {
                get;
                set;
            }

            public bool IsEnabled {
                get {
                    return Command.CanExecute(Command);
                }
            }

            public VisualBrush IconBrush {
                get;
                set;
            }

            public Thickness IconMargin {
                get;
                set;
            }

            public void NotifyEnabled() {
                NotifyPropertyChanged("IsEnabled");
            }

            private void NotifyPropertyChanged(String propertyName) {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (null != handler) {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public event RoutedEventHandler AboutClick;

        public event RoutedEventHandler ProfileSettingsClick;

        public event RoutedEventHandler LoggerClick;

        private void BuildCommands() {
            Commands.Clear();
            Commands.Add(new MenuItem("Console", FindResource<Canvas>("appbar_app"), new Thickness(5, 7, 5, 7), new ModelCommand((p) => {
                if (LoggerClick != null) {
                    LoggerClick(this, null);
                }
                this.IsOpen = false;
            })));
            Commands.Add(new MenuItem("About", FindResource<Canvas>("appbar_information"), new Thickness(9, 4, 9, 4), new ModelCommand((p) => {
                if (AboutClick != null) {
                    AboutClick(this, null);
                }
                this.IsOpen = false;
            })));
            CommandList.ItemsSource = Commands;
        }

        public T FindResource<T>(string name) {
            return (T)this.FindResource(name);
        }

        #endregion Commands

        #region Profile Selection

        private void OnProfileLocked(bool IsLocked) {
            IsChangeEnabled = !IsLocked;
            Commands.ForEach(e => e.NotifyEnabled());
        }

        /* We must prevent updating current profile in ProfileList_SelectionChanged by updating
         * ProfileList.ItemsSource or ProfileList.SelectedItem by outside profiles reloading */
        private bool IsPreventChange = false;

        private void ReloadProfiles() {
            IsPreventChange = true;
            ProfileList.ItemsSource = LauncherEnv.Settings.Profiles;
            ProfileList.SelectedItem = LauncherEnv.Settings.CurrentProfile;
            IsPreventChange = false;
        }

        private void ReloadCurrentProfile() {
            IsPreventChange = true;
            ProfileList.SelectedItem = LauncherEnv.Settings.CurrentProfile;
            IsPreventChange = false;
        }

        private void OnProfileSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!IsPreventChange) {
                LauncherEnv.Settings.CurrentProfile = (Profile)ProfileList.SelectedItem;
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
            if (ProfileSettingsClick != null) {
                ProfileSettingsClick(this, null);
            }
            this.IsOpen = false;
        }
    }
}