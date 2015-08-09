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

using System;

namespace AdvancedLauncher.SDK.Management.Plugins {

    /// <summary>
    /// Plugin information structure for internal use
    /// </summary>
    [Serializable]
    public struct PluginInfo {

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="AssemblyPath">Plugin assembly path</param>
        /// <param name="TypeName">Plugin type name</param>
        /// <param name="AssemblyToken">Plugin assembly PublicKey token</param>
        public PluginInfo(string AssemblyPath, string TypeName, byte[] AssemblyToken) {
            this.AssemblyPath = AssemblyPath;
            this.TypeName = TypeName;
            this.AssemblyToken = AssemblyToken;
        }

        /// <summary>
        /// Plugin assembly path
        /// </summary>
        public string AssemblyPath {
            get;
            private set;
        }

        /// <summary>
        /// Plugin assembly PublicKey token
        /// </summary>
        public byte[] AssemblyToken {
            get;
            private set;
        }

        /// <summary>
        /// Plugin type name
        /// </summary>
        public string TypeName {
            get;
            private set;
        }
    }
}