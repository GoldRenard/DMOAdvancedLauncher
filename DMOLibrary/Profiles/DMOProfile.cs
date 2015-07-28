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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Web;
using DMOLibrary.Database.Context;
using HtmlAgilityPack;

namespace DMOLibrary.Profiles {

    public abstract class DMOProfile : IGameProfile {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(DMOProfile));
        protected INewsProfile _NewsProfile = null;
        protected ObservableCollection<Server> _ServerList;

        private readonly string TypeName;

        private readonly Server.ServerType ServerType;

        public DMOProfile(Server.ServerType serverType, string typeName) {
            this.TypeName = typeName;
            this.ServerType = serverType;
        }

        #region Game start

        protected string UserId;
        protected SecureString Password;
        protected int LoginTryNum, StartTry = 0, LastError = -1;

        protected System.Windows.Forms.WebBrowser wb = new System.Windows.Forms.WebBrowser() {
            ScriptErrorsSuppressed = true
        };

        public abstract void TryLogin(string UserId, SecureString Password);

        public event LoginCompleteEventHandler LoginCompleted;

        public event LoginStateEventHandler LoginStateChanged;

        protected virtual void OnCompleted(LoginCode code, string arguments, string UserId) {
            LoginCompleteEventArgs args = new LoginCompleteEventArgs(code, arguments, UserId);
            LOGGER.InfoFormat("Logging in completed: code={0}, result=\"{1}\", user={2}", code, arguments, UserId);
            if (LoginCompleted != null) {
                LoginCompleted(this, args);
            }
        }

        protected virtual void OnChanged(LoginState state) {
            LOGGER.InfoFormat("Logging state changed: state={0}", state);
            if (LoginStateChanged != null) {
                LoginStateChanged(this, new LoginStateEventArgs(state, StartTry + 1, LastError));
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
                OnCompleted(LoginCode.SUCCESS, Args, UserId);
            } else {
                LastError = res_code;
                StartTry++;
                TryLogin(UserId, Password);
            }
        }

        #endregion Game start

        #region Database Section

        public Server GetServerById(int serverId) {
            return ServerList.Where(i => i.Identifier == serverId).FirstOrDefault();
        }

        public INewsProfile NewsProfile {
            get {
                if (_NewsProfile == null) {
                    _NewsProfile = GetNewsProfile();
                }
                return _NewsProfile;
            }
        }

        public ObservableCollection<Server> ServerList {
            get {
                if (_ServerList == null) {
                    using (MainContext context = new MainContext()) {
                        _ServerList = new ObservableCollection<Server>(context.Servers.Where(i => i.Type == ServerType).ToList());
                    }
                }
                return _ServerList;
            }
        }

        public bool IsWebAvailable {
            get {
                return GetWebProfile() != null;
            }
        }

        public bool IsNewsAvailable {
            get {
                return NewsProfile != null;
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
            return TypeName;
        }

        public virtual IWebProfile GetWebProfile() {
            return null;
        }

        protected virtual INewsProfile GetNewsProfile() {
            return null;
        }

        #endregion Definition
    }
}