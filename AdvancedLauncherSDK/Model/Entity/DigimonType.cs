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

namespace AdvancedLauncher.SDK.Model.Entity {

    /// <summary>
    /// Digimon type entity
    /// </summary>
    public class DigimonType : BaseEntity {

        /// <summary>
        /// Gets or sets type code
        /// </summary>
        [Required]
        public int Code {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets value that determine is it starter Digimon
        /// </summary>
        public bool IsStarter {
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
        /// Gets or sets name
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets alternative name
        /// </summary>
        [StringLength(50)]
        public string NameAlt {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Korean name
        /// </summary>
        [StringLength(50)]
        public string NameKorean {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets search word in GDMO representation
        /// </summary>
        [StringLength(50)]
        public string SearchGDMO {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets search word in KDMO representation
        /// </summary>
        [StringLength(50)]
        public string SearchKDMO {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets digimon collection
        /// </summary>
        public virtual ICollection<Digimon> Digimons {
            get;
            set;
        }

        /// <summary>
        /// Returns string representation of this object
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString() {
            return string.Format("DigimonType [Id={0}, Code={1}, Name={2}, NameAlt={3}, SearchGDMO={4}, SearchKDMO={5}",
                Id, Code, Name, NameAlt, SearchGDMO, SearchKDMO);
        }
    }
}