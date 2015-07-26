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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AdvancedLauncher.Model {

    public class JoymaxViewModel : INotifyPropertyChanged {

        public JoymaxViewModel() {
            this.Items = new ObservableCollection<JoymaxItemViewModel>();
        }

        public ObservableCollection<JoymaxItemViewModel> Items {
            get;
            private set;
        }

        public bool IsDataLoaded {
            get;
            private set;
        }

        public void LoadData(List<JoymaxItemViewModel> List) {
            this.IsDataLoaded = true;
            foreach (JoymaxItemViewModel item in List) {
                this.Items.Add(new JoymaxItemViewModel {
                    Title = item.Title,
                    Content = item.Content,
                    Link = item.Link,
                    TypeName = item.TypeName,
                    Date = item.Date,
                    ImgVB = item.ImgVB
                });
            }
        }

        public void UnLoadData() {
            this.IsDataLoaded = false;
            this.Items.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}