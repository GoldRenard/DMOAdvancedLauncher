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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Entity;

namespace AdvancedLauncher.SDK.Model.Web {

    public abstract class AbstractServersProvider : CrossDomainObject, IServersProvider {
        protected ICollection<Server> _ServerList;

        public ICollection<Server> ServerList {
            get {
                if (_ServerList == null) {
                    _ServerList = CreateServerList();
                }
                return _ServerList;
            }
        }

        protected readonly Server.ServerType ServerType;

        public AbstractServersProvider() {
        }

        public AbstractServersProvider(Server.ServerType serverType) {
            this.ServerType = serverType;
        }

        public Server GetServerById(long serverId) {
            return ServerList.Where(i => i.Identifier == serverId).FirstOrDefault();
        }

        protected abstract ReadOnlyCollection<Server> CreateServerList();
    }
}