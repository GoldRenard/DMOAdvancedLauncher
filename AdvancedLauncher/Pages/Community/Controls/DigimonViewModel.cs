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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Linq;
using DMOLibrary.DMOFileSystem;
using DMOLibrary;
using System.IO;

namespace AdvancedLauncher
{
    public class DigimonViewModel : INotifyPropertyChanged
    {
        static string SIZE_FORMAT = "{0}cm ({1}%)";
        static string images_path = "community_icons\\{0}.png";
        DMOFileSystem res_fs;
        public DigimonViewModel()
        {
            this.Items = new ObservableCollection<DigimonItemViewModel>();
            res_fs = new DMOFileSystem(32, SettingsProvider.APP_PATH + SettingsProvider.RES_HF_FILE, SettingsProvider.APP_PATH + SettingsProvider.RES_PF_FILE);
        }

        public ObservableCollection<DigimonItemViewModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData(tamer tamer)
        {
            this.IsDataLoaded = true;
            string TYPE_NAME;
            digimon_type dtype;
            if (App.DMOProfile.Database.OpenConnection())
            {
                foreach (digimon item in tamer.Digimons)
                {
                    dtype = App.DMOProfile.Database.Digimon_GetTypeById(item.Type_id);
                    TYPE_NAME = dtype.Name;
                    if (dtype.Name_alt != null)
                        TYPE_NAME += " (" + dtype.Name_alt + ")";
                    this.Items.Add(new DigimonItemViewModel { DName = item.Name, DType = TYPE_NAME, Image = GetImage(item.Type_id), TName = tamer.Name, Level = item.Lvl, SizePC = item.Size_pc, Size = string.Format(SIZE_FORMAT, item.Size_cm, item.Size_pc), Rank = item.Rank });
                }
                App.DMOProfile.Database.CloseConnection();
            }
            else
                foreach (digimon item in tamer.Digimons)
                    this.Items.Add(new DigimonItemViewModel { DName = item.Name, DType = "Unknown", Image = GetImage(item.Type_id), TName = tamer.Name, Level = item.Lvl, SizePC = item.Size_pc, Size = string.Format(SIZE_FORMAT, item.Size_cm, item.Size_pc), Rank = item.Rank });
        }

        public void LoadData(List<tamer> tamers)
        {
            this.IsDataLoaded = true;
            string TYPE_NAME;
            digimon_type dtype;
            if (App.DMOProfile.Database.OpenConnection())
            {
                foreach (tamer t in tamers)
                {
                    foreach (digimon item in t.Digimons)
                    {
                        dtype = App.DMOProfile.Database.Digimon_GetTypeById(item.Type_id);
                        TYPE_NAME = dtype.Name;
                        if (dtype.Name_alt != null)
                            TYPE_NAME += " (" + dtype.Name_alt + ")";
                        this.Items.Add(new DigimonItemViewModel { DName = item.Name, DType = TYPE_NAME, Image = GetImage(item.Type_id), TName = t.Name, Level = item.Lvl, SizePC = item.Size_pc, Size = string.Format(SIZE_FORMAT, item.Size_cm, item.Size_pc), Rank = item.Rank });
                    }
                }
                App.DMOProfile.Database.CloseConnection();
            }
            else
                foreach (tamer t in tamers)
                    foreach (digimon item in t.Digimons)
                        this.Items.Add(new DigimonItemViewModel { DName = item.Name, DType = "Unknown", Image = GetImage(item.Type_id), TName = t.Name, Level = item.Lvl, SizePC = item.Size_pc, Size = string.Format(SIZE_FORMAT, item.Size_cm, item.Size_pc), Rank = item.Rank });
        }

        public void RemoveAt(int index)
        {
            if (this.IsDataLoaded && index < this.Items.Count)
            {
                this.Items.RemoveAt(index);
            }
        }

        public void UnLoadData()
        {
            this.IsDataLoaded = false;
            this.Items.Clear();
        }

        private bool _sortASC;
        private Type last_type;
        public void Sort<TType>(Func<DigimonItemViewModel, TType> keySelector)
        {
            List<DigimonItemViewModel> sortedList;
            if (last_type != typeof(TType))
                _sortASC = true;

            if (_sortASC)
                sortedList = Items.OrderBy(keySelector).ToList();
            else
                sortedList = Items.OrderByDescending(keySelector).ToList();

            last_type = typeof(TType);
            _sortASC = !_sortASC;

            this.Items.Clear();
            foreach (var sortedItem in sortedList)
                this.Items.Add(sortedItem);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public BitmapImage GetImage(int digi_id)
        {
            Stream str = res_fs.ReadFile(string.Format(images_path, digi_id));
            if (str == null)
                return null;
            MemoryStream img_stream = new MemoryStream();
            str.CopyTo(img_stream);
            str.Close();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = img_stream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
    }
}