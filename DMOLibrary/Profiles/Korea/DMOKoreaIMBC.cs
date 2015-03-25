// ======================================================================
// DMOLibrary
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
using DMOLibrary.Database.Entity;
using DMOLibrary.Events;

namespace DMOLibrary.Profiles.Korea {

    public class DMOKoreaIMBC : DMOKorea {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(DMOKoreaIMBC));

        public DMOKoreaIMBC()
            : base(Server.ServerType.KDMO_IMBC, "KoreaIMBC") {
        }

        #region Getting user login commandline

        public override void LoginDocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e) {
            LOGGER.InfoFormat("Document requested: {0}", e.Url.OriginalString);
            switch (e.Url.AbsolutePath) {
                //loginning
                case "/RealMedia/ads/adstream_sx.ads/www.imbc.com/Login@Middle": {
                        if (LoginTryNum >= 1) {
                            OnCompleted(LoginCode.WRONG_USER, string.Empty);
                            return;
                        }
                        LoginTryNum++;

                        bool isFound = true;
                        try {
                            wb.Document.GetElementsByTagName("input").GetElementsByName("Uid")[0].SetAttribute("value", UserId);
                            wb.Document.GetElementsByTagName("input").GetElementsByName("Password")[0].SetAttribute("value", SecureStringConverter.ConvertToUnsecureString(Password));
                        } catch {
                            isFound = false;
                        }

                        if (isFound) {
                            System.Windows.Forms.HtmlElement form = wb.Document.GetElementById("frmLogin");
                            if (form != null) {
                                form.InvokeMember("submit");
                            }
                        } else {
                            OnCompleted(LoginCode.WRONG_PAGE, string.Empty);
                            return;
                        }
                        break;
                    }
                //logged
                case "/Counsel/PasswordModify90Days.aspx":
                case "/": {
                        OnChanged(LoginState.GETTING_DATA);
                        wb.Navigate("http://dm.imbc.com/inc/xml/launcher.aspx");
                        break;
                    }
                //getting data
                case "/inc/xml/launcher.aspx": {
                        TryParseInfo(wb.DocumentText);
                        break;
                    }
                default:
                    break;
            }
        }

        public override void TryLogin(string UserId, SecureString Password) {
            this.UserId = UserId;
            this.Password = Password;
            if (UserId.Length == 0 || Password.Length == 0) {
                OnCompleted(LoginCode.WRONG_USER, string.Empty);
                return;
            }

            LoginTryNum = 0;
            if (wb != null)
                wb.Dispose();
            wb = new System.Windows.Forms.WebBrowser() {
                ScriptErrorsSuppressed = true
            };
            wb.DocumentCompleted += LoginDocumentCompleted;
            wb.Navigate("http://member.imbc.com/Login/Login.aspx");
            OnChanged(LoginState.LOGINNING);
        }

        #endregion Getting user login commandline
    }
}