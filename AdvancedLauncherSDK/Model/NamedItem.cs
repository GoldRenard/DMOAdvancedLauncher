﻿// ======================================================================
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

    /// <summary>
    /// Base item with name and IsEnabled properties, used for <see cref="MenuItem"/> or <see cref="PageItem"/>.
    /// </summary>
    public abstract class NamedItem : CrossDomainObject, IRemotePropertyChanged {

        /// <summary>
        /// Property changed event handler
        /// </summary>
        public event RemotePropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="NamedItem"/> for specified name and binding flag (false by default).
        /// </summary>
        /// <param name="Name">Item name</param>
        /// <param name="IsBinding">Is it binding name</param>
        public NamedItem(string Name, bool IsBinding = false) {
            this.Name = Name;
            this.IsBinding = IsBinding;
        }

        private string _Name;

        /// <summary>
        /// Gets or sets name of item
        /// </summary>
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

        /// <summary>
        /// Gets or sets value that determines is item name is binging name
        /// </summary>
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

        /// <summary>
        /// Gets or sets value that determines is this item enabled
        /// </summary>
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

        /// <summary>
        /// Notifies property changing
        /// </summary>
        /// <param name="propertyName">Changed property name</param>
        protected void NotifyPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new RemotePropertyChangedEventArgs(propertyName));
            }
        }
    }
}