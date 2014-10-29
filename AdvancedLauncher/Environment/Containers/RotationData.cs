// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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

namespace AdvancedLauncher.Environment.Containers {

    public class RotationData {

        [XmlAttribute("Guild")]
        public string Guild {
            set;
            get;
        }

        [XmlAttribute("Tamer")]
        public string Tamer {
            set;
            get;
        }

        [XmlAttribute("ServerId")]
        public byte ServerId {
            set;
            get;
        }

        [XmlAttribute("UpdateInterval")]
        public int UpdateInterval {
            set;
            get;
        }

        public RotationData(RotationData rd) {
            Guild = rd.Guild;
            Tamer = rd.Tamer;
            ServerId = rd.ServerId;
            UpdateInterval = rd.UpdateInterval;
        }

        public RotationData() {
        }
    }
}