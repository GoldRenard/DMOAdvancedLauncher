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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedLauncher.SDK.Model.Entity {

    public class Digimon : BaseEntity {

        [Required]
        [ForeignKey("Tamer")]
        public long TamerId {
            get;
            set;
        }

        [InverseProperty("Digimons")]
        public virtual Tamer Tamer {
            get;
            set;
        }

        [Required]
        public string Name {
            get;
            set;
        }

        public long TypeId {
            get;
            set;
        }

        public virtual DigimonType Type {
            get;
            set;
        }

        public long Rank {
            get;
            set;
        }

        public byte Level {
            get;
            set;
        }

        public double SizeCm {
            get;
            set;
        }

        public int SizePc {
            get;
            set;
        }

        public int SizeRank {
            get;
            set;
        }
    }
}