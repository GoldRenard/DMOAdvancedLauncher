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

using DMOLibrary.Database.Entity;

namespace DMOLibrary.Profiles.Joymax {

    public class DMOJoymax : DMOProfile {

        public DMOJoymax()
            : base(Server.ServerType.GDMO, "Joymax") {
        }

        #region Getting user login commandline

        public override void TryLogin(string UserId, System.Security.SecureString Password) {
            OnCompleted(LoginCode.SUCCESS, "true");
        }

        #endregion Getting user login commandline

        protected override DMONewsProfile GetNewsProfile() {
            return new JMNews();
        }

        public override AbstractWebProfile GetWebProfile() {
            return new JoymaxWebProfile();
        }

        public override string GetGameStartArgs(string args) {
            return "true";
        }

        public override string GetLauncherStartArgs(string args) {
            return string.Empty;
        }
    }
}