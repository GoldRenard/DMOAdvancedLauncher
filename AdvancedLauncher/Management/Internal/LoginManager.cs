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
using System.Collections.Concurrent;
using System.Collections.Generic;
using AdvancedLauncher.Model.Protected;
using AdvancedLauncher.Model.Proxy;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Web;
using AdvancedLauncher.Tools;
using AdvancedLauncher.UI.Windows;
using MahApps.Metro.Controls.Dialogs;
using Ninject;

namespace AdvancedLauncher.Management.Internal {

    internal sealed class LoginManager : ILoginProviderEventAccessor {
        private HashSet<string> failedLogin = new HashSet<string>();

        private ProgressDialogController controller;

        public event LoginCompleteEventHandler LoginCompleted;

        private ConcurrentDictionary<Profile, LoginData> CredentialsCollection = new ConcurrentDictionary<Profile, LoginData>();

        private LoginProviderEventAccessor<LoginManager> eventAccessor;

        [Inject]
        public ILanguageManager LanguageManager {
            get; set;
        }

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        [Inject]
        public IConfigurationManager ConfigurationManager {
            get; set;
        }

        public LoginManager() {
            eventAccessor = new LoginProviderEventAccessor<LoginManager>(this);
        }

        public void Login(Profile profile) {
            if (profile == null) {
                throw new ArgumentException("profile argument cannot be null");
            }
            LoginData credentials = GetCredentials(profile);
            if (credentials != null) {
                if (ConfigurationManager.GetConfiguration(profile.GameModel).IsLastSessionAvailable && !string.IsNullOrEmpty(credentials.LastSessionArgs)) {
                    ShowLastSessionDialog(profile);
                    return;
                }
                if (PerformLogin(credentials)) {
                    return;
                }
            }
            ShowLoginDialog(LanguageManager.Model.LoginLogIn, String.Empty, credentials.User);
        }

        private async void ShowLoginDialog(string title, string message, string initUserName) {
            MainWindow MainWindow = App.Kernel.Get<MainWindow>();
            LoginDialogData result = await MainWindow.ShowLoginAsync(title, message, new LoginDialogSettings {
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
                LoginCompleted(this, new LoginCompleteEventArgs(LoginCode.CANCELLED, string.Empty, result != null ? result.Username : string.Empty));
            }
        }

        private async void ShowLoggingInDialog(LoginDialogData loginData) {
            MainWindow MainWindow = App.Kernel.Get<MainWindow>();
            GameModel model = ProfileManager.CurrentProfile.GameModel;
            ILoginProvider loginProvider = ConfigurationManager.GetConfiguration(model).CreateLoginProvider();
            MetroDialogSettings settings = new MetroDialogSettings() {
                ColorScheme = MetroDialogColorScheme.Accented
            };
            controller = await MainWindow.ShowProgressAsync(LanguageManager.Model.LoginLogIn, String.Empty, true, settings);
            loginProvider.LoginStateChanged += eventAccessor.OnLoginStateChanged;
            loginProvider.LoginCompleted += eventAccessor.OnLoginCompleted;
            loginProvider.TryLogin(loginData.Username, loginData.Password);
        }

        private async void ShowLastSessionDialog(Profile profile) {
            MainWindow MainWindow = App.Kernel.Get<MainWindow>();
            MessageDialogResult result = await MainWindow.ShowMessageAsync(LanguageManager.Model.UseLastSession, string.Empty,
                MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() {
                    AffirmativeButtonText = LanguageManager.Model.Yes,
                    NegativeButtonText = LanguageManager.Model.No,
                    ColorScheme = MetroDialogColorScheme.Accented
                });

            LoginData credentials = GetCredentials(profile);
            if (credentials != null) {
                if (result == MessageDialogResult.Affirmative) {
                    if (LoginCompleted != null) {
                        LoginCompleted(this, new LoginCompleteEventArgs(LoginCode.SUCCESS,
                            credentials.LastSessionArgs, credentials.User));
                    }
                    return;
                } else if (PerformLogin(credentials)) {
                    return;
                }
            }
            ShowLoginDialog(LanguageManager.Model.LoginLogIn, String.Empty, credentials.User);
        }

        public async void OnLoginCompleted(object sender, LoginCompleteEventArgs e) {
            await controller.CloseAsync();
            if (e.Code == LoginCode.WRONG_USER) {
                failedLogin.Add(e.UserName);
                ShowLoginDialog(LanguageManager.Model.LoginLogIn, LanguageManager.Model.LoginBadAccount, string.Empty);
                return;
            }
            if (LoginCompleted != null) {
                LoginCompleted(sender, e);
            }
        }

        public void OnLoginStateChanged(object sender, LoginStateEventArgs e) {
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

            if (controller.IsCanceled) {
                ILoginProvider loginProvider = sender as ILoginProvider;
                if (loginProvider != null) {
                    loginProvider.CancelLogin();
                }
            }
        }

        public bool UpdateCredentials(Profile profile, LoginData data) {
            return CredentialsCollection.AddOrUpdate(profile, data, (key, oldValue) => data).Equals(data);
        }

        public bool UpdateLastSessionArgs(Profile profile, string args) {
            LoginData data = GetCredentials(profile);
            if (data != null) {
                data.LastSessionArgs = args;
            }
            return data != null;
        }

        public LoginData GetCredentials(Profile profile) {
            LoginData data = null;
            if (CredentialsCollection.TryGetValue(profile, out data)) {
                return data;
            }
            return null;
        }

        private bool PerformLogin(LoginData credentials) {
            if (!failedLogin.Contains(credentials.User)) {
                if (credentials.IsCorrect) {
                    LoginDialogData loginData = new LoginDialogData() {
                        Username = credentials.User,
                        Password = PassEncrypt.ConvertToUnsecureString(credentials.SecurePassword)
                    };
                    ShowLoggingInDialog(loginData);
                    return true;
                }
            }
            return false;
        }
    }
}