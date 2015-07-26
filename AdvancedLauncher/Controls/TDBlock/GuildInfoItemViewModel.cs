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

using AdvancedLauncher.Management;
using DMOLibrary.Database.Entity;

namespace AdvancedLauncher.Controls {

    public class GuildInfoItemViewModel : AbstractItemViewModel<Guild> {

        protected override void LanguageChanged() {
            NotifyPropertyChanged("Name");
        }

        private string _Name;

        public string Name {
            get {
                return LanguageManager.Model[_Name];
            }
            set {
                if (value != _Name) {
                    _Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private object _Value;

        public object Value {
            get {
                return _Value;
            }
            set {
                if (value != _Value) {
                    _Value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }
}