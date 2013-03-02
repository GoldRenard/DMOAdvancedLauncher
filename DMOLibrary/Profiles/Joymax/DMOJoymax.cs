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
using System.Diagnostics;

namespace DMOLibrary.Profiles.Joymax
{
    public class DMOJoymax : DMOProfile
    {
        private void InitVars()
        {
            TYPE_NAME = "Joymax";
            GAME_EXE = "GDMO.exe";
            LAUNCHER_EXE = "DMLauncher.exe";
            LOCAL_VERSION_FILE = @"\LauncherLib\vGDMO.ini";
            REMOTE_VERSION_FILE = "http://patch.dmo.joymax.com/PatchInfo_GDMO.ini";
            PATCHES_URL = "http://patch.dmo.joymax.com/GDMO{0}.zip";
            REGEDIT_GAME_KEY = "Software\\Joymax\\DMO";

            IsWebSupported = true;
            IsUpdateSupported = true;
            IsLoginRequired = false;
            IsNewsSupported = true;
            IsSeparateLauncher = false;

            ReadSettings();

            DMODatabase.CREATE_DATABASE_QUERY += @"
            INSERT INTO Servers([name]) VALUES ('Leviamon');
            INSERT INTO Servers([name]) VALUES ('Lucemon');
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
        public DMOJoymax()
        {
            InitVars();
        }

        public DMOJoymax(System.Windows.Threading.Dispatcher owner_dispatcher)
        {
            this.owner_dispatcher = owner_dispatcher;
            InitVars();
        }

        public DMOJoymax(string profile)
        {
            if (profile != string.Empty)
                PROFILE_NAME = profile;
            InitVars();
        }

        public DMOJoymax(System.Windows.Threading.Dispatcher owner_dispatcher, string profile)
        {
            this.owner_dispatcher = owner_dispatcher;
            if (profile != string.Empty)
                PROFILE_NAME = profile;
            InitVars();
        }
        #endregion

        #region Getting user login commandline
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
                string args = "true";
                string EXE = string.Format(PATH_FORMAT, GetGamePath(), LAUNCHER_EXE);
                if (ApplicationLauncher.Execute(string.Format(PATH_FORMAT, GetGamePath(), GAME_EXE), args, S_USE_APPLOC))
                    OnCompleted(LoginCode.SUCCESS, args);
                else
                    OnCompleted(LoginCode.EXECUTE_ERROR, EXE);
            }
        }

        public override void LastSessionStart()
        {
            GameStart();
        }
        #endregion

        public override DMOWebProfile GetWebProfile()
        {
            return new JMWebInfo(Database);
        }

        public override DMONewsProfile GetNewsProfile()
        {
            return new JMNews();
        }
    }
}