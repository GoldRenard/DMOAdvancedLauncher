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

using System.Security;
using System.Xml.Serialization;
using AdvancedLauncher.Tools;

namespace AdvancedLauncher.Model.Config {

    public class ProxySetting {

        public class ProxyCredentials {

            [XmlAttribute("Password")]
            public string Password {
                set {
                    SecurePassword = PassEncrypt.ConvertToSecureString(
                        PassEncrypt.Decrypt(
                            value,
                            FingerPrint.Value(FingerPrint.FingerPart.UUID, false)
                        )
                    );
                    return;
                }
                get {
                    return PassEncrypt.Encrypt(
                        PassEncrypt.ConvertToUnsecureString(SecurePassword),
                        FingerPrint.Value(FingerPrint.FingerPart.UUID, false)
                    );
                }
            }

            [XmlAttribute("User")]
            public string User {
                set;
                get;
            }

            public ProxyCredentials(ProxyCredentials source) {
                User = source.User;
                Password = source.Password;
            }

            public ProxyCredentials() {
            }

            [XmlIgnore]
            public SecureString SecurePassword {
                set;
                get;
            }

            [XmlIgnore]
            public bool IsCorrect {
                get {
                    if (SecurePassword == null) {
                        return false;
                    }
                    return SecurePassword.Length > 0 && !string.IsNullOrEmpty(User);
                }
            }
        }

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
        public int Mode {
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