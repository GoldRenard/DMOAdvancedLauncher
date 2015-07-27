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

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using AdvancedLauncher.Tools;
using DMOLibrary;

namespace AdvancedLauncher.Model.Config {

    [XmlType(TypeName = "RemoteVersion")]
    public class RemoteVersion {

        [XmlElement("Version")]
        public string VersionString {
            set;
            get;
        }

        [XmlIgnore]
        public Version Version {
            set {
                VersionString = value.ToString();
            }
            get {
                if (string.IsNullOrEmpty(VersionString)) {
                    return null;
                }
                return new Version(VersionString);
            }
        }

        [XmlElement]
        public string DownloadUrl {
            set;
            get;
        }

        [XmlIgnore]
        public string ChangeLog {
            set;
            get;
        }

        [XmlText]
        public XmlNode[] CDataContent {
            get {
                var dummy = new XmlDocument();
                return new XmlNode[] { dummy.CreateCDataSection(System.Environment.NewLine + ChangeLog + System.Environment.NewLine) };
            }
            set {
                if (value == null) {
                    ChangeLog = null;
                    return;
                }

                if (value.Length != 1) {
                    throw new InvalidOperationException(
                        String.Format(
                            "Invalid array length {0}", value.Length));
                }

                ChangeLog = value[0].Value.Trim();
            }
        }

        public static RemoteVersion Instance {
            get {
                string xmlContent;
                try {
                    xmlContent = WebClientEx.DownloadContent(URLUtils.REMOTE_VERSION_FILE + "?" + Guid.NewGuid().ToString(), 5000);
                    XmlSerializer serializer = new XmlSerializer(typeof(RemoteVersion));
                    using (TextReader reader = new StringReader(xmlContent)) {
                        return serializer.Deserialize(reader) as RemoteVersion;
                    }
                } catch {
                    return null;
                }
            }
        }
    }
}