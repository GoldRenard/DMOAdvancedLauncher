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
using System.Xml.Serialization;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.Tools;

namespace AdvancedLauncher.Model.Protected {

    [XmlType(TypeName = "Profile")]
    public class ProtectedProfile {

        [XmlAttribute("Id")]
        public int Id {
            get;
            set;
        }

        [XmlAttribute("Name")]
        public string Name {
            get;
            set;
        } = "Default";

        [XmlAttribute]
        [Obsolete]
        public bool AppLocaleEnabled {
            get;
            set;
        }

        [XmlAttribute]
        public string LaunchMode {
            get;
            set;
        }

        [XmlAttribute]
        public bool UpdateEngineEnabled {
            get;
            set;
        }

        [XmlAttribute]
        public bool KBLCServiceEnabled {
            get;
            set;
        }

        public LoginData Login {
            get;
            set;
        } = new LoginData();

        public RotationData Rotation {
            get;
            set;
        } = new RotationData() {
            UpdateInterval = 2
        };

        public NewsData News {
            get;
            set;
        } = new NewsData() {
            FirstTab = 0,
            TwitterUrl = URLUtils.DEFAULT_TWITTER_SOURCE
        };

        public string ImagePath {
            get;
            set;
        }

        [XmlElement("GameEnv")]
        public GameModel GameModel {
            get;
            set;
        } = new GameModel();

        public ProtectedProfile() {
        }

        public ProtectedProfile(Profile profile) {
            this.Id = profile.Id;
            this.Name = profile.Name;
            this.ImagePath = profile.ImagePath;
            this.LaunchMode = profile.LaunchMode;
            this.UpdateEngineEnabled = profile.UpdateEngineEnabled;
            this.KBLCServiceEnabled = profile.KBLCServiceEnabled;
            this.Rotation = new RotationData(profile.Rotation);
            this.News = new NewsData(profile.News);
            this.GameModel = new GameModel(profile.GameModel);
        }
    }
}