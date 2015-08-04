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
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Model.Entity {

    public abstract class BaseEntity : CrossDomainObject {

        [Key]
        public long Id {
            get;
            set;
        }

        public override int GetHashCode() {
            int prime = 31;
            int result = 1;
            result = prime * result + Id.GetHashCode();
            return result;
        }

        public override bool Equals(object obj) {
            if (this == obj) {
                return true;
            }
            if (obj == null) {
                return false;
            }
            if (!this.GetType().IsAssignableFrom(obj.GetType())) {
                return false;
            }
            BaseEntity other = (BaseEntity)obj;
            if (!Id.Equals(other.Id)) {
                return false;
            }
            return this.GetHashCode() == obj.GetHashCode();
        }
    }
}