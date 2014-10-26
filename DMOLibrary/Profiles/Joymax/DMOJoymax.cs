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

namespace DMOLibrary.Profiles.Joymax {
    public class DMOJoymax : DMOProfile {
        private void InitVars() {
            typeName = "Joymax";

            Database = new DMODatabase(GetDatabasePath(), @"
            INSERT INTO Servers([name]) VALUES ('Leviamon');
            INSERT INTO Servers([name]) VALUES ('Lucemon');
            INSERT INTO Servers([name]) VALUES ('Lilithmon');
            INSERT INTO Servers([name]) VALUES ('Barbamon');
            INSERT INTO Servers([name]) VALUES ('Beelzemon');");
            if (Database.OpenConnection()) {
                _ServerList = Database.GetServers();
                Database.CloseConnection();
            }
            _WebProfile = new JMWebInfo(Database);
            _NewsProfile = new JMNews();
        }

        #region Constructors
        public DMOJoymax() {
            InitVars();
        }

        public DMOJoymax(System.Windows.Threading.Dispatcher ownerDispatcher) {
            this.OwnerDispatcher = ownerDispatcher;
            InitVars();
        }

        #endregion

        #region Getting user login commandline
        public override void TryLogin(string UserId, System.Security.SecureString Password) {
            OnCompleted(LoginCode.SUCCESS, "true");
        }

        #endregion

        public override string GetGameStartArgs(string args) {
            return "true";
        }

        public override string GetLauncherStartArgs(string args) {
            return string.Empty;
        }
    }
}