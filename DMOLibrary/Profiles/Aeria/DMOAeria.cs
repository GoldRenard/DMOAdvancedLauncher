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
            GAME_EXE = "GDMO.exe";
            LAUNCHER_EXE = "DMLauncher.exe";
            LOCAL_VERSION_FILE = @"\LauncherLib\vGDMO.ini";
            REMOTE_VERSION_FILE = "http://patch.dmo.joymax.com/Aeria/PatchInfo_GDMO.ini";
            PATCHES_URL = "http://patch.dmo.joymax.com/Aeria/GDMO{0}.zip";
            REGEDIT_GAME_KEY = "Software\\Aeria Games\\DMO";
            S_ROTATION_GNAME = "VirusBusters";

            IsWebSupported = false;
            IsUpdateSupported = true;
            IsLoginRequired = true;
            IsNewsSupported = true;
            IsSeparateLauncher = false;

            ReadSettings();

            DMODatabase.CREATE_DATABASE_QUERY += @"INSERT INTO Servers([name]) VALUES ('Seraphimon');";
            Database = new DMODatabase(GetDatabasePath());
            if (Database.OpenConnection())
            {
                ServerList = Database.GetServers();
                Database.CloseConnection();
            }
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

        public DMOAeria(string profile)
        {
            if (profile != string.Empty)
                PROFILE_NAME = profile;
            InitVars();
        }

        public DMOAeria(System.Windows.Threading.Dispatcher owner_dispatcher, string profile)
        {
            this.owner_dispatcher = owner_dispatcher;
            if (profile != string.Empty)
                PROFILE_NAME = profile;
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
                            wb.Document.GetElementById("edit-id").SetAttribute("value", USER_ID);
                            wb.Document.GetElementById("edit-pass").SetAttribute("value", USER_PASSWORD);
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
                        OnChanged(LoginState.GETTING_DATA);
                        LastSessionArgs = string.Format("{0} {1} {2}", "Aeria", HttpUtility.ParseQueryString(e.Url.Query).Get("code"), USER_ID);
                        LastSessionStart();
                        break;
                    }
                default:
                    {
                        OnCompleted(LoginCode.UNKNOWN_URL, string.Empty);
                        break;
                    }
            }
        }

        public override void GameStart()
        {
            if (IsUpdateNeeded)
            {
                string EXE = string.Format(PATH_FORMAT, GetGamePath(), LAUNCHER_EXE);
                if (ApplicationLauncher.Execute(EXE, S_USE_APPLOC))
                    OnCompleted(LoginCode.SUCCESS, string.Empty);
                else
                    OnCompleted(LoginCode.EXECUTE_ERROR, EXE);
            }
            else
            {
                if (USER_ID.Length == 0 || USER_PASSWORD.Length == 0)
                {
                    OnCompleted(LoginCode.WRONG_USER, string.Empty);
                    return;
                }

                login_try = 0;
                wb.DocumentCompleted += LoginDocumentCompleted;
                wb.Navigate("http://www.aeriagames.com/dialog/oauth?response_type=code&client_id=f24233f2506681f0ba2022418e6a5b44050b5216f&https://agoa-dmo.joymax.com/code2token.html&&state=xyz");
                OnChanged(LoginState.LOGINNING);
            }
        }

        public override void LastSessionStart()
        {
            if (LastSessionArgs.Length == 0)
                return;
            if (IsUpdateNeeded)
            {
                string EXE = string.Format(PATH_FORMAT, GetGamePath(), LAUNCHER_EXE);
                if (ApplicationLauncher.Execute(EXE, S_USE_APPLOC))
                    OnCompleted(LoginCode.SUCCESS, string.Empty);
                else
                    OnCompleted(LoginCode.EXECUTE_ERROR, EXE);
            }
            else
            {
                string EXE = string.Format(PATH_FORMAT, GetGamePath(), GAME_EXE);
                if (ApplicationLauncher.Execute(EXE, LastSessionArgs, S_USE_APPLOC))
                    OnCompleted(LoginCode.SUCCESS, LastSessionArgs);
                else
                    OnCompleted(LoginCode.EXECUTE_ERROR, EXE);
            }
        }
        #endregion

        public override DMOWebProfile GetWebProfile()
        {
            return new DMOLibrary.Profiles.Joymax.JMWebInfo(Database);
        }

        public override DMONewsProfile GetNewsProfile()
        {
            return new DMOLibrary.Profiles.Joymax.JMNews();
        }
    }
}