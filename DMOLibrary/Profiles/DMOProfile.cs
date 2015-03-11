// ======================================================================
// DMOLibrary
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using DMOLibrary.Database.Context;
using DMOLibrary.Database.Entity;
using HtmlAgilityPack;

namespace DMOLibrary.Profiles {

    public abstract class DMOProfile {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(DMOProfile));
        private string DATABASES_FOLDER = "{0}\\Databases";
        protected DMOWebProfile _WebProfile = null;
        protected DMONewsProfile _NewsProfile = null;
        protected ObservableCollection<Server> _ServerList;
        public DMODatabase Database;

        private readonly string typeName;

        public DMOProfile(Server.ServerType serverType, string typeName) {
            this.typeName = typeName;
            string dir = string.Format(DATABASES_FOLDER, System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory));
            if (!Directory.Exists(dir)) {
                try {
                    Directory.CreateDirectory(dir);
                } catch {
                }
            }
            using (MainContext context = new MainContext()) {
                _ServerList = new ObservableCollection<Server>(context.Servers.Where(i => i.Type == serverType).ToList());
            }
            Database = new DMODatabase(GetDatabasePath());
        }

        #region Game start

        protected string UserId;
        protected SecureString Password;
        protected int loginTryNum, start_try = 0, last_error = -1;

        protected System.Windows.Forms.WebBrowser wb = new System.Windows.Forms.WebBrowser() {
            ScriptErrorsSuppressed = true
        };

        public abstract void TryLogin(string UserId, SecureString Password);

        public delegate void LoginCompleteHandler(object sender, LoginCode code, string result);

        public event LoginCompleteHandler LoginCompleted;

        public delegate void LoginStateHandler(object sender, LoginState state, int try_num, int last_error);

        public event LoginStateHandler LoginStateChanged;

        protected virtual void OnCompleted(LoginCode code, string result) {
            LOGGER.InfoFormat("Logging in completed: code={0}, result=\"{1}\"", code, result);
            if (LoginCompleted != null) {
                LoginCompleted(this, code, result);
            }
        }

        protected virtual void OnChanged(LoginState state) {
            LOGGER.InfoFormat("Logging state changed: state={0}", state);
            if (LoginStateChanged != null) {
                LoginStateChanged(this, state, start_try + 1, last_error);
            }
        }

        protected void TryParseInfo(string content) {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            string result_text = doc.DocumentNode.SelectSingleNode("//body").InnerText;

            result_text = result_text.Replace("\r\n-\r\n", "");
            result_text = result_text.Replace("\r\n", "");
            result_text = System.Net.WebUtility.HtmlDecode(result_text);

            HtmlDocument result = new HtmlDocument();
            result.LoadHtml(result_text);

            int res_code = Convert.ToInt32(result.DocumentNode.SelectSingleNode("//result").Attributes["value"].Value);
            string Args = string.Empty;
            if (res_code == 0) {
                foreach (HtmlNode node in result.DocumentNode.SelectNodes("//param")) {
                    try {
                        Args += node.Attributes["value"].Value + " ";
                    } catch {
                    };
                }
                OnCompleted(LoginCode.SUCCESS, Args);
            } else {
                last_error = res_code;
                start_try++;
                TryLogin(UserId, Password);
            }
        }

        #endregion Game start

        #region Database Section

        public string GetDatabasePath() {
            String fileName = GetTypeName();
            if (GetDatabaseName() != null) {
                fileName = GetDatabaseName();
            }
            return string.Format(DATABASES_FOLDER + "\\{1}.sqlite", System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory), fileName);
        }

        public Server GetServerById(int serverId) {
            return _ServerList.Where(i => i.Identifier == serverId).FirstOrDefault();
        }

        public DMOWebProfile WebProfile {
            get {
                if (_WebProfile == null) {
                    _WebProfile = GetWebProfile();
                }
                return _WebProfile;
            }
        }

        public DMONewsProfile NewsProfile {
            get {
                if (_NewsProfile == null) {
                    _NewsProfile = GetNewsProfile();
                }
                return _NewsProfile;
            }
        }

        public ObservableCollection<Server> ServerList {
            get {
                return _ServerList;
            }
        }

        public bool IsWebAvailable {
            get {
                return _WebProfile != null;
            }
        }

        public bool IsNewsAvailable {
            get {
                return _NewsProfile != null;
            }
        }

        public virtual bool IsLoginRequired {
            get {
                return false;
            }
        }

        #endregion Database Section

        #region Definition

        public abstract string GetGameStartArgs(string args);

        public abstract string GetLauncherStartArgs(string args);

        public string GetTypeName() {
            return typeName;
        }

        protected virtual string GetDatabaseName() {
            return null;
        }

        protected virtual DMOWebProfile GetWebProfile() {
            return null;
        }

        protected virtual DMONewsProfile GetNewsProfile() {
            return null;
        }

        #endregion Definition
    }
}