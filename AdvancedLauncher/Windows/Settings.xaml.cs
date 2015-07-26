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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Environment.Containers;
using AdvancedLauncher.Service;
using AdvancedLauncher.Service.Execution;

namespace AdvancedLauncher.Windows {

    public partial class Settings : AbstractWindow, INotifyPropertyChanged {
        private string LINK_EAL_INSTALLING_RUS = "http://www.bolden.ru/index.php?option=com_content&task=view&id=76";
        private string LINK_EAL_INSTALLING = "http://www.voom.net/install-files-for-east-asian-languages-windows-xp";
        private string LINK_MS_APPLOCALE = "http://www.microsoft.com/en-us/download/details.aspx?id=2043";

        private Microsoft.Win32.OpenFileDialog FileDialog = new Microsoft.Win32.OpenFileDialog() {
            Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
        };

        private System.Windows.Forms.FolderBrowserDialog Folderdialog = new System.Windows.Forms.FolderBrowserDialog() {
            ShowNewFolderButton = false
        };

        private AdvancedLauncher.Environment.Containers.Settings settingsContainer;
        private bool IsPreventPassChange = false;

        public Settings() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
                ComboBoxLauncher.ItemsSource = LauncherFactory.Instance;
                //Copying settings object and set it as DataContext
                ProfileList.DataContext = settingsContainer = new AdvancedLauncher.Environment.Containers.Settings(LauncherEnv.Settings);

