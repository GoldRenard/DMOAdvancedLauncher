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

    /// <summary>
    /// Digimon entity
    /// </summary>
    public class Digimon : BaseEntity {

        /// <summary>
        /// Gets or sets master Tamer id
        /// </summary>
        [Required]
        [ForeignKey("Tamer")]
        public long TamerId {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets master Tamer entity
        /// </summary>
        [InverseProperty("Digimons")]
        public virtual Tamer Tamer {
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
        /// Gets or sets type id
        /// </summary>
        public long TypeId {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets type entity
        /// </summary>

        public virtual DigimonType Type {
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
        /// Gets or sets level
        /// </summary>
        public byte Level {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets size in centimeters
        /// </summary>
        public double SizeCm {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets size persentage
        /// </summary>
        public int SizePc {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets size rank
        /// </summary>
        public int SizeRank {
            get;
            set;
        }
    }
}