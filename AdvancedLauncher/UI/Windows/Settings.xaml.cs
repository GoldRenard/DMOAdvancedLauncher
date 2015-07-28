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
using AdvancedLauncher.Management.Execution;
using AdvancedLauncher.Model.Config;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.Tools;
using AdvancedLauncher.UI.Extension;
using Ninject;

namespace AdvancedLauncher.UI.Windows {

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

        private bool IsPreventPassChange = false;

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get; set;
        }

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        [Inject]
        public ILauncherManager LauncherManager {
            get; set;
        }

        [Inject]
        public IConfigurationManager GameManager {
            get; set;
        }

        public Settings() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            }
        }

        public override void Show() {
            base.Show();
            ReloadProfiles();
        }

        private void ReloadProfiles() {
            ProfileManager.RevertChanges();
            foreach (Profile p in ProfileManager.PendingProfiles) {
                if (p.Id == ProfileManager.CurrentProfile.Id) {
                    ProfileList.SelectedItem = p;
                    break;
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
            /*if (SelectedProfile.Login.SecurePassword != null) {
                pbPass.Password = "empty_pass";
            } else {
                pbPass.Clear();
            }*/
            IsPreventPassChange = false;
        }

        private void OnTypeSelectionChanged(object sender, SelectionChangedEventArgs e) {
            ValidatePaths();
        }

        private void OnSetDefaultClick(object sender, RoutedEventArgs e) {
            ProfileManager.PendingDefaultProfile = SelectedProfile;
            NotifyPropertyChanged("IsSelectedNotDefault");
        }

        public bool IsSelectedNotDefault {
            set {
            }
            get {
                return !ProfileManager.PendingDefaultProfile.Equals(SelectedProfile);
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e) {
            ProfileManager.AddProfile();
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e) {
            if (ProfileList.Items.Count == 1) {
                DialogsHelper.ShowErrorDialog(LanguageManager.Model.Settings_LastProfile);
                return;
            }
            Profile profile = SelectedProfile;

            if (ProfileList.SelectedIndex != ProfileList.Items.Count - 1) {
                ProfileList.SelectedIndex++;
            } else {
                ProfileList.SelectedIndex--;
            }

            ProfileManager.RemoveProfile(profile);
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
            Folderdialog.Description = LanguageManager.Model.Settings_SelectGameDir;
            while (true) {
                if (Folderdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    string defaultPath = SelectedProfile.GameModel.GamePath;
                    SelectedProfile.GameModel.GamePath = Folderdialog.SelectedPath;
                    if (GameManager.CheckGame(SelectedProfile.GameModel)) {
                        break;
                    }
                    SelectedProfile.GameModel.GamePath = defaultPath;
                    await DialogsHelper.ShowMessageDialogAsync(LanguageManager.Model.Settings_GamePath,
                        LanguageManager.Model.Settings_SelectGameDirError);
                } else {
                    break;
                }
            }
        }

        private async void OnLauncherBrowse(object sender, RoutedEventArgs e) {
            Folderdialog.Description = LanguageManager.Model.Settings_SelectLauncherDir;
            while (true) {
                if (Folderdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    string defaultPath = SelectedProfile.GameModel.DefLauncherPath;
                    SelectedProfile.GameModel.DefLauncherPath = Folderdialog.SelectedPath;

                    if (GameManager.CheckLauncher(SelectedProfile.GameModel)) {
                        break;
                    }
                    SelectedProfile.GameModel.DefLauncherPath = defaultPath;
                    await DialogsHelper.ShowMessageDialogAsync(LanguageManager.Model.Settings_LauncherPath,
                        LanguageManager.Model.Settings_SelectLauncherDirError);
                } else {
                    break;
                }
            }
        }

        #endregion Path Browse Section

        #region AppLocale Section

        public bool IsALSupported {
            get {
                return LauncherManager.findByType<AppLocaleLauncher>(typeof(AppLocaleLauncher)).IsSupported;
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

            string message = LanguageManager.Model.AppLocale_FailReasons + System.Environment.NewLine;
            if (!AppLocaleLauncher.IsInstalled) {
                message += System.Environment.NewLine + LanguageManager.Model.AppLocale_NotInstalled;
            }

            if (!AppLocaleLauncher.IsKoreanSupported) {
                message += System.Environment.NewLine + LanguageManager.Model.AppLocale_EALNotInstalled;
            }
            message += System.Environment.NewLine + System.Environment.NewLine + LanguageManager.Model.AppLocale_FixQuestion;

            if (await DialogsHelper.ShowYesNoDialog(LanguageManager.Model.AppLocale_Error, message)) {
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
            ProfileManager.ApplyChanges();
            EnvironmentManager.Save();
            Close();
        }

        #endregion Global Actions Section

        #region Service

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            URLUtils.OpenSite(e.Uri.AbsoluteUri);
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
            //SelectedProfile.Login.SecurePassword = pbPass.SecurePassword;
        }

        private void Run_Loaded(object sender, RoutedEventArgs e) {
            Run run = sender as Run;
            LanguageManager.LanguageChanged += (s, e2) => {
                run.Text = LanguageManager.Model.Settings_AppLocale_Help;
            };
        }
    }
}