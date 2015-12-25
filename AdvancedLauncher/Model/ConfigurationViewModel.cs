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

using AdvancedLauncher.SDK.Management.Configuration;

namespace AdvancedLauncher.Model {

    public class ConfigurationViewModel : AbstractItemViewModel<IConfiguration> {

        public ConfigurationViewModel(IConfiguration Configuration) : base(null) {
            this.Configuration = Configuration;
        }

        public IConfiguration Configuration {
            get;
            private set;
        }

        public string Name {
            get {
                return Configuration.Name;
            }
        }

        public string ServerName {
            get {
                return Configuration.ServerName;
            }
        }

        public string GameType {
            get {
                return Configuration.GameType;
            }
        }

        public bool IsWebAvailable {
            get {
                return Configuration.IsWebAvailable;
            }
        }

        public bool IsNewsAvailable {
            get {
                return Configuration.IsNewsAvailable;
            }
        }

        public bool IsLoginRequired {
            get {
                return Configuration.IsLoginRequired;
            }
        }

        public bool IsManualLoginSupported {
            get {
                return Configuration.IsManualLoginSupported;
            }
        }
    }
}