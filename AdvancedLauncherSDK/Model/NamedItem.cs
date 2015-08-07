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

using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.Model {

    public abstract class NamedItem : CrossDomainObject, IRemotePropertyChanged {

        public event RemotePropertyChangedEventHandler PropertyChanged;

        public NamedItem(string Name, bool IsBinding = false) {
            this.Name = Name;
            this.IsBinding = IsBinding;
        }

        private string _Name;

        public string Name {
            get {
                return _Name;
            }
            set {
                if (_Name != value) {
                    _Name = value;
                }
                NotifyPropertyChanged("Name");
            }
        }

        private bool _IsBinding;

        public bool IsBinding {
            get {
                return _IsBinding;
            }
            set {
                if (_IsBinding != value) {
                    _IsBinding = value;
                }
                NotifyPropertyChanged("IsBinding");
                NotifyPropertyChanged("Name");
            }
        }

        private bool _IsEnabled = true;

        public bool IsEnabled {
            get {
                return _IsEnabled;
            }
            set {
                if (_IsEnabled != value) {
                    _IsEnabled = value;
                }
                NotifyPropertyChanged("IsEnabled");
            }
        }

        protected void NotifyPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new RemotePropertyChangedEventArgs(propertyName));
            }
        }
    }
}