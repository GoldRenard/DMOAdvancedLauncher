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

namespace AdvancedLauncher.Model {

    public class DigimonItemViewModel : AbstractItemViewModel<Digimon> {
        private string _DName;

        public string DName {
            get {
                return _DName;
            }
            set {
                if (value != _DName) {
                    _DName = value;
                    NotifyPropertyChanged("DName");
                }
            }
        }

        private string _TName;

        public string TName {
            get {
                return _TName;
            }
            set {
                if (value != _TName) {
                    _TName = value;
                    NotifyPropertyChanged("TName");
                }
            }
        }

        private string _DType;

        public string DType {
            get {
                return _DType;
            }
            set {
                if (value != _DType) {
                    _DType = value;
                    NotifyPropertyChanged("DType");
                }
            }
        }

        private int _Level;

        public int Level {
            get {
                return _Level;
            }
            set {
                if (value != _Level) {
                    _Level = value;
                    NotifyPropertyChanged("Level");
                }
            }
        }

        private string _Size;

        public string Size {
            get {
                return _Size;
            }
            set {
                if (value != _Size) {
                    _Size = value;
                    NotifyPropertyChanged("Size");
                }
            }
        }

        private int _SizePC;

        public int SizePC {
            get {
                return _SizePC;
            }
            set {
                if (value != _SizePC) {
                    _SizePC = value;
                    NotifyPropertyChanged("SizePC");
                }
            }
        }

        private int _PSize;

        public int PSize {
            get {
                return _PSize;
            }
            set {
                if (value != _PSize) {
                    _PSize = value;
                    NotifyPropertyChanged("PSize");
                }
            }
        }

        private long _Rank;

        public long Rank {
            get {
                return _Rank;
            }
            set {
                if (value != _Rank) {
                    _Rank = value;
                    NotifyPropertyChanged("Rank");
                }
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