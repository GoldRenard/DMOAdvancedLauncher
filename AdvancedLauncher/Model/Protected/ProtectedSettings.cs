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

using System.Collections.Generic;
using System.Xml.Serialization;
using AdvancedLauncher.SDK.Model.Config;

namespace AdvancedLauncher.Model.Protected {

    [XmlType(TypeName = "Settings")]
    public class ProtectedSettings {

        [XmlElement("Language")]
        public string Language {
            get; set;
        }

        [XmlElement("AppTheme")]
        public string AppTheme {
            get; set;
        }

        [XmlElement("ThemeAccent")]
        public string ThemeAccent {
            get; set;
        }

        [XmlElement("CheckForUpdates")]
        public bool CheckForUpdates {
            get; set;
        }

        [XmlElement("DefaultProfile")]
        public int DefaultProfile {
            get; set;
        }

        private ProxySetting _Proxy = new ProxySetting();

        [XmlElement("Proxy")]
        public ProxySetting Proxy {
            get;
            set;
        }

        [XmlArray("Profiles"), XmlArrayItem(typeof(ProtectedProfile), ElementName = "Profile")]
        public List<ProtectedProfile> Profiles {
            get;
            set;
        }

        public ProtectedSettings() {
            this.CheckForUpdates = true;
        }

        public ProtectedSettings(Settings settings) {
            this.Language = settings.LanguageFile;
            this.AppTheme = settings.AppTheme;
            this.ThemeAccent = settings.ThemeAccent;
            this.CheckForUpdates = settings.CheckForUpdates;
            this.Profiles = new List<ProtectedProfile>();
        }
    }
}