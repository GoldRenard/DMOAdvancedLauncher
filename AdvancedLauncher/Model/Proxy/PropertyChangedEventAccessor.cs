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

using System.Security.Permissions;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.Model.Proxy {

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    public class PropertyChangedEventAccessor : CrossDomainObject {
        private readonly IPropertyChangedEventAccessor Object;

        public PropertyChangedEventAccessor(IPropertyChangedEventAccessor Object) {
            this.Object = Object;
        }

        public void OnPropertyChanged(object sender, RemotePropertyChangedEventArgs e) {
            Object.OnPropertyChanged(sender, e);
        }
    }
}