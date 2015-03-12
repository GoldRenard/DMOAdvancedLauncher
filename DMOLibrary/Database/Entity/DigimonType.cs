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

using System.ComponentModel.DataAnnotations;

namespace DMOLibrary.Database.Entity {

    public class DigimonType : BaseEntity {

        [Required]
        public int Code {
            get;
            set;
        }

        public bool IsStarter {
            get;
            set;
        }

        public double SizeCm {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string Name {
            get;
            set;
        }

        [StringLength(50)]
        public string NameAlt {
            get;
            set;
        }

        [StringLength(50)]
        public string NameKorean {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string SearchGDMO {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string SearchKDMO {
            get;
            set;
        }

        public override string ToString() {
            return string.Format("DigimonType [Id={0}, Code={1}, Name={2}, NameAlt={3}, SearchGDMO={4}, SearchKDMO={5}",
                Id, Code, Name, NameAlt, SearchGDMO, SearchKDMO);
        }
    }
}