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

namespace DMOLibrary.Profiles.Korea
{
    public class DMOKoreaIMBC : DMOProfile
    {
        private void InitVars()
        {
            TYPE_NAME = "KoreaIMBC";
            GAME_EXE = "DigimonMasters.exe";
            LAUNCHER_EXE = "D-Player.exe";
            LOCAL_VERSION_FILE = @"\LauncherLib\vDMO.ini";
            REMOTE_VERSION_FILE = "http://digimonmasters.nowcdn.co.kr/s1/PatchInfo.ini";
            PATCHES_URL = "http://digimonmasters.nowcdn.co.kr/s1/{0}.zip";
            REGEDIT_GAME_KEY = "Software\\Digitalic\\DigimonMasters";
            REGEDIT_LAUNCHER_KEY = "Software\\Digitalic\\Launcher";
            S_ROTATION_GNAME = "VirusBusters";

            IsWebSupported = true;
            IsUpdateSupported = true;
            IsLoginRequired = true;
            IsNewsSupported = false;
            IsSeparateLauncher = true;
            IsLastSessionAvailable = true;

            ReadSettings();

            DATABASE_NAME = "Korea";
            DMODatabase.CREATE_DATABASE_QUERY += @"
            INSERT INTO Servers([name]) VALUES ('Lucemon');
            INSERT INTO Servers([name]) VALUES ('Leviamon');
            INSERT INTO Servers([name]) VALUES ('Lilithmon');
            INSERT INTO Servers([name]) VALUES ('Barbamon');";
            Database = new DMODatabase(GetDatabasePath());
            if (Database.OpenConnection())
            {
                ServerList = Database.GetServers();
                Database.CloseConnection();
            }
        }

        #region Constructors
        public DMOKoreaIMBC()
        {
            InitVars();
        }

        public DMOKoreaIMBC(System.Windows.Threading.Dispatcher owner_dispatcher)
        {
            this.owner_dispatcher = owner_dispatcher;
            InitVars();
        }

        public DMOKoreaIMBC(string profile)
        {
            if (profile != string.Empty)
                PROFILE_NAME = profile;
            InitVars();
        }

        public DMOKoreaIMBC(System.Windows.Threading.Dispatcher owner_dispatcher, string profile)
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
                case "/RealMedia/ads/adstream_sx.ads/www.imbc.com/Login@Middle":
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
                            wb.Document.GetElementsByTagName("input").GetElementsByName("Uid")[0].SetAttribute("value", USER_ID);
                            wb.Document.GetElementsByTagName("input").GetElementsByName("Password")[0].SetAttribute("value", USER_PASSWORD);
                        }
                        catch { isFound = false; }

                        if (isFound)
                        {
                            System.Windows.Forms.HtmlElement form = wb.Document.GetElementById("frmLogin");
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
                case "/Counsel/PasswordModify90Days.aspx":
                case "/":
                    {
                        OnChanged(LoginState.GETTING_DATA);
                        wb.Navigate("http://dm.imbc.com/inc/xml/launcher.aspx");
                        break;
                    }
                //getting data
                case "/inc/xml/launcher.aspx":
                    {
                        TryParseInfo(wb.DocumentText);
                        break;
                    }
                default:
                    break;
            }
        }

        public override void GameStart()
        {
            if (USER_ID.Length == 0 || USER_PASSWORD.Length == 0)
            {
                OnCompleted(LoginCode.WRONG_USER, string.Empty);
                return;
            }

            login_try = 0;
            if (wb != null)
                wb.Dispose();
            wb = new System.Windows.Forms.WebBrowser() { ScriptErrorsSuppressed = true };
            wb.DocumentCompleted += LoginDocumentCompleted;
            wb.Navigate("http://member.imbc.com/Login/Login.aspx");
            OnChanged(LoginState.LOGINNING);
        }

        public override void LastSessionStart()
        {
            if (LastSessionArgs.Length == 0)
                return;
            if (IsUpdateNeeded)
            {
                string EXE = string.Format(PATH_FORMAT, GetLauncherPath(), LAUNCHER_EXE);
                if (ApplicationLauncher.Execute(EXE, LastSessionArgs, S_USE_APPLOC))
                    OnCompleted(LoginCode.SUCCESS, string.Empty);
                else
                    OnCompleted(LoginCode.EXECUTE_ERROR, EXE);
            }
            else
            {
                string EXE = string.Format(PATH_FORMAT, GetGamePath(), GAME_EXE);
                if (ApplicationLauncher.Execute(EXE, LastSessionArgs.Replace(" 1 ", " "), S_USE_APPLOC))
                    OnCompleted(LoginCode.SUCCESS, LastSessionArgs);
                else
                    OnCompleted(LoginCode.EXECUTE_ERROR, EXE);
            }
        }
        #endregion

        public override DMOWebProfile GetWebProfile()
        {
            return new KoreaWebInfo(Database);
        }

        public override DMONewsProfile GetNewsProfile()
        {
            return new DMOLibrary.Profiles.Joymax.JMNews();
        }
    }
}