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

using System.Security;
using System.Web;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.Tools;
using AdvancedLauncher.UI.Windows;
using Ninject;

namespace AdvancedLauncher.Providers.Aeria {

    internal class AeriaLoginProvider : AbstractLoginProvider {
        private const string AUTH_ENTRY_POINT = "http://www.aeriagames.com/dialog/oauth?response_type=code&client_id=f24233f2506681f0ba2022418e6a5b44050b5216f&https://agoa-dmo.joymax.com/code2token.html&&state=xyz";

        private WebWindow WebWindow;

        public AeriaLoginProvider(ILogManager logManager) : base(logManager) {
        }

        private void LoginDocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e) {
            if (IsManual && WebWindow == null) {
                return;
            }
            if (LogManager != null) {
                LogManager.InfoFormat("Document requested: {0}", e.Url.OriginalString);
            }
            switch (e.Url.AbsolutePath) {
                //loginning
                case "/dialog/oauth":
                    {
                        bool isFound = true;
                        try {
                            if (!string.IsNullOrEmpty(UserId)) {
                                Browser.Document.GetElementById("edit-id").SetAttribute("value", UserId);
                                Browser.Document.GetElementById("edit-pass").SetAttribute("value", PassEncrypt.ConvertToUnsecureString(Password));
                            }
                        } catch {
                            isFound = false;
                        }
                        if (!IsManual) {
                            if (LoginAttemptNum >= 1) {
                                OnCompleted(LoginCode.WRONG_USER, string.Empty, UserId);
                                return;
                            }
                            LoginAttemptNum++;

                            if (isFound) {
                                System.Windows.Forms.HtmlElement form = Browser.Document.GetElementById("account_login");
                                if (form != null) {
                                    form.InvokeMember("submit");
                                }
                            } else {
                                OnCompleted(LoginCode.WRONG_PAGE, string.Empty, UserId);
                                return;
                            }
                        }

                        break;
                    }
                case "/dialog/oauth/authorize":
                    {
                        if (!IsManual) {
                            System.Windows.Forms.HtmlElementCollection links = Browser.Document.GetElementsByTagName("a");
                            foreach (System.Windows.Forms.HtmlElement link in links) {
                                if (link.InnerText.Trim().ToLower().Equals("authorize")) {
                                    link.InvokeMember("click");
                                    break;
                                }
                            }
                            OnCompleted(LoginCode.UNKNOWN_URL, string.Empty, UserId);
                        }
                        break;
                    }
                //logged
                case "/code2token.html":
                    {
                        OnCompleted(LoginCode.SUCCESS, string.Format("{0} {1} {2}", "Aeria", HttpUtility.ParseQueryString(e.Url.Query).Get("code"), UserId), UserId);
                        break;
                    }
                default:
                    {
                        if (!IsManual && !e.Url.Host.Contains("facebook")) {
                            OnCompleted(LoginCode.UNKNOWN_URL, string.Empty, UserId);
                        }
                        break;
                    }
            }
        }

        protected override void OnCompleted(LoginCode code, string arguments, string UserId) {
            if (IsManual && WebWindow != null) {
                WebWindow.Close();
                WebWindow = null;
            }
            base.OnCompleted(code, arguments, UserId);
        }

        public override void TryLogin(string UserId, SecureString Password) {
            this.UserId = UserId;
            this.Password = Password;

            if (!IsManual) {
                if (UserId.Length == 0 || Password.Length == 0) {
                    OnCompleted(LoginCode.WRONG_USER, string.Empty, UserId);
                    return;
                }
            } else {
                WebWindow = App.Kernel.Get<WebWindow>();
                WebWindow.Title = App.Kernel.Get<ILanguageManager>().Model.Settings_Account_Manual_Auth;
                WebWindow.Closed += (s, e) => OnCompleted(LoginCode.CANCELLED, string.Empty, UserId);
                Browser = WebWindow.Browser;
            }

            LoginAttemptNum = 0;
            Browser.DocumentCompleted += LoginDocumentCompleted;
            Browser.Navigate(AUTH_ENTRY_POINT);
            OnStateChanged(LoginState.LOGINNING);
            if (IsManual) {
                WebWindow.ShowDialog();
            }
        }

        private void Browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e) {
            return;
        }
    }
}