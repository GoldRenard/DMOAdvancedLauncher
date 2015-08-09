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
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AdvancedLauncher.SDK.Model.Entity {

    /// <summary>
    /// Tamer entity
    /// </summary>
    public class Tamer : BaseEntity {

        /// <summary>
        /// Initializes a new <see cref="Tamer"/> instance
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tamer() {
            Digimons = new List<Digimon>();
        }

        /// <summary>
        /// Gets or sets remote account id
        /// </summary>
        public int AccountId {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets guild id
        /// </summary>
        [Required]
        [ForeignKey("Guild")]
        public long GuildId {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets guild entity
        /// </summary>
        [InverseProperty("Tamers")]
        public virtual Guild Guild {
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
        /// Gets or sets level
        /// </summary>
        public byte Level {
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
        /// Gets or sets type id
        /// </summary>
        public long? TypeId {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets type entity
        /// </summary>
        /// <seealso cref="TamerType"/>
        public virtual TamerType Type {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets value that determines is it master of related guild
        /// </summary>
        public bool IsMaster {
            get;
            set;
        }

        /// <summary>
        /// Gets partner digimon
        /// </summary>
        [NotMapped]
        public Digimon Partner {
            get {
                return Digimons.First(d => d.Type.IsStarter);
            }
        }

        /// <summary>
        /// Gets or sets digimon collection
        /// </summary>
        public virtual ICollection<Digimon> Digimons {
            get;
            set;
        }
    }
}