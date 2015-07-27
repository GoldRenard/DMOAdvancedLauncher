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
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using AdvancedLauncher.Management;
using AdvancedLauncher.Management.Execution;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Tools;
using Ninject;

namespace AdvancedLauncher.Model.Config {

    [XmlType(TypeName = "Profile")]
    public class Profile : INotifyPropertyChanged {
        private int _Id = 0;

        [XmlAttribute("Id")]
        public int Id {
            set {
                _Id = value; NotifyPropertyChanged("Id");
            }
            get {
                return _Id;
            }
        }

        private string _Name = "Default";

        [XmlAttribute("Name")]
        public string Name {
            set {
                _Name = value;
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("FullName");
            }
            get {
                return _Name;
            }
        }

        [XmlIgnore]
        public string FullName {
            get {
                return string.Format("{0} ({1})", Name, GameType);
            }
        }

        [Obsolete]
        private bool _AppLocaleEnabled = true;

        [XmlAttribute]
        [Obsolete]
        public bool AppLocaleEnabled {
            set {
                _AppLocaleEnabled = value; NotifyPropertyChanged("AppLocaleEnabled");
            }
            get {
                return _AppLocaleEnabled;
            }
        }

        private string _LaunchMode;

        [XmlAttribute]
        public string LaunchMode {
            set {
                _LaunchMode = value;
                NotifyPropertyChanged("LaunchMode");
            }
            get {
                return _LaunchMode;
            }
        }

        [XmlIgnore]
        public ILauncher Launcher {
            set {
                if (value != null) {
                    LaunchMode = value.Mnemonic;
                }
            }
            get {
                return App.Kernel.Get<ILauncherManager>().GetProfileLauncher(this);
            }
        }

        private bool _UpdateEngineEnabled = false;

        [XmlAttribute]
        public bool UpdateEngineEnabled {
            set {
                _UpdateEngineEnabled = value; NotifyPropertyChanged("UpdateEngineEnabled");
            }
            get {
                return _UpdateEngineEnabled;
            }
        }

        private bool _KBLCServiceEnabled = false;

        [XmlAttribute]
        public bool KBLCServiceEnabled {
            set {
                _KBLCServiceEnabled = value; NotifyPropertyChanged("KBLCServiceEnabled");
            }
            get {
                return _KBLCServiceEnabled;
            }
        }

        #region Subcontainers

        private LoginData _Login = new LoginData();

        public LoginData Login {
            set {
                _Login = value; NotifyPropertyChanged("Login");
            }
            get {
                return _Login;
            }
        }

        private RotationData _Rotation = new RotationData() {
            UpdateInterval = 2
        };

        public RotationData Rotation {
            set {
                _Rotation = value; NotifyPropertyChanged("Rotation");
            }
            get {
                return _Rotation;
            }
        }

        private NewsData _News = new NewsData() {
            FirstTab = 0,
            TwitterUrl = URLUtils.DEFAULT_TWITTER_SOURCE
        };

        public NewsData News {
            set {
                _News = value; NotifyPropertyChanged("News");
            }
            get {
                return _News;
            }
        }

        #endregion Subcontainers

        #region Image

        private string _ImagePath;

        public string ImagePath {
            set {
                _ImagePath = value;
                NotifyPropertyChanged("ImagePath");
                NotifyPropertyChanged("Image");
                NotifyPropertyChanged("HasImage");
                NotifyPropertyChanged("NoImage");
            }
            get {
                return _ImagePath;
            }
        }

        private string _ImagePathLoaded;
        private ImageSource _Image;

        [XmlIgnore]
        public ImageSource Image {
            set {
                if (_Image != value) {
                    _Image = value;
                    NotifyPropertyChanged("Image");
                    NotifyPropertyChanged("HasImage");
                    NotifyPropertyChanged("NoImage");
                }
            }
            get {
                if (_ImagePath != null) {
                    if (_ImagePath != _ImagePathLoaded) {
                        if (File.Exists(_ImagePath)) {
                            _Image = new BitmapImage(new Uri(_ImagePath));
                            _ImagePathLoaded = _ImagePath;
                        }
                    }
                }
                return _Image;
            }
        }

        [XmlIgnore]
        public bool HasImage {
            get {
                return Image != null;
            }
        }

        [XmlIgnore]
        public bool NoImage {
            get {
                return Image == null;
            }
        }

        #endregion Image

        #region Game Environment

        private GameModel _GameModel = new GameModel();

        [XmlElement("GameEnv")]
        public GameModel GameModel {
            set {
                _GameModel = value;
                NotifyPropertyChanged("GameModel");
                NotifyPropertyChanged("FullName");
            }
            get {
                return _GameModel;
            }
        }

        [XmlIgnore]
        public string GameType {
            set {
            }
            get {
                switch (GameModel.Type) {
                    case GameManager.GameType.ADMO:
                        {
                            return "Aeria Games";
                        }
                    case GameManager.GameType.GDMO:
                        {
                            return "Joymax";
                        }
                    case GameManager.GameType.KDMO_DM:
                        {
                            return "Korea DM";
                        }
                    case GameManager.GameType.KDMO_IMBC:
                        {
                            return "Korea IMBC";
                        }
                    default:
                        return "Unknown";
                }
            }
        }

        [XmlIgnore]
        public byte GameTypeNum {
            set {
                GameModel.Type = (GameManager.GameType)value;
                NotifyPropertyChanged("GameModel");       //We've changed env, so we must update all bindings
                NotifyPropertyChanged("FullName");
                NotifyPropertyChanged("GameType");
                NotifyPropertyChanged("DMOProfile");
                NotifyPropertyChanged("Rotation");      //cuz dmoprofile changed, we must update rotation and news support
                NotifyPropertyChanged("News");
            }
            get {
                return (byte)GameModel.Type;
            }
        }

        #endregion Game Environment

        #region Constructors

        public Profile() {
        }

        public Profile(Profile p) {
            this.Id = p.Id;
            this.Name = p.Name;
            this.ImagePath = p.ImagePath;
            this.LaunchMode = p.LaunchMode;
            this.UpdateEngineEnabled = p.UpdateEngineEnabled;
            this.KBLCServiceEnabled = p.KBLCServiceEnabled;
            this.Login = new LoginData(p.Login);
            this.Rotation = new RotationData(p.Rotation);
            this.News = new NewsData(p.News);
            this.GameModel = new GameModel(p.GameModel);
        }

        #endregion Constructors

        #region Property Change Handler

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Property Change Handler
    }
}