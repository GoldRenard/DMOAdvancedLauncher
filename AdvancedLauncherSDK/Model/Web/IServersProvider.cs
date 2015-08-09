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
using AdvancedLauncher.SDK.Model.Entity;

namespace AdvancedLauncher.SDK.Model.Web {

    /// <summary>
    /// Server provider interface
    /// </summary>
    /// <seealso cref="AbstractServersProvider"/>
    /// <seealso cref="Server"/>
    public interface IServersProvider {

        /// <summary>
        /// Returns server by its id
        /// </summary>
        /// <param name="serverId">Server id</param>
        /// <returns>Server entity</returns>
        Server GetServerById(long serverId);

        /// <summary>
        /// Gets server list for this provider
        /// </summary>
        ICollection<Server> ServerList {
            get;
        }
    }
}