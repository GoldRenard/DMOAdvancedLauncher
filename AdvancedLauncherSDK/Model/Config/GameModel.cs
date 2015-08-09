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
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Model.Config {

    /// <summary>
    /// Game model configuration
    /// </summary>
    /// <seealso cref="Profile"/>
    /// <seealso cref="IConfigurationManager"/>
    [XmlType(TypeName = "GameEnv")]
    public class GameModel : CrossDomainObject, INotifyPropertyChanged {
        private string _Type;

        /// <summary>
        /// Gets or sets game type
        /// </summary>
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

        /// <summary>
        /// Gets or sets path to game
        /// </summary>
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

        private string _LauncherPath;

        /// <summary>
        /// Gets or sets path to stock launcher
        /// </summary>
        [XmlElement("DefLauncherPath")]
        public string LauncherPath {
            set {
                _LauncherPath = value;
                NotifyPropertyChanged("LauncherPath");
            }
            get {
                return _LauncherPath;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GameModel"/> instance
        /// </summary>
        public GameModel() {
        }

        /// <summary>
        /// Initializes a new <see cref="GameModel"/> based on another
        /// </summary>
        /// <param name="another">Source <see cref="GameModel"/></param>
        public GameModel(GameModel another) {
            this.Type = another.Type;
            this.GamePath = another.GamePath;
            this.LauncherPath = another.LauncherPath;
        }

        /// <summary>
        /// Returns the hash code for this <see cref="GameModel"/>
        /// </summary>
        /// <returns>Hash code for this <see cref="GameModel"/></returns>
        public override int GetHashCode() {
            int prime = 31;
            int result = 1;
            result = prime * result + Type.GetHashCode();
            result = prime * result + (GamePath == null ? 0 : GamePath.GetHashCode());
            result = prime * result + (LauncherPath == null ? 0 : LauncherPath.GetHashCode());
            return result;
        }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="GameModel"/> are the same
        /// </summary>
        /// <param name="obj">The object to compare to this instance</param>
        /// <returns><b>True</b> of the object of the obj parameter is the same as the current instance</returns>
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

            if (LauncherPath == null) {
                if (other.LauncherPath != null) {
                    return false;
                }
            } else if (!LauncherPath.Equals(other.LauncherPath)) {
                return false;
            }

            return this.GetHashCode() == obj.GetHashCode();
        }

        #region Property Change Handler

        /// <summary>
        /// Property changed event handler
        /// </summary>
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