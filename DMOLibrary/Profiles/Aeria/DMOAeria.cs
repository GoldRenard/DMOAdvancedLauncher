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

namespace DMOLibrary.Profiles.Aeria
{
    public class DMOAeria : DMOProfile
    {
        private void InitVars()
        {
            TYPE_NAME = "Aeria";
            _IsLoginRequired = true;
            _NewsProfile = new DMOLibrary.Profiles.Joymax.JMNews();
        }

        #region Constructors
        public DMOAeria()
        {
            InitVars();
        }

        public DMOAeria(System.Windows.Threading.Dispatcher owner_dispatcher)
        {
            this.owner_dispatcher = owner_dispatcher;
            InitVars();
        }
        #endregion

        #region Getting user login commandline
        private void LoginDocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            switch (e.Url.AbsolutePath)
            {
                //loginning
                case "/dialog/oauth":
                    {
                        if (login_try >= 1)
                        {
                            OnCompleted(LoginCode.WRONG_USER, string.Empty);
                            return;
                        }
                        login_try++;

                        bool isFound = true;
                        try
                        {
                            wb.Document.GetElementById("edit-id").SetAttribute("value", UserId);
                            wb.Document.GetElementById("edit-pass").SetAttribute("value", SecureStringConverter.ConvertToUnsecureString(Password));
                        }
                        catch { isFound = false; }

                        if (isFound)
                        {
                            System.Windows.Forms.HtmlElement form = wb.Document.GetElementById("account_login");
                            if (form != null)
                                form.InvokeMember("submit");
                        }
                        else
                        {
                            OnCompleted(LoginCode.WRONG_PAGE, string.Empty);
                            return;
                        }
                        break;
                    }
                //logged
                case "/code2token.html":
                    {
                        OnCompleted(LoginCode.SUCCESS, string.Format("{0} {1} {2}", "Aeria", HttpUtility.ParseQueryString(e.Url.Query).Get("code"), UserId));
                        break;
                    }
                default:
                    {
                        if (!e.Url.Host.Contains("facebook"))
                            OnCompleted(LoginCode.UNKNOWN_URL, string.Empty);
                        break;
                    }
            }
        }

        public override void TryLogin(string UserId, SecureString Password)
        {
            this.UserId = UserId;
            this.Password = Password;
            if (UserId.Length == 0 || Password.Length == 0)
            {
                OnCompleted(LoginCode.WRONG_USER, string.Empty);
                return;
            }

            login_try = 0;
            wb.DocumentCompleted += LoginDocumentCompleted;
            wb.Navigate("http://www.aeriagames.com/dialog/oauth?response_type=code&client_id=f24233f2506681f0ba2022418e6a5b44050b5216f&https://agoa-dmo.joymax.com/code2token.html&&state=xyz");
            OnChanged(LoginState.LOGINNING);
        }

        #endregion

        public override string GetGameStartArgs(string args)
        {
            return args;
        }


        public override string GetLauncherStartArgs(string args)
        {
            return string.Empty;
        }
    }
}