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

using System.Windows.Media;
using DMOLibrary.Database.Entity;

namespace AdvancedLauncher.UI.Controls {

    public class TamerItemViewModel : AbstractItemViewModel<Tamer> {
        private string _TName;

        public string TName {
            get {
                return _TName;
            }
            set {
                _TName = value;
                NotifyPropertyChanged("TName");
            }
        }

        private int _Level;

        public int Level {
            get {
                return _Level;
            }
            set {
                _Level = value;
                NotifyPropertyChanged("Level");
            }
        }

        private long _Rank;

        public long Rank {
            get {
                return _Rank;
            }
            set {
                _Rank = value;
                NotifyPropertyChanged("Rank");
            }
        }

        private string _PName;

        public string PName {
            get {
                return _PName;
            }
            set {
                _PName = value;
                NotifyPropertyChanged("PName");
            }
        }

        private int _DCnt;

        public int DCnt {
            get {
                return _DCnt;
            }
            set {
                _DCnt = value;
                NotifyPropertyChanged("DCnt");
            }
        }

        private string _TType;

        public string TType {
            get {
                return _TType;
            }
            set {
                _TType = value;
                NotifyPropertyChanged("TType");
            }
        }

        private Tamer _Tamer;

        public Tamer Tamer {
            get {
                return _Tamer;
            }
            set {
                _Tamer = value;
                NotifyPropertyChanged("Tamer");
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