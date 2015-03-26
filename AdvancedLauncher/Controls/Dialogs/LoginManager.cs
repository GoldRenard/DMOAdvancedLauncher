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
using AdvancedLauncher.Environment;
using AdvancedLauncher.Environment.Containers;
using AdvancedLauncher.Service;
using AdvancedLauncher.Windows;
using DMOLibrary.Events;
using DMOLibrary.Profiles;
using MahApps.Metro.Controls.Dialogs;

namespace AdvancedLauncher.Controls.Dialogs {

    internal class LoginManager {
        private static LoginManager _Instance;

        private List<DMOProfile> failedLogin = new List<DMOProfile>();

        private ProgressDialogController controller;

        public event LoginCompleteEventHandler LoginCompleted;

        private LoginManager() {
            // hide the constructod
        }

        public static LoginManager Instance {
            get {
                if (_Instance == null) {
                    _Instance = new LoginManager();
                }
                return _Instance;
            }
        }

        public void Login() {
            Profile profile = LauncherEnv.Settings.CurrentProfile;
            if (!failedLogin.Contains(profile.DMOProfile)) {
                if (profile.GameEnv.IsLastSessionAvailable() && !string.IsNullOrEmpty(profile.Login.LastSessionArgs)) {
                    ShowLastSessionDialog();
                    return;
                }
                if (profile.Login.IsCorrect) {
                    LoginDialogData loginData = new LoginDialogData() {
                        Username = LauncherEnv.Settings.CurrentProfile.Login.User,
                        Password = PassEncrypt.ConvertToUnsecureString(LauncherEnv.Settings.CurrentProfile.Login.SecurePassword)
                    };
                    ShowLoggingInDialog(loginData);
                    return;
                }
            }
            ShowLoginDialog(LanguageEnv.Strings.LoginLogIn, String.Empty);
        }

        private async void ShowLoginDialog(string title, string message) {
            LoginDialogData result = await MainWindow.Instance.ShowLoginAsync(title, message, new LoginDialogSettings {
                ColorScheme = MetroDialogColorScheme.Accented,
                InitialUsername = LauncherEnv.Settings.CurrentProfile.Login.User,
                NegativeButtonVisibility = System.Windows.Visibility.Visible,
                NegativeButtonText = LanguageEnv.Strings.CancelButton,
                UsernameWatermark = LanguageEnv.Strings.Settings_Account_User,
                PasswordWatermark = LanguageEnv.Strings.Settings_Account_Password,
                AffirmativeButtonText = LanguageEnv.Strings.LogInButton
            });
            if (result != null) {
                ShowLoggingInDialog(result);
                return;
            }
            if (LoginCompleted != null) {
                LoginCompleted(this, new LoginCompleteEventArgs(LoginCode.CANCELLED));
            }
        }

        private async void ShowLoggingInDialog(LoginDialogData loginData) {
            DMOProfile dmoProfile = LauncherEnv.Settings.CurrentProfile.DMOProfile;
            MetroDialogSettings settings = new MetroDialogSettings() {
                ColorScheme = MetroDialogColorScheme.Accented
            };
            controller = await MainWindow.Instance.ShowProgressAsync(LanguageEnv.Strings.LoginLogIn, String.Empty, false, settings);
            LauncherEnv.Settings.CurrentProfile.DMOProfile.LoginStateChanged += OnLoginStateChanged;
            LauncherEnv.Settings.CurrentProfile.DMOProfile.LoginCompleted += OnLoginCompleted;
            LauncherEnv.Settings.CurrentProfile.DMOProfile.TryLogin(loginData.Username, PassEncrypt.ConvertToSecureString(loginData.Password));
        }

        private async void ShowLastSessionDialog() {
            MessageDialogResult result = await MainWindow.Instance.ShowMessageAsync(LanguageEnv.Strings.UseLastSession, string.Empty,
                MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() {
                    AffirmativeButtonText = LanguageEnv.Strings.Yes,
                    NegativeButtonText = LanguageEnv.Strings.No,
                    ColorScheme = MetroDialogColorScheme.Accented
                });

            if (result == MessageDialogResult.Affirmative) {
                if (LoginCompleted != null) {
                    LoginCompleted(this, new LoginCompleteEventArgs(LoginCode.SUCCESS,
                        LauncherEnv.Settings.CurrentProfile.Login.LastSessionArgs));
                }
                return;
            }
            ShowLoginDialog(LanguageEnv.Strings.LoginLogIn, String.Empty);
        }

        private async void OnLoginCompleted(object sender, LoginCompleteEventArgs e) {
            await controller.CloseAsync();
            DMOProfile profile = (DMOProfile)sender;
            profile.LoginStateChanged -= OnLoginStateChanged;
            profile.LoginCompleted -= OnLoginCompleted;

            if (e.Code == LoginCode.WRONG_USER) {
                if (!failedLogin.Contains(profile)) {
                    failedLogin.Add(profile);
                }
                ShowLoginDialog(LanguageEnv.Strings.LoginLogIn, LanguageEnv.Strings.LoginBadAccount);
                return;
            }
            if (LoginCompleted != null) {
                LoginCompleted(sender, e);
            }
        }

        private void OnLoginStateChanged(object sender, LoginStateEventArgs e) {
            if (e.Code == LoginState.LOGINNING) {
                controller.SetTitle(LanguageEnv.Strings.LoginLogIn);
            } else if (e.Code == LoginState.GETTING_DATA) {
                controller.SetTitle(LanguageEnv.Strings.LoginGettingData);
            }
            string message = string.Format(LanguageEnv.Strings.LoginTry, e.TryNumber);
            if (e.LastError != -1) {
                message += string.Format(" ({0} {1})", LanguageEnv.Strings.LoginWasError, e.LastError);
            }
            controller.SetMessage(message);
        }
    }
}