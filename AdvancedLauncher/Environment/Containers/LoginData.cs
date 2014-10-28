// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

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
using AdvancedLauncher.Service;

namespace AdvancedLauncher.Environment.Containers {
    public class LoginData {
        [XmlIgnore]
        public SecureString SecurePassword {
            set;
            get;
        }

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
        [XmlAttribute("LastSessionArgs")]
        public string LastSessionArgs {
            set;
            get;
        }

        public LoginData(LoginData source) {
            User = source.User;
            Password = source.Password;
            LastSessionArgs = source.LastSessionArgs;
        }
        public LoginData() {
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
}
