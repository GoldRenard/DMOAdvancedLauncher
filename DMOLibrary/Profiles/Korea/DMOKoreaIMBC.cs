// ======================================================================
// DMOLibrary
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Net;
using System.Web;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DMOLibrary.Profiles.Korea {
    public class DMOKoreaIMBC : DMOProfile {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(DMOKoreaIMBC));
        private void InitVars() {
            typeName = "KoreaIMBC";
            databaseName = "Korea";
            _IsLoginRequired = true;

            Database = new DMODatabase(GetDatabasePath(), @"
            INSERT INTO Servers([name]) VALUES ('Lucemon');
            INSERT INTO Servers([name]) VALUES ('Leviamon');
            INSERT INTO Servers([name]) VALUES ('Lilithmon');
            INSERT INTO Servers([name]) VALUES ('Barbamon');");
            if (Database.OpenConnection()) {
                _ServerList = Database.GetServers();
                Database.CloseConnection();
            }
            _WebProfile = new KoreaWebInfo(Database);
        }

        #region Constructors
        public DMOKoreaIMBC() {
            InitVars();
        }

        public DMOKoreaIMBC(System.Windows.Threading.Dispatcher ownerDispatcher) {
            this.OwnerDispatcher = ownerDispatcher;
            InitVars();
        }

        #endregion

        #region Getting user login commandline

        private void LoginDocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e) {
            LOGGER.InfoFormat("Document requested: {0}", e.Url.OriginalString);
            switch (e.Url.AbsolutePath) {
                //loginning
                case "/RealMedia/ads/adstream_sx.ads/www.imbc.com/Login@Middle": {
                        if (loginTryNum >= 1) {
                            OnCompleted(LoginCode.WRONG_USER, string.Empty);
                            return;
                        }
                        loginTryNum++;

                        bool isFound = true;
                        try {
                            wb.Document.GetElementsByTagName("input").GetElementsByName("Uid")[0].SetAttribute("value", UserId);
                            wb.Document.GetElementsByTagName("input").GetElementsByName("Password")[0].SetAttribute("value", SecureStringConverter.ConvertToUnsecureString(Password));
                        } catch { isFound = false; }

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

            loginTryNum = 0;
            if (wb != null)
                wb.Dispose();
            wb = new System.Windows.Forms.WebBrowser() { ScriptErrorsSuppressed = true };
            wb.DocumentCompleted += LoginDocumentCompleted;
            wb.Navigate("http://member.imbc.com/Login/Login.aspx");
            OnChanged(LoginState.LOGINNING);
        }

        #endregion

        public override string GetGameStartArgs(string args) {
            return args.Replace(" 1 ", " ");
        }

        public override string GetLauncherStartArgs(string args) {
            return args;
        }
    }
}