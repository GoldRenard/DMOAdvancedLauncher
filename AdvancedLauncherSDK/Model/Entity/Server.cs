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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AdvancedLauncher.SDK.Model.Entity {

    /// <summary>
    /// Server entity
    /// </summary>
    public class Server : BaseEntity {

        /// <summary>
        /// Server type enumeration
        /// </summary>
        public enum ServerType {

            /// <summary>
            /// KDMO Server type
            /// </summary>
            KDMO = 1,

            /// <summary>
            /// KDMO IMBC Server type
            /// </summary>
            KDMO_IMBC = 2,

            /// <summary>
            /// Joymax Server type
            /// </summary>
            GDMO = 3,

            /// <summary>
            /// Aeria Games Server type
            /// </summary>
            ADMO = 4
        }

        /// <summary>
        /// Initializes a new <see cref="Server"/> instance
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Server() {
            Guilds = new List<Guild>();
        }

        /// <summary>
        /// Initializes a new <see cref="Server"/> based on another
        /// </summary>
        /// <param name="server">Source <see cref="Server"/></param>
        public Server(Server server) {
            this.Identifier = server.Identifier;
            this.Name = server.Name;
            this.Type = server.Type;
        }

        /// <summary>
        /// Gets or sets remote identifier
        /// </summary>
        [Required]
        public byte Identifier {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets name
        /// </summary>
        [Required]
        [StringLength(25)]
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets server type
        /// </summary>
        /// <seealso cref="ServerType"/>
        [Required]
        public ServerType Type {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets guilds collection
        /// </summary>
        public virtual ICollection<Guild> Guilds {
            get;
            set;
        }

        /// <summary>
        /// Returns string representation of this object
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString() {
            return Name;
        }
    }
}