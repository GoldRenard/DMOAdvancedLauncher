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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml.Serialization;
using DMOLibrary.Profiles;


namespace AdvancedLauncher.Environment.Containers {
    [XmlType(TypeName = "Profile")]
    public class Profile : INotifyPropertyChanged {
        private int _pId = 0;
        [XmlAttribute("Id")]
        public int pId {
            set { _pId = value; NotifyPropertyChanged("Id"); }
            get { return _pId; }
        }

        private string _pName = "Default";
        [XmlAttribute("Name")]
        public string pName {
            set { _pName = value; NotifyPropertyChanged("pName"); }
            get { return _pName; }
        }

        private bool pAppLocale = true;
        [XmlAttribute]
        public bool AppLocale {
            set { pAppLocale = value; NotifyPropertyChanged("AppLocale"); }
            get {
                if (!Service.ApplicationLauncher.IsALSupported)
                    return false;
                return pAppLocale;
            }
        }

        private bool pUpdateEngine = false;
        [XmlAttribute]
        public bool UpdateEngine {
            set { pUpdateEngine = value; NotifyPropertyChanged("UpdateEngine"); }
            get { return pUpdateEngine; }
        }

        #region Subcontainers

        private LoginData pLogin = new LoginData();
        public LoginData Login {
            set { pLogin = value; NotifyPropertyChanged("Login"); }
            get {
                if (!DMOProfile.IsLoginRequired) {
                    pLogin.Password = string.Empty;
                    pLogin.pLastSessionArgs = string.Empty;
                    pLogin.User = string.Empty;
                }
                return pLogin;
            }
        }

        private RotationData pRotation = new RotationData() { URate = 2 };
        public RotationData Rotation {
            set { pRotation = value; NotifyPropertyChanged("Rotation"); }
            get {
                if (!DMOProfile.IsWebAvailable) {
                    pRotation.Guild = string.Empty;
                    pRotation.ServerId = 0;
                    pRotation.Tamer = string.Empty;
                    pRotation.URate = 0;
                }
                return pRotation;
            }
        }

        private NewsData pNews = new NewsData() { FirstTab = 0, TwitterUrl = "http://renamon.ru/launcher/dmor_timeline.php" };
        public NewsData News {
            set { pNews = value; NotifyPropertyChanged("News"); }
            get {
                if (!DMOProfile.IsNewsAvailable)
                    pNews.FirstTab = 0;
                return pNews;
            }
        }

        #endregion

        #region Image
        private string pImagePath;
        public string ImagePath {
            set { pImagePath = value; NotifyPropertyChanged("ImagePath"); NotifyPropertyChanged("Image"); }
            get { return pImagePath; }
        }

        private string pImagePathLoaded;
        private ImageSource pImage = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/NoAvatar.png"));
        [XmlIgnore]
        public ImageSource Image {
            set {
                if (pImage != value) {
                    pImage = value;
                    NotifyPropertyChanged("Image");
                }
            }
            get {
                if (pImagePath != null) {
                    if (pImagePath != pImagePathLoaded) {
                        if (File.Exists(pImagePath)) {
                            pImage = new BitmapImage(new Uri(pImagePath));
                            pImagePathLoaded = pImagePath;
                        }
                    }
                }
                return pImage;
            }
        }
        #endregion

        #region Game Environment
        private GameEnv pGameEnv = new GameEnv();
        public GameEnv GameEnv {
            set { pGameEnv = value; NotifyPropertyChanged("GameEnv"); }
            get {
                if (!pGameEnv.IsInitialized)
                    pGameEnv.Initialize();
                return pGameEnv;
            }
        }

        [XmlIgnore]
        public string GameType {
            set { }
            get {
                switch (GameEnv.pType) {
                    case Environment.GameEnv.GameType.ADMO: { return "Aeria Games"; }
                    case Environment.GameEnv.GameType.GDMO: { return "Joymax"; }
                    case Environment.GameEnv.GameType.KDMO_DM: { return "Korea DM"; }
                    case Environment.GameEnv.GameType.KDMO_IMBC: { return "Korea IMBC"; }
                }
                return string.Empty;
            }
        }

        [XmlIgnore]
        public byte GameTypeNum {
            set {
                GameEnv.pType = (GameEnv.GameType)value;
                GameEnv.LoadType(GameEnv.pType);
                NotifyPropertyChanged("GameEnv");       //We've changed env, so we must update all bindings
                NotifyPropertyChanged("GameType");
                NotifyPropertyChanged("DMOProfile");
                NotifyPropertyChanged("Rotation");      //cuz dmoprofile changed, we must update rotation and news support
                NotifyPropertyChanged("News");
            }
            get { return (byte)GameEnv.pType; }
        }

        #endregion

        #region DMOLibrary.DMOProfile
        private static DMOProfile _DMOAeria = null;
        private static DMOProfile _DMOJoymax = null;
        private static DMOProfile _DMOKorea = null;
        private static DMOProfile _DMOKoreaIMBC = null;
        [XmlIgnore]
        public DMOProfile DMOProfile {
            set { }
            get {
                switch (GameEnv.pType) {
                    case Environment.GameEnv.GameType.ADMO: {
                            if (_DMOAeria == null)
                                _DMOAeria = new DMOLibrary.Profiles.Aeria.DMOAeria();
                            return _DMOAeria;
                        }
                    case Environment.GameEnv.GameType.GDMO: {
                            if (_DMOJoymax == null)
                                _DMOJoymax = new DMOLibrary.Profiles.Joymax.DMOJoymax();
                            return _DMOJoymax;
                        }
                    case Environment.GameEnv.GameType.KDMO_DM: {
                            if (_DMOKorea == null)
                                _DMOKorea = new DMOLibrary.Profiles.Korea.DMOKorea();
                            return _DMOKorea;
                        }
                    case Environment.GameEnv.GameType.KDMO_IMBC: {
                            if (_DMOKoreaIMBC == null)
                                _DMOKoreaIMBC = new DMOLibrary.Profiles.Korea.DMOKoreaIMBC();
                            return _DMOKoreaIMBC;
                        }
                    default:
                        return null;
                }
            }
        }

        public static DMOProfile GetJoymaxProfile() {
            if (_DMOJoymax == null)
                _DMOJoymax = new DMOLibrary.Profiles.Joymax.DMOJoymax();
            return _DMOJoymax;
        }
        #endregion

        #region Constructors

        public Profile() { }

        public Profile(Profile p) {
            this.pId = p.pId;
            this.pName = p.pName;
            this.ImagePath = p.ImagePath;
            this.AppLocale = p.AppLocale;
            this.UpdateEngine = p.UpdateEngine;
            this.pLogin = new LoginData(p.pLogin);
            this.pRotation = new RotationData(p.pRotation);
            this.pNews = new NewsData(p.pNews);
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
