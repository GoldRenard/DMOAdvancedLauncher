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
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Management.Security;
using AdvancedLauncher.Model.Config;
using AdvancedLauncher.UI.Windows;
using DMOLibrary.Events;
using DMOLibrary.Profiles;
using MahApps.Metro.Controls.Dialogs;
using Ninject;

namespace AdvancedLauncher.Management {

    internal sealed class LoginManager : ILoginManager {
        private List<DMOProfile> failedLogin = new List<DMOProfile>();

        private ProgressDialogController controller;

        public event LoginCompleteEventHandler LoginCompleted;

        [Inject]
        public ILanguageManager LanguageManager {
            get; set;
        }

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        public void Login() {
            Login(ProfileManager.CurrentProfile);
        }

        public void Login(Profile profile) {
            if (!failedLogin.Contains(GameManager.GetProfile(profile.GameModel.Type))) {
                if (GameManager.Current.IsLastSessionAvailable() && !string.IsNullOrEmpty(profile.Login.LastSessionArgs)) {
                    ShowLastSessionDialog(profile);
                    return;
                }
                if (profile.Login.IsCorrect) {
                    LoginDialogData loginData = new LoginDialogData() {
                        Username = profile.Login.User,
                        Password = PassEncrypt.ConvertToUnsecureString(profile.Login.SecurePassword)
                    };
                    ShowLoggingInDialog(loginData);
                    return;
                }
            }
            ShowLoginDialog(LanguageManager.Model.LoginLogIn, String.Empty, profile.Login.User);
        }

        private async void ShowLoginDialog(string title, string message, string initUserName) {
            LoginDialogData result = await MainWindow.Instance.ShowLoginAsync(title, message, new LoginDialogSettings {
                ColorScheme = MetroDialogColorScheme.Accented,
                InitialUsername = initUserName,
                NegativeButtonVisibility = System.Windows.Visibility.Visible,
                NegativeButtonText = LanguageManager.Model.CancelButton,
                UsernameWatermark = LanguageManager.Model.Settings_Account_User,
                PasswordWatermark = LanguageManager.Model.Settings_Account_Password,
                AffirmativeButtonText = LanguageManager.Model.LogInButton
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
            DMOProfile dmoProfile = GameManager.CurrentProfile;
            MetroDialogSettings settings = new MetroDialogSettings() {
                ColorScheme = MetroDialogColorScheme.Accented
            };
            controller = await MainWindow.Instance.ShowProgressAsync(LanguageManager.Model.LoginLogIn, String.Empty, false, settings);
            dmoProfile.LoginStateChanged += OnLoginStateChanged;
            dmoProfile.LoginCompleted += OnLoginCompleted;
            dmoProfile.TryLogin(loginData.Username, PassEncrypt.ConvertToSecureString(loginData.Password));
        }

        private async void ShowLastSessionDialog(Profile profile) {
            MessageDialogResult result = await MainWindow.Instance.ShowMessageAsync(LanguageManager.Model.UseLastSession, string.Empty,
                MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() {
                    AffirmativeButtonText = LanguageManager.Model.Yes,
                    NegativeButtonText = LanguageManager.Model.No,
                    ColorScheme = MetroDialogColorScheme.Accented
                });

            if (result == MessageDialogResult.Affirmative) {
                if (LoginCompleted != null) {
                    LoginCompleted(this, new LoginCompleteEventArgs(LoginCode.SUCCESS,
                        profile.Login.LastSessionArgs));
                }
                return;
            }
            ShowLoginDialog(LanguageManager.Model.LoginLogIn, String.Empty, profile.Login.User);
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
                ShowLoginDialog(LanguageManager.Model.LoginLogIn, LanguageManager.Model.LoginBadAccount, string.Empty);
                return;
            }
            if (LoginCompleted != null) {
                LoginCompleted(sender, e);
            }
        }

        private void OnLoginStateChanged(object sender, LoginStateEventArgs e) {
            if (e.Code == LoginState.LOGINNING) {
                controller.SetTitle(LanguageManager.Model.LoginLogIn);
            } else if (e.Code == LoginState.GETTING_DATA) {
                controller.SetTitle(LanguageManager.Model.LoginGettingData);
            }
            string message = string.Format(LanguageManager.Model.LoginTry, e.TryNumber);
            if (e.LastError != -1) {
                message += string.Format(" ({0} {1})", LanguageManager.Model.LoginWasError, e.LastError);
            }
            controller.SetMessage(message);
        }
    }
}