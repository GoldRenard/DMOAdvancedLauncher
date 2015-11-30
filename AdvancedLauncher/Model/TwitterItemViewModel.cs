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
using System.Windows.Media;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.UI.Controls;

namespace AdvancedLauncher.Model {

    public class TwitterItemViewModel : AbstractItemViewModel<NewsBlock.UserStatus> {

        public TwitterItemViewModel(ILanguageManager LanguageManager) : base(LanguageManager) {
        }

        protected override void OnLanguageChanged(object sender, SDK.Model.Events.BaseEventArgs e) {
            NotifyPropertyChanged("LocalizedDate");
        }

        private string _UserName;

        public string UserName {
            get {
                return _UserName;
            }
            set {
                if (value != _UserName) {
                    _UserName = value;
                    NotifyPropertyChanged("UserName");
                }
            }
        }

        private string _UserLink;

        public string UserLink {
            get {
                return _UserLink;
            }
            set {
                if (value != _UserLink) {
                    _UserLink = value;
                    NotifyPropertyChanged("UserLink");
                }
            }
        }

        private string _StatusLink;

        public string StatusLink {
            get {
                return _StatusLink;
            }
            set {
                if (value != _StatusLink) {
                    _StatusLink = value;
                    NotifyPropertyChanged("StatusLink");
                }
            }
        }

        private string _Title;

        public string Title {
            get {
                return _Title;
            }
            set {
                if (value != _Title) {
                    _Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private DateTime _Date;

        public DateTime Date {
            get {
                return _Date;
            }
            set {
                if (value != _Date) {
                    _Date = value;
                    NotifyPropertyChanged("LocalizedDate");
                    NotifyPropertyChanged("Date");
                }
            }
        }

        public string LocalizedDate {
            get {
                return LanguageManager.Model.NewsPubDate + ": " + _Date.ToLocalTime().ToUniversalTime();
            }
        }

        private ImageSource _Image;

        public ImageSource Image {
            get {
                return _Image;
            }
            set {
                if (value != _Image) {
                    _Image = value;
                    NotifyPropertyChanged("Image");
                }
            }
        }
    }
}