                //Search and set current profile
                foreach (Profile p in settingsContainer.Profiles) {
                    if (p.Id == LauncherEnv.Settings.CurrentProfile.Id) {
                        ProfileList.SelectedItem = p;
                        break;
                    }
                }
            }
        }

        #region Profile Section

        public static Profile SelectedProfile;

        private void OnProfileSelectionChanged(object sender, SelectionChangedEventArgs e) {
            SelectedProfile = (Profile)ProfileList.SelectedItem;
            if (SelectedProfile == null) {
                return;
            }
            ValidatePaths();
            NotifyPropertyChanged("IsSelectedNotDefault");

            IsPreventPassChange = true;
            if (SelectedProfile.Login.SecurePassword != null) {
                pbPass.Password = "empty_pass";
            } else {
                pbPass.Clear();
            }
            IsPreventPassChange = false;
        }

        private void OnTypeSelectionChanged(object sender, SelectionChangedEventArgs e) {
            ValidatePaths();
        }

        private void OnSetDefaultClick(object sender, RoutedEventArgs e) {
            settingsContainer.DefaultProfile = SelectedProfile.Id;
            NotifyPropertyChanged("IsSelectedNotDefault");
        }

        public bool IsSelectedNotDefault {
            set {
            }
            get {
                return SelectedProfile.Id != settingsContainer.DefaultProfile;
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e) {
            settingsContainer.AddProfile();
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e) {
            if (ProfileList.Items.Count == 1) {
                Utils.ShowErrorDialog(LanguageEnv.Strings.Settings_LastProfile);
                return;
            }
            Profile profile = SelectedProfile;

            if (ProfileList.SelectedIndex != ProfileList.Items.Count - 1) {
                ProfileList.SelectedIndex++;
            } else {
                ProfileList.SelectedIndex--;
            }

            settingsContainer.RemoveProfile(profile);
        }

        private void OnImageSelect(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Nullable<bool> result = FileDialog.ShowDialog();
            if (result == true) {
                SelectedProfile.ImagePath = FileDialog.FileName;
            }
        }

        #endregion Profile Section

        #region Path Browse Section

        private async void OnGameBrowse(object sender, RoutedEventArgs e) {
            Folderdialog.Description = LanguageEnv.Strings.Settings_SelectGameDir;
            while (true) {
                if (Folderdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    if (SelectedProfile.GameEnv.CheckGame(Folderdialog.SelectedPath)) {
                        SelectedProfile.GameEnv.GamePath = Folderdialog.SelectedPath;
                        break;
                    } else {
                        await Utils.ShowMessageDialogAsync(LanguageEnv.Strings.Settings_GamePath,
                            LanguageEnv.Strings.Settings_SelectGameDirError);
                    }
                } else {
                    break;
                }
            }
        }

        private async void OnLauncherBrowse(object sender, RoutedEventArgs e) {
            Folderdialog.Description = LanguageEnv.Strings.Settings_SelectLauncherDir;
            while (true) {
                if (Folderdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    if (SelectedProfile.GameEnv.CheckDefLauncher(Folderdialog.SelectedPath)) {
                        SelectedProfile.GameEnv.DefLauncherPath = Folderdialog.SelectedPath;
                        break;
                    } else {
                        await Utils.ShowMessageDialogAsync(LanguageEnv.Strings.Settings_LauncherPath,
                            LanguageEnv.Strings.Settings_SelectLauncherDirError);
                    }
                } else {
                    break;
                }
            }
        }

        #endregion Path Browse Section

        #region AppLocale Section

        public bool IsALSupported {
            get {
                return LauncherFactory.findByType<AppLocaleLauncher>(typeof(AppLocaleLauncher)).IsSupported;
            }
        }

        public bool IsALNotSupported {
            get {
                return !IsALSupported;
            }
        }

        private async void OnAppLocaleHelpClick(object sender, RoutedEventArgs e) {
            ComboBoxItem item = (sender as Hyperlink).Parent.FindAncestor<ComboBoxItem>();
            AppLocaleLauncher launcher = item.Content as AppLocaleLauncher;
            if (launcher == null || IsALSupported) {
                return;
            }

            string message = LanguageEnv.Strings.AppLocale_FailReasons + System.Environment.NewLine;
            if (!AppLocaleLauncher.IsInstalled) {
                message += System.Environment.NewLine + LanguageEnv.Strings.AppLocale_NotInstalled;
            }

            if (!AppLocaleLauncher.IsKoreanSupported) {
                message += System.Environment.NewLine + LanguageEnv.Strings.AppLocale_EALNotInstalled;
            }
            message += System.Environment.NewLine + System.Environment.NewLine + LanguageEnv.Strings.AppLocale_FixQuestion;

            if (await Utils.ShowYesNoDialog(LanguageEnv.Strings.AppLocale_Error, message)) {
                if (!AppLocaleLauncher.IsInstalled) {
                    System.Diagnostics.Process.Start(LINK_MS_APPLOCALE);
                }
                if (!AppLocaleLauncher.IsKoreanSupported) {
                    if (CultureInfo.CurrentCulture.Name == "ru-RU") {
                        System.Diagnostics.Process.Start(LINK_EAL_INSTALLING_RUS);
                    } else {
                        System.Diagnostics.Process.Start(LINK_EAL_INSTALLING);
                    }
                }
            }
        }

        #endregion AppLocale Section

        #region Global Actions Section

        protected override void OnCloseClick(object sender, RoutedEventArgs e) {
            base.OnCloseClick(sender, e);
        }

        private void OnApplyClick(object sender, RoutedEventArgs e) {
            LauncherEnv.Settings.MergeProfiles(settingsContainer);
            LauncherEnv.Save();
            Close();
        }

        #endregion Global Actions Section

        #region Service

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            Utils.OpenSite(e.Uri.AbsoluteUri);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Service

        #region Validation

        private void ValidatePaths() {
            tbGamePath.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbLauncherPath.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        #endregion Validation

        private void PasswordChanged(object sender, RoutedEventArgs e) {
            if (IsPreventPassChange) {
                return;
            }
            SelectedProfile.Login.SecurePassword = pbPass.SecurePassword;
        }

        private void Run_Loaded(object sender, RoutedEventArgs e) {
            Run run = sender as Run;
            LanguageEnv.LanguageChanged += (s, e2) => {
                run.Text = LanguageEnv.Strings.Settings_AppLocale_Help;
            };
        }
    }
}