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

using System.Xml.Serialization;

namespace AdvancedLauncher.Model.Protected {

    public enum ProxyMode {
        Default = 0,
        HTTP = 1,
        HTTPS = 2
    }

    public class ProxySetting {

        public ProxySetting() {
            Credentials = new ProxyCredentials();
            Port = 8080;
        }

        public ProxySetting(ProxySetting source) {
            IsEnabled = source.IsEnabled;
            Mode = source.Mode;
            Host = source.Host;
            Port = source.Port;
            Authentication = source.Authentication;
            Credentials = new ProxyCredentials(source.Credentials);
        }

        [XmlAttribute("Enabled")]
        public bool IsEnabled {
            get;
            set;
        }

        [XmlAttribute("Mode")]
        public ProxyMode Mode {
            get;
            set;
        }

        [XmlAttribute("Host")]
        public string Host {
            get;
            set;
        }

        [XmlAttribute("Port")]
        public ushort Port {
            get;
            set;
        }

        [XmlAttribute("Authentication")]
        public bool Authentication {
            get;
            set;
        }

        [XmlElement("Credentials")]
        public ProxyCredentials Credentials {
            get;
            set;
        }
    }
}