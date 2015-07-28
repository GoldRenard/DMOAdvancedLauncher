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

using System.ComponentModel;
using System.Xml.Serialization;
using AdvancedLauncher.SDK.Model.Config;

namespace AdvancedLauncher.Model.Config {

    [XmlType(TypeName = "GameEnv")]
    public class GameModel : IGameModel, INotifyPropertyChanged {
        private string _Type;

        [XmlAttribute("Type")]
        public string Type {
            set {
                _Type = value;
                NotifyPropertyChanged("Type");
            }
            get {
                return _Type;
            }
        }

        private string _GamePath;

        [XmlElement("GamePath")]
        public string GamePath {
            set {
                _GamePath = value;
                NotifyPropertyChanged("GamePath");
            }
            get {
                return _GamePath;
            }
        }

        private string _DefLauncherPath;

        [XmlElement("DefLauncherPath")]
        public string DefLauncherPath {
            set {
                _DefLauncherPath = value;
                NotifyPropertyChanged("DefLauncherPath");
            }
            get {
                return _DefLauncherPath;
            }
        }

        public GameModel() {
        }

        public GameModel(IGameModel another) {
            this.Type = another.Type;
            this.GamePath = another.GamePath;
            this.DefLauncherPath = another.DefLauncherPath;
        }

        public override int GetHashCode() {
            int prime = 31;
            int result = 1;
            result = prime * result + Type.GetHashCode();
            result = prime * result + (GamePath == null ? 0 : GamePath.GetHashCode());
            result = prime * result + (DefLauncherPath == null ? 0 : DefLauncherPath.GetHashCode());
            return result;
        }

        public override bool Equals(object obj) {
            if (this == obj) {
                return true;
            }
            if (obj == null) {
                return false;
            }
            if (!this.GetType().IsAssignableFrom(obj.GetType())) {
                return false;
            }
            GameModel other = (GameModel)obj;
            if (!Type.Equals(other.Type)) {
                return false;
            }

            if (GamePath == null) {
                if (other.GamePath != null) {
                    return false;
                }
            } else if (!GamePath.Equals(other.GamePath)) {
                return false;
            }

            if (DefLauncherPath == null) {
                if (other.DefLauncherPath != null) {
                    return false;
                }
            } else if (!DefLauncherPath.Equals(other.DefLauncherPath)) {
                return false;
            }

            return this.GetHashCode() == obj.GetHashCode();
        }

        #region Property Change Handler

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Property Change Handler
    }
}