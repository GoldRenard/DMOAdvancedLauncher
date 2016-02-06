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
using System.Security;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Web;
using AdvancedLauncher.Tools;
using HtmlAgilityPack;

namespace AdvancedLauncher.Providers {

    internal abstract class AbstractLoginProvider : CrossDomainObject, ILoginProvider {
        protected bool IsCancelled = false;

        protected string UserId;

        protected SecureString Password;

        protected int LoginAttemptNum, StartAttempt = 0, LastError = -1;

        public event LoginCompleteEventHandler LoginCompleted;

        public event LoginStateEventHandler LoginStateChanged;

        protected System.Windows.Forms.WebBrowser Browser = new System.Windows.Forms.WebBrowser() {
            ScriptErrorsSuppressed = true
        };

        protected ILogManager LogManager {
            get;
            private set;
        }

        protected bool IsManual {
            get;
            set;
        }

        public AbstractLoginProvider() {
        }

        public AbstractLoginProvider(ILogManager logManager) {
            Initialize(logManager);
        }

        public void Initialize(ILogManager logManager) {
            this.LogManager = logManager;
        }

        public void TryManualLogin(string UserId, string Password) {
            IsManual = true;
            TryLogin(UserId, PassEncrypt.ConvertToSecureString(Password));
        }

        public void TryLogin(string UserId, string Password) {
            IsManual = false;
            TryLogin(UserId, PassEncrypt.ConvertToSecureString(Password));
        }

        public abstract void TryLogin(string UserId, SecureString Password);

        protected void TryParseInfo(string content) {
            if (IsCancelled) {
                OnCompleted(LoginCode.CANCELLED, string.Empty, UserId);
                return;
            }
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            string resultText = doc.DocumentNode.SelectSingleNode("//body").InnerText;

            resultText = resultText.Replace("\r\n-\r\n", "");
            resultText = resultText.Replace("\r\n", "");
            resultText = System.Net.WebUtility.HtmlDecode(resultText);

            HtmlDocument result = new HtmlDocument();
            result.LoadHtml(resultText);

            HtmlNode resultNode = result.DocumentNode.SelectSingleNode("//result");

            if (resultNode == null) {
                OnCompleted(LoginCode.UNKNOWN_URL, string.Empty, UserId);
                return;
            }

            int resultCode = Convert.ToInt32(resultNode.Attributes["value"].Value);
            string Args = string.Empty;
            if (resultCode == 0) {
                foreach (HtmlNode node in result.DocumentNode.SelectNodes("//param")) {
                    try {
                        Args += node.Attributes["value"].Value + " ";
                    } catch {
                    };
                }
                OnCompleted(LoginCode.SUCCESS, Args, UserId);
            } else {
                LastError = resultCode;
                StartAttempt++;
                TryLogin(UserId, Password);
            }
        }

        protected virtual void OnCompleted(LoginCode code, string arguments, string UserId) {
            LoginCompleteEventArgs args = new LoginCompleteEventArgs(code, arguments, UserId);
            if (LogManager != null) {
                LogManager.InfoFormat("Logging in completed: code={0}, result=\"{1}\", user={2}", code, arguments, UserId);
            }
            if (LoginCompleted != null) {
                LoginCompleted(this, args);
            }
        }

        protected virtual void OnStateChanged(LoginState state) {
            if (LogManager != null) {
                LogManager.InfoFormat("Logging state changed: state={0}", state);
            }
            if (LoginStateChanged != null) {
                LoginStateChanged(this, new LoginStateEventArgs(state, StartAttempt + 1, LastError));
            }
        }

        public void CancelLogin() {
            IsCancelled = true;
        }
    }
}