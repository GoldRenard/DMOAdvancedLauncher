// ======================================================================
// DMOLibrary
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Linq;
using System.Text;

namespace DMOLibrary.Database.Entity {
    public class Tamer : BaseEntity {

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

        public long GuildId {
            get;
            set;
        }

        public virtual Guild Guild {
            get;
            set;
        }

        [Required]
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Do not forget to rename from Lvl in viewModels
        /// </summary>
        public byte Level {
            get;
            set;
        }

        public long Rank {
            get;
            set;
        }

        public TamerType Type {
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
