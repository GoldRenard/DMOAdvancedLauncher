// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
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

using System.Collections.ObjectModel;
using System.Security;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.Model.Web {

    public interface IGameProfile {

        event LoginCompleteEventHandler LoginCompleted;

        event LoginStateEventHandler LoginStateChanged;

        INewsProfile NewsProfile {
            get;
        }

        IWebProfile GetWebProfile();

        Server GetServerById(int serverId);

        void TryLogin(string UserId, SecureString Password);

        string GetGameStartArgs(string args);

        string GetLauncherStartArgs(string args);

        string GetTypeName();

        bool IsWebAvailable {
            get;
        }

        bool IsNewsAvailable {
            get;
        }

        bool IsLoginRequired {
            get;
        }

        ObservableCollection<Server> ServerList {
            get;
        }
    }
}