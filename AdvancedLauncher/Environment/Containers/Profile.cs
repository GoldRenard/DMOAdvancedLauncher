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

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml.Serialization;
using DMOLibrary.Profiles;


namespace AdvancedLauncher.Environment.Containers {
    [XmlType(TypeName = "Profile")]
    public class Profile : INotifyPropertyChanged {
        private static string DEFAULT_TWITTER_SOURCE = "http://renamon.ru/launcher/dmor_timeline.php";

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
                _Name = value; NotifyPropertyChanged("Name");
            }
            get {
                return _Name;
            }
        }

        private bool _AppLocaleEnabled = true;
        [XmlAttribute]
        public bool AppLocaleEnabled {
            set {
                _AppLocaleEnabled = value; NotifyPropertyChanged("AppLocaleEnabled");
            }
            get {
                if (!Service.ApplicationLauncher.IsALSupported) {
                    return false;
                }
                return _AppLocaleEnabled;
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

        #region Subcontainers

        private LoginData _Login = new LoginData();
        public LoginData Login {
            set {
                _Login = value; NotifyPropertyChanged("Login");
            }
            get {
                if (!DMOProfile.IsLoginRequired) {
                    _Login.Password = string.Empty;
                    _Login.LastSessionArgs = string.Empty;
                    _Login.User = string.Empty;
                }
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
                if (!DMOProfile.IsWebAvailable) {
                    _Rotation.Guild = string.Empty;
                    _Rotation.ServerId = 0;
                    _Rotation.Tamer = string.Empty;
                    _Rotation.UpdateInterval = 0;
                }
                return _Rotation;
            }
        }

        private NewsData _News = new NewsData() {
            FirstTab = 0,
            TwitterUrl = DEFAULT_TWITTER_SOURCE
        };
        public NewsData News {
            set {
                _News = value; NotifyPropertyChanged("News");
            }
            get {
                if (!DMOProfile.IsNewsAvailable) {
                    _News.FirstTab = 0;
                }
                return _News;
            }
        }

        #endregion

        #region Image
        private string _ImagePath;
        public string ImagePath {
            set {
                _ImagePath = value; NotifyPropertyChanged("ImagePath"); NotifyPropertyChanged("Image");
            }
            get {
                return _ImagePath;
            }
        }

        private string _ImagePathLoaded;
        private ImageSource _Image = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/NoAvatar.png"));
        [XmlIgnore]
        public ImageSource Image {
            set {
                if (_Image != value) {
                    _Image = value;
                    NotifyPropertyChanged("Image");
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
        #endregion

        #region Game Environment
        private GameEnv _GameEnv = new GameEnv();
        public GameEnv GameEnv {
            set {
                _GameEnv = value; NotifyPropertyChanged("GameEnv");
            }
            get {
                if (!_GameEnv.IsInitialized)
                    _GameEnv.Initialize();
                return _GameEnv;
            }
        }

        [XmlIgnore]
        public string GameType {
            set {
            }
            get {
                switch (GameEnv.GetGameType()) {
                    case Environment.GameEnv.GameType.ADMO: {
                            return "Aeria Games";
                        }
                    case Environment.GameEnv.GameType.GDMO: {
                            return "Joymax";
                        }
                    case Environment.GameEnv.GameType.KDMO_DM: {
                            return "Korea DM";
                        }
                    case Environment.GameEnv.GameType.KDMO_IMBC: {
                            return "Korea IMBC";
                        }
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        [XmlIgnore]
        public byte GameTypeNum {
            set {
                GameEnv.SetGameType((GameEnv.GameType)value);
                GameEnv.LoadType(GameEnv.GetGameType());
                NotifyPropertyChanged("GameEnv");       //We've changed env, so we must update all bindings
                NotifyPropertyChanged("GameType");
                NotifyPropertyChanged("DMOProfile");
                NotifyPropertyChanged("Rotation");      //cuz dmoprofile changed, we must update rotation and news support
                NotifyPropertyChanged("News");
            }
            get {
                return (byte)GameEnv.GetGameType();
            }
        }

        #endregion

        #region DMOLibrary.DMOProfile
        private static Dictionary<Environment.GameEnv.GameType, DMOProfile> profileCollection = new Dictionary<Environment.GameEnv.GameType, DMOProfile>();

        [XmlIgnore]
        public DMOProfile DMOProfile {
            set {
            }
            get {
                DMOProfile profile;
                if (profileCollection.ContainsKey(GameEnv.GetGameType())) {
                    profileCollection.TryGetValue(GameEnv.GetGameType(), out profile);
                } else {
                    switch (GameEnv.GetGameType()) {
                        case Environment.GameEnv.GameType.ADMO:
                            profile = new DMOLibrary.Profiles.Aeria.DMOAeria();
                            break;
                        case Environment.GameEnv.GameType.GDMO:
                            profile = new DMOLibrary.Profiles.Joymax.DMOJoymax();
                            break;
                        case Environment.GameEnv.GameType.KDMO_DM:
                            profile = new DMOLibrary.Profiles.Korea.DMOKorea();
                            break;
                        case Environment.GameEnv.GameType.KDMO_IMBC:
                            profile = new DMOLibrary.Profiles.Korea.DMOKoreaIMBC();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    profileCollection.Add(GameEnv.GetGameType(), profile);
                }
                return profile;
            }
        }

        public static DMOProfile GetJoymaxProfile() {
            DMOProfile profile;
            if (profileCollection.ContainsKey(Environment.GameEnv.GameType.GDMO)) {
                profileCollection.TryGetValue(Environment.GameEnv.GameType.GDMO, out profile);
            } else {
                profile = new DMOLibrary.Profiles.Joymax.DMOJoymax();
                profileCollection.Add(Environment.GameEnv.GameType.GDMO, profile);
            }
            return profile;
        }
        #endregion

        #region Constructors

        public Profile() {
        }

        public Profile(Profile p) {
            this.Id = p.Id;
            this.Name = p.Name;
            this.ImagePath = p.ImagePath;
            this.AppLocaleEnabled = p.AppLocaleEnabled;
            this.UpdateEngineEnabled = p.UpdateEngineEnabled;
            this._Login = new LoginData(p._Login);
            this._Rotation = new RotationData(p._Rotation);
            this._News = new NewsData(p._News);
            this.GameEnv = new GameEnv(p.GameEnv);
        }

        #endregion

        #region Property Change Handler
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
