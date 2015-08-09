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
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Entity;

namespace AdvancedLauncher.SDK.Model.Web {

    /// <summary>
    /// Servers provider that allows to read server list from database
    /// </summary>
    public abstract class DatabaseServersProvider : AbstractServersProvider {

        /// <summary>
        /// DatabaseManager API
        /// </summary>
        protected readonly IDatabaseManager DatabaseManager;

        /// <summary>
        /// Initializes a new instance of <see cref="DatabaseServersProvider"/> for specified <see cref="IDatabaseManager"/> and <see cref="Server.ServerType"/>.
        /// </summary>
        /// <param name="DatabaseManager">DatabaseManager API</param>
        /// <param name="serverType">Server type</param>
        public DatabaseServersProvider(IDatabaseManager DatabaseManager, Server.ServerType serverType) : base(serverType) {
            this.DatabaseManager = DatabaseManager;
        }

        /// <summary>
        /// Creates new read-only server collection for storing in <see cref="AbstractServersProvider.ServerList"/>.
        /// </summary>
        /// <returns></returns>
        protected override ReadOnlyCollection<Server> CreateServerList() {
            using (IDatabaseContext context = DatabaseManager.CreateContext()) {
                return new ReadOnlyCollection<Server>(context.FindServerByServerType(ServerType));
            }
        }
    }
}