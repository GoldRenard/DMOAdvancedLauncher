﻿// ======================================================================
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
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Model.Config {

    /// <summary>
    /// User profile settings
    /// </summary>
    public class Profile : CrossDomainObject, INotifyPropertyChanged {
        private int _Id = 0;

        /// <summary>
        /// Profile identifier
        /// </summary>
        public int Id {
            set {
                _Id = value; NotifyPropertyChanged("Id");
            }
            get {
                return _Id;
            }
        }

        private string Guid;

        private string _Name = "Default";

        /// <summary>
        /// Profile name
        /// </summary>
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

        private string _LaunchMode;

        /// <summary>
        /// Profile launch mode. It defines full name of launcher type.
        /// </summary>
        public string LaunchMode {
            set {
                _LaunchMode = value;
                NotifyPropertyChanged("LaunchMode");
            }
            get {
                return _LaunchMode;
            }
        }

        private bool _UpdateEngineEnabled = false;

        /// <summary>
        /// If <b>True</b>, internal update engine will be enabled for this profile.
        /// Otherwise, user will be forced to stock launcher for game update.
        /// </summary>
        public bool UpdateEngineEnabled {
            set {
                _UpdateEngineEnabled = value; NotifyPropertyChanged("UpdateEngineEnabled");
            }
            get {
                return _UpdateEngineEnabled;
            }
        }

        private bool _KBLCServiceEnabled = false;

        /// <summary>
        /// If <b>True</b>, Leyboard Layout Change Service will be enabled. It will fix
        /// in-game layout changing.
        /// </summary>
        public bool KBLCServiceEnabled {
            set {
                _KBLCServiceEnabled = value; NotifyPropertyChanged("KBLCServiceEnabled");
            }
            get {
                return _KBLCServiceEnabled;
            }
        }

        #region Subcontainers

        private RotationData _Rotation;

        /// <summary>
        /// DigiRotation data
        /// </summary>
        /// <seealso cref="RotationData"/>
        public RotationData Rotation {
            set {
                _Rotation = value; NotifyPropertyChanged("Rotation");
            }
            get {
                return _Rotation;
            }
        }

        private NewsData _News;

        /// <summary>
        /// News data
        /// </summary>
        /// <seealso cref="NewsData"/>
        public NewsData News {
            set {
                _News = value; NotifyPropertyChanged("News");
            }
            get {
                return _News;
            }
        }

        private GameModel _GameModel;

        /// <summary>
        /// Game model data
        /// </summary>
        /// <seealso cref="Config.GameModel"/>
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

        #endregion Subcontainers

        #region Image

        private string _ImagePath;

        /// <summary>
        /// Avatar image path
        /// </summary>
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

        /// <summary>
        /// Avatar image source
        /// </summary>
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

        /// <summary>
        /// <b>True</b> if user have avatar image
        /// </summary>
        public bool HasImage {
            get {
                return Image != null;
            }
        }

        /// <summary>
        /// <b>True</b> if user doesn't have avatar image
        /// </summary>
        public bool NoImage {
            get {
                return Image == null;
            }
        }

        #endregion Image

        #region Constructors

        public Profile() {
            this.Guid = System.Guid.NewGuid().ToString();
            this.Rotation = new RotationData();
            this.News = new NewsData();
            this.GameModel = new GameModel();
        }

        /// <summary>
        /// Creates new <see cref="Profile"/> based on another
        /// </summary>
        /// <param name="p">Source <see cref="Profile"/></param>
        public Profile(Profile p) {
            this.Id = p.Id;
            this.Guid = p.Guid;
            this.Name = p.Name;
            this.ImagePath = p.ImagePath;
            this.LaunchMode = p.LaunchMode;
            this.UpdateEngineEnabled = p.UpdateEngineEnabled;
            this.KBLCServiceEnabled = p.KBLCServiceEnabled;
            this.Rotation = new RotationData(p.Rotation);
            this.News = new NewsData(p.News);
            this.GameModel = new GameModel(p.GameModel);
        }

        #endregion Constructors

        public override int GetHashCode() {
            int prime = 31;
            int result = 1;
            result = prime * result + (Guid == null ? 0 : Guid.GetHashCode());
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
            Profile other = (Profile)obj;
            if (!Guid.Equals(other.Guid)) {
                return false;
            }
            return this.GetHashCode() == obj.GetHashCode();
        }

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