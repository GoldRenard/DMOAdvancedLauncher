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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AdvancedLauncher.SDK.Model.Entity {

    /// <summary>
    /// Guild entity
    /// </summary>
    public class Guild : BaseEntity {

        /// <summary>
        /// Initializes a new <see cref="Guild"/> instance
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Guild() {
            Tamers = new List<Tamer>();
        }

        /// <summary>
        /// Gets or sets remote identifier
        /// </summary>
        public int Identifier {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets server id
        /// </summary>
        [Required]
        [ForeignKey("Server")]
        public long ServerId {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets server entity
        /// </summary>
        [InverseProperty("Guilds")]
        public virtual Server Server {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets name
        /// </summary>
        [Required]
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets reputation score
        /// </summary>
        public long Rep {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets rank score
        /// </summary>
        public long Rank {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets last update time
        /// </summary>
        public DateTime? UpdateTime {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets value that determines is detailed data was merged last time
        /// </summary>
        public bool IsDetailed {
            get;
            set;
        }

        /// <summary>
        /// Gets master Tamer instance
        /// </summary>
        [NotMapped]
        public Tamer Master {
            get {
                return Tamers.First(t => t.IsMaster);
            }
        }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public virtual ICollection<Tamer> Tamers {
            get;
            set;
        }

        /// <summary>
        /// Returns string representation of this object
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString() {
            return string.Format("Guild [Identifier={0}, Name={1}]", Identifier, Name);
        }
    }
}