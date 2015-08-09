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

    /// <summary>
    /// Base <see cref="IServersProvider"/> implementation
    /// </summary>
    /// <seealso cref="IServersProvider"/>
    /// <seealso cref="Server"/>
    public abstract class AbstractServersProvider : CrossDomainObject, IServersProvider {
        private ICollection<Server> _ServerList;

        /// <summary>
        /// Gets server list for this provider
        /// </summary>
        public ICollection<Server> ServerList {
            get {
                if (_ServerList == null) {
                    _ServerList = CreateServerList();
                }
                return _ServerList;
            }
        }

        /// <summary>
        /// ServerType related with this instance
        /// </summary>
        protected readonly Server.ServerType ServerType;

        /// <summary>
        /// Initializes a new <see cref="AbstractServersProvider"/> instance
        /// </summary>
        public AbstractServersProvider() {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AbstractServersProvider"/> for specified <see cref="Server.ServerType"/>.
        /// </summary>
        /// <param name="serverType">Server type</param>
        public AbstractServersProvider(Server.ServerType serverType) {
            this.ServerType = serverType;
        }

        /// <summary>
        /// Returns server by its id
        /// </summary>
        /// <param name="serverId">Server id</param>
        /// <returns>Server entity</returns>
        public Server GetServerById(long serverId) {
            return ServerList.Where(i => i.Identifier == serverId).FirstOrDefault();
        }

        /// <summary>
        /// Creates new read-only server collection for storing in <see cref="ServerList"/>.
        /// </summary>
        /// <returns>Server list collection</returns>
        protected abstract ReadOnlyCollection<Server> CreateServerList();
    }
}