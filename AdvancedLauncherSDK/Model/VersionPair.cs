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

using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Model {

    /// <summary>
    /// Version pair for game update logic
    /// </summary>
    public class VersionPair : CrossDomainObject {

        /// <summary>
        /// Gets local game version
        /// </summary>
        public int Local {
            get;
            private set;
        }

        /// <summary>
        /// Gets remote game version
        /// </summary>
        public int Remote {
            get;
            private set;
        }

        /// <summary>
        /// Gets <b>True</b> if game update required (<see cref="Remote"/> greater than <see cref="Local"/>)
        /// </summary>
        public bool IsUpdateRequired {
            get {
                return Remote > Local;
            }
        }

        public VersionPair(int Local, int Remote) {
            this.Local = Local;
            this.Remote = Remote;
        }
    }
}