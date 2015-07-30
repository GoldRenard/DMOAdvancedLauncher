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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AdvancedLauncher.Management;
using AdvancedLauncher.Model.Config;
using AdvancedLauncher.Model.Protected;
using AdvancedLauncher.UI.Commands;
using MahApps.Metro;
using Ninject;

namespace AdvancedLauncher.UI.Controls {

    public partial class SettingsFlyout : AbstractFlyout {
        private int CurrentLangIndex;
        private AppThemeMenuData CurrentAppTheme;
        private AccentColorMenuData CurrentAccent;

        private bool IsPreventPassChange = false;

        private Settings settingsContainer;

        private ProxySetting proxySettings;

        public SettingsFlyout() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                LanguageManager.LanguageChanged += (s, e) => {
                    this.DataContext = LanguageManager.Model;
                };

                this.CloseCommand = new SimpleCommand {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => this.ResetAll()
                };

                this.ClosingFinished += (s, e) => {
                    ResetAll();
                };

                ResetAll();

                IsPreventPassChange = true;
                if (proxySettings.Credentials.SecurePassword != null) {
                    ProxyPassword.Password = "empty_pass";
                } else {
                    ProxyPassword.Clear();
                }
                IsPreventPassChange = false;

                InitializeColorTheme();
                InitializeLanguages();
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e) {
            this.IsOpen = false;
            ResetAll();
        }

        private void OnApplyClick(object sender, RoutedEventArgs e) {
            CurrentLangIndex = ComboBoxLanguage.SelectedIndex;
            CurrentAppTheme = (AppThemeMenuData)BaseColorsList.SelectedItem;
            CurrentAccent = (AccentColorMenuData)AccentColorsList.SelectedItem;
            settingsContainer.LanguageFile = ComboBoxLanguage.SelectedValue.ToString();
            settingsContainer.AppTheme = CurrentAppTheme.Name;
            settingsContainer.ThemeAccent = CurrentAccent.Name;
            App.Kernel.Get<ProxyManager>().Initialize(proxySettings);
            EnvironmentManager.Settings.MergeConfig(settingsContainer);
            EnvironmentManager.Save();
            this.IsOpen = false;
        }

        private void ResetAll() {
            settingsContainer = new Settings(EnvironmentManager.Settings);
            proxySettings = new ProxySetting(App.Kernel.Get<ProxyManager>().Settings);
            ProxySettings.DataContext = proxySettings;
            ComboBoxLanguage.SelectedIndex = CurrentLangIndex;
            BaseColorsList.SelectedItem = CurrentAppTheme;
            AccentColorsList.SelectedItem = CurrentAccent;
        }

        #region Language changing

        public class LanguageEntry {

            public string Code {
                set;
                get;
            }

            public string Name {
                get {
                    var culture = CultureInfo.GetCultureInfo(Code);
                    if (culture != null) {
                        return culture.NativeName;
                    }
                    return Code;
                }
            }
        }

        private void InitializeLanguages() {
            List<LanguageEntry> Langs = new List<LanguageEntry>() { new LanguageEntry() {
                Code = LanguageManager.GetDefaultName()
            }};
            foreach (string lang in LanguageManager.GetTranslations()) {
                Langs.Add(new LanguageEntry() {
                    Code = Path.GetFileNameWithoutExtension(lang)
                });
            }
            ComboBoxLanguage.ItemsSource = Langs;
            ComboBoxLanguage.SelectedItem = Langs.FirstOrDefault(e => e.Code == EnvironmentManager.Settings.LanguageFile);
            CurrentLangIndex = Langs.IndexOf((LanguageEntry)ComboBoxLanguage.SelectedItem);
        }

        private void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.IsLoaded) {
                LanguageManager.Load(ComboBoxLanguage.SelectedValue.ToString());
            }
        }

        #endregion Language changing

        #region Theme/Accent changing

        public class AccentColorMenuData {

            public string Name {
                get;
                set;
            }

            public Brush BorderColorBrush {
                get;
                set;
            }

            public Brush ColorBrush {
                get;
                set;
            }

            private ICommand changeAccentCommand;

            public ICommand ChangeAccentCommand {
                get {
                    return this.changeAccentCommand ?? (changeAccentCommand = new SimpleCommand {
                        CanExecuteDelegate = x => true,
                        ExecuteDelegate = x => this.DoChangeTheme(x)
                    });
                }
            }

            protected virtual void DoChangeTheme(object sender) {
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                var accent = ThemeManager.GetAccent(this.Name);
                ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
            }
        }

        public class AppThemeMenuData : AccentColorMenuData {

            protected override void DoChangeTheme(object sender) {
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                var appTheme = ThemeManager.GetAppTheme(this.Name);
                ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
            }
        }

        private void InitializeColorTheme() {
            List<AppThemeMenuData> AppThemes = ThemeManager.AppThemes
                .Select(a => new AppThemeMenuData() {
                    Name = a.Name,
                    BorderColorBrush = a.Resources["BlackColorBrush"] as Brush,
                    ColorBrush = a.Resources["WhiteColorBrush"] as Brush
                })
                .ToList();
            List<AccentColorMenuData> Accents = ThemeManager.Accents
                .Select(a => new AccentColorMenuData() {
                    Name = a.Name,
                    ColorBrush = a.Resources["AccentColorBrush"] as Brush
                })
                .ToList();

            BaseColorsList.ItemsSource = AppThemes;
            AccentColorsList.ItemsSource = Accents;

            Tuple<AppTheme, Accent> currentTheme = ThemeManager.DetectAppStyle(Application.Current);
            CurrentAppTheme = AppThemes.First(a => a.Name == currentTheme.Item1.Name);
            CurrentAccent = Accents.First(a => a.Name == currentTheme.Item2.Name);
            BaseColorsList.SelectedItem = CurrentAppTheme;
            AccentColorsList.SelectedItem = CurrentAccent;
        }

        private void ChangeColorHandler(object sender, SelectionChangedEventArgs e) {
            if (!IsLoaded) {
                return;
            }
            ListBox listBox = (ListBox)sender;
            AccentColorMenuData data = (AccentColorMenuData)listBox.SelectedItem;
            data.ChangeAccentCommand.Execute(this);
        }

        #endregion Theme/Accent changing

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) {
            if (IsPreventPassChange) {
                return;
            }
            proxySettings.Credentials.SecurePassword = ProxyPassword.SecurePassword;
        }
    }
}