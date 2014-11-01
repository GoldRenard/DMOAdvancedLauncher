// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Linq;
using AdvancedLauncher.Environment.Containers;
using DMOLibrary;

namespace AdvancedLauncher.Controls {

    public class DigimonViewModel : INotifyPropertyChanged {
        private static string SIZE_FORMAT = "{0}cm ({1}%)";

        public DigimonViewModel() {
            this.Items = new ObservableCollection<DigimonItemViewModel>();
        }

        public ObservableCollection<DigimonItemViewModel> Items {
            get;
            private set;
        }

        public bool IsDataLoaded {
            get;
            private set;
        }

        private void LoadDigimonList(bool dbConnected, Tamer tamer) {
            string typeName;
            DigimonType dtype;
            if (dbConnected) {
                foreach (Digimon item in tamer.Digimons) {
                    dtype = Profile.GetJoymaxProfile().Database.GetDigimonTypeById(item.TypeId).GetValueOrDefault();
                    typeName = dtype.Name;
                    if (dtype.NameAlt != null) {
                        typeName += " (" + dtype.NameAlt + ")";
                    }
                    this.Items.Add(new DigimonItemViewModel {
                        DName = item.Name,
                        DType = typeName,
                        Image = IconHolder.GetImage(item.TypeId),
                        TName = tamer.Name,
                        Level = item.Lvl,
                        SizePC = item.SizePc,
                        Size = string.Format(SIZE_FORMAT, item.SizeCm, item.SizePc),
                        Rank = item.Rank
                    });
                }
            } else {
                foreach (Digimon item in tamer.Digimons) {
                    this.Items.Add(new DigimonItemViewModel {
                        DName = item.Name,
                        DType = "Unknown",
                        Image = IconHolder.GetImage(item.TypeId),
                        TName = tamer.Name,
                        Level = item.Lvl,
                        SizePC = item.SizePc,
                        Size = string.Format(SIZE_FORMAT, item.SizeCm, item.SizePc),
                        Rank = item.Rank
                    });
                }
            }
        }

        public void LoadData(Tamer tamer) {
            this.IsDataLoaded = true;
            bool isConnected = Profile.GetJoymaxProfile().Database.OpenConnection();
            LoadDigimonList(isConnected, tamer);
            if (isConnected) {
                Profile.GetJoymaxProfile().Database.CloseConnection();
            }
        }

        public void LoadData(List<Tamer> tamers) {
            this.IsDataLoaded = true;
            bool isConnected = Profile.GetJoymaxProfile().Database.OpenConnection();
            foreach (Tamer tamer in tamers) {
                LoadDigimonList(isConnected, tamer);
            }
            if (isConnected) {
                Profile.GetJoymaxProfile().Database.CloseConnection();
            }
        }

        public void RemoveAt(int index) {
            if (this.IsDataLoaded && index < this.Items.Count) {
                this.Items.RemoveAt(index);
            }
        }

        public void UnLoadData() {
            this.IsDataLoaded = false;
            this.Items.Clear();
        }

        private bool _sortASC;
        private Type last_type;

        public void Sort<TType>(Func<DigimonItemViewModel, TType> keySelector) {
            List<DigimonItemViewModel> sortedList;
            if (last_type != typeof(TType)) {
                _sortASC = true;
            }

            if (_sortASC) {
                sortedList = Items.OrderBy(keySelector).ToList();
            } else {
                sortedList = Items.OrderByDescending(keySelector).ToList();
            }

            last_type = typeof(TType);
            _sortASC = !_sortASC;

            this.Items.Clear();
            foreach (var sortedItem in sortedList) {
                this.Items.Add(sortedItem);
            }
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