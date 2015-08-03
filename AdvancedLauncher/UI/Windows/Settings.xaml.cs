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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using AdvancedLauncher.Management;
using AdvancedLauncher.Management.Execution;
using AdvancedLauncher.Model.Protected;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Entity;
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

        private bool IsPreventLoginChange = false;

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
        public IConfigurationManager ConfigurationManager {
            get; set;
        }

        [Inject]
        public IDialogManager DialogManager {
            get; set;
        }

        private Profile SelectedProfile {
            get {
                return ProfileList.SelectedItem as Profile;
            }
        }

        public ObservableCollection<Server> ServerList {
            get;
            set;
        } = new ObservableCollection<Server>();

        private Dictionary<Profile, LoginData> Credentials = new Dictionary<Profile, LoginData>();

        public Settings() {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
                ProfileList.DataContext = ProfileManager;
                ProfileList.ItemsSource = ProfileManager.PendingProfiles;
                ConfigurationCb.ItemsSource = ConfigurationManager;
                ComboBoxServer.ItemsSource = ServerList;
            }
        }

        public override void OnShow() {
            ReloadProfiles();
        }

        private void ReloadProfiles() {
            ProfileManager.RevertChanges();
            Credentials.Clear();
            LoginManager loginManager = App.Kernel.Get<LoginManager>();
            foreach (Profile p in ProfileManager.PendingProfiles) {
                if (p.Id == ProfileManager.CurrentProfile.Id) {
                    ProfileList.SelectedItem = p;
                }
                LoginData data = loginManager.GetCredentials(p);
                Credentials.Add(p, new LoginData(data));
            }
        }

        #region Profile Section

        private void OnProfileSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (SelectedProfile == null) {
                return;
            }
            ValidatePaths();
            NotifyPropertyChanged("IsSelectedNotDefault");

            LoginData login = null;
            IsPreventLoginChange = true;
            if (Credentials.TryGetValue(SelectedProfile, out login)) {
                if (pbPass != null) {
                    if (login.SecurePassword != null) {
                        pbPass.Password = "empty_pass";
                    } else {
                        pbPass.Clear();
                    }
                }
                if (tbUser != null) {
                    tbUser.Text = login.User;
                }
            } else {
                if (tbUser != null) {
                    tbUser.Clear();
                }
                if (pbPass != null) {
                    pbPass.Clear();
                }
            }
            IsPreventLoginChange = false;
        }

        private void OnTypeSelectionChanged(object sender, SelectionChangedEventArgs e) {
            Profile profile = SelectedProfile;
            IConfiguration config = ConfigurationCb.SelectedItem as IConfiguration;
            if (config != null && profile != null) {
                if (config.IsWebAvailable) {
                    ComboBoxServer.SelectedIndex = -1;
                    ServerList.Clear();
                    if (config.ServersProvider.ServerList.Count > 0) {
                        // Construct new list to avoid plugin's transparent proxy issues
                        foreach (Server server in config.ServersProvider.ServerList) {
                            ServerList.Add(new Server(server));
                        }
                        ComboBoxServer.SelectedIndex = 0;
                    }
                }
            }
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
            Profile profile = ProfileManager.CreateProfile();
            Credentials.Add(profile, new LoginData());
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e) {
            if (ProfileList.Items.Count == 1) {
                DialogManager.ShowErrorDialog(LanguageManager.Model.Settings_LastProfile);
                return;
            }
            Profile profile = SelectedProfile;
            if (ProfileList.SelectedIndex != ProfileList.Items.Count - 1) {
                ProfileList.SelectedIndex++;
            } else {
                ProfileList.SelectedIndex--;
            }
            if (ProfileManager.RemoveProfile(profile)) {
                Credentials.Remove(profile);
            }
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
            Profile profile = SelectedProfile;
            Folderdialog.Description = LanguageManager.Model.Settings_SelectGameDir;
            while (true) {
                if (Folderdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    string defaultPath = profile.GameModel.GamePath;
                    profile.GameModel.GamePath = Folderdialog.SelectedPath;
                    if (ConfigurationManager.CheckGame(profile.GameModel)) {
                        break;
                    }
                    profile.GameModel.GamePath = defaultPath;
                    await DialogManager.ShowMessageDialogAsync(LanguageManager.Model.Settings_GamePath,
                        LanguageManager.Model.Settings_SelectGameDirError);
                } else {
                    break;
                }
            }
        }

        private async void OnLauncherBrowse(object sender, RoutedEventArgs e) {
            Profile profile = SelectedProfile;
            Folderdialog.Description = LanguageManager.Model.Settings_SelectLauncherDir;
            while (true) {
                if (Folderdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    string defaultPath = profile.GameModel.LauncherPath;
                    profile.GameModel.LauncherPath = Folderdialog.SelectedPath;

                    if (ConfigurationManager.CheckLauncher(profile.GameModel)) {
                        break;
                    }
                    profile.GameModel.LauncherPath = defaultPath;
                    await DialogManager.ShowMessageDialogAsync(LanguageManager.Model.Settings_LauncherPath,
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

            if (await DialogManager.ShowYesNoDialog(LanguageManager.Model.AppLocale_Error, message)) {
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

        private void OnApplyClick(object sender, RoutedEventArgs e) {
            ProfileManager.ApplyChanges();

            LoginManager loginManager = App.Kernel.Get<LoginManager>();
            foreach (Profile profile in ProfileManager.Profiles) {
                LoginData data = null;
                Credentials.TryGetValue(profile, out data);
                loginManager.UpdateCredentials(profile, data);
            }
            Credentials.Clear();

            EnvironmentManager.Save();
            Close();
        }

        #endregion Global Actions Section

        #region Service

        private void UsernameChanged(object sender, TextChangedEventArgs e) {
            if (IsPreventLoginChange) {
                return;
            }
            LoginData login = null;
            if (Credentials.TryGetValue(SelectedProfile, out login)) {
                login.User = tbUser.Text;
            }
        }

        private void PasswordChanged(object sender, RoutedEventArgs e) {
            if (IsPreventLoginChange) {
                return;
            }
            LoginData login = null;
            if (Credentials.TryGetValue(SelectedProfile, out login)) {
                login.SecurePassword = pbPass.SecurePassword;
            }
        }

        private void LauncherHelp_Loaded(object sender, RoutedEventArgs e) {
            Run run = sender as Run;
            LanguageManager.LanguageChanged += (s, e2) => {
                run.Text = LanguageManager.Model.Settings_AppLocale_Help;
            };
        }

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
            if (tbGamePath == null || tbLauncherPath == null) {
                return;
            }
            tbGamePath.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbLauncherPath.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        #endregion Validation
    }
}