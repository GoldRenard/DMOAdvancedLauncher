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

using System.Security;

namespace DMOLibrary.Profiles.Korea {

    public class DMOKorea : DMOProfile {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(DMOKorea));

        private void InitVars() {
            typeName = "Korea";
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

        public DMOKorea() {
            InitVars();
        }

        public DMOKorea(System.Windows.Threading.Dispatcher ownerDispatcher) {
            this.OwnerDispatcher = ownerDispatcher;
            InitVars();
        }

        #endregion Constructors

        #region Getting user login commandline

        public virtual void LoginDocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e) {
            LOGGER.InfoFormat("Document requested: {0}", e.Url.OriginalString);
            switch (e.Url.AbsolutePath) {
                //loginning
                case "/help/Login/MemberLogin.aspx": {
                        if (loginTryNum >= 1) {
                            OnCompleted(LoginCode.WRONG_USER, string.Empty);
                            return;
                        }
                        loginTryNum++;

                        bool isFound = true;
                        try {
                            wb.Document.GetElementById("security_name").SetAttribute("value", UserId);
                            wb.Document.GetElementById("security_code").SetAttribute("value", SecureStringConverter.ConvertToUnsecureString(Password));
                        } catch {
                            isFound = false;
                        }

                        if (isFound) {
                            System.Windows.Forms.HtmlElement form = wb.Document.GetElementById("login");
                            if (form != null) {
                                form.InvokeMember("Click");
                            }
                        } else {
                            OnCompleted(LoginCode.WRONG_PAGE, string.Empty);
                            return;
                        }
                        break;
                    }
                //logged
                case "/index.aspx": {
                        OnChanged(LoginState.GETTING_DATA);
                        wb.Navigate("http://www.digimonmasters.com/inc/xml/launcher.aspx");
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
            wb = new System.Windows.Forms.WebBrowser() {
                ScriptErrorsSuppressed = true
            };
            wb.DocumentCompleted += LoginDocumentCompleted;
            wb.Navigate("http://www.digimonmasters.com/help/Login/MemberLogin.aspx");
            OnChanged(LoginState.LOGINNING);
        }

        #endregion Getting user login commandline

        public override string GetGameStartArgs(string args) {
            return args.Replace(" 1 ", " ");
        }

        public override string GetLauncherStartArgs(string args) {
            return args;
        }
    }
}