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

    public class Tamer : BaseEntity {

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tamer() {
            Digimons = new List<Digimon>();
        }

        /// <summary>
        /// Legacy: Id
        /// </summary>
        public int AccountId {
            get;
            set;
        }

        [Required]
        [ForeignKey("Guild")]
        public long GuildId {
            get;
            set;
        }

        [InverseProperty("Tamers")]
        public virtual Guild Guild {
            get;
            set;
        }

        [Required]
        public string Name {
            get;
            set;
        }

        public byte Level {
            get;
            set;
        }

        public long Rank {
            get;
            set;
        }

        public long? TypeId {
            get;
            set;
        }

        public virtual TamerType Type {
            get;
            set;
        }

        public bool IsMaster {
            get;
            set;
        }

        [NotMapped]
        public Digimon Partner {
            get {
                return Digimons.First(d => d.Type.IsStarter);
            }
        }

        public virtual ICollection<Digimon> Digimons {
            get;
            set;
        }
    }
}