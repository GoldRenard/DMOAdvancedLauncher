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
    public class TamerViewModel : INotifyPropertyChanged
    {
        static string images_path = "community_icons\\{0}.png";
        DMOFileSystem res_fs;

        public TamerViewModel()
        {
            this.Items = new ObservableCollection<TamerItemViewModel>();
            res_fs = new DMOFileSystem(32, SettingsProvider.APP_PATH + SettingsProvider.RES_HF_FILE, SettingsProvider.APP_PATH + SettingsProvider.RES_PF_FILE);
        }

        public ObservableCollection<TamerItemViewModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData(List<tamer> List)
        {
            this.IsDataLoaded = true;
            if (App.DMOProfile.Database.OpenConnection())
            {
                foreach (tamer item in List)
                    this.Items.Add(new TamerItemViewModel { TName = item.Name, TType = App.DMOProfile.Database.Tamer_GetTypeById(item.Type_id).Name, Level = item.Lvl, PName = item.Partner_name, Rank = item.Rank, DCnt = item.Digimons.Count, Tamer = item, Image = GetImage(item.Type_id) });
                App.DMOProfile.Database.CloseConnection();
            }
            else
                foreach (tamer item in List)
                    this.Items.Add(new TamerItemViewModel { TName = item.Name, TType = "Unknown", Level = item.Lvl, PName = item.Partner_name, Rank = item.Rank, DCnt = item.Digimons.Count, Tamer = item, Image = GetImage(item.Type_id) });
        }

        public void UnLoadData()
        {
            this.IsDataLoaded = false;
            this.Items.Clear();
        }

        private bool _sortASC;
        private Type last_type;
        public void Sort<TType>(Func<TamerItemViewModel, TType> keySelector)
        {
            List<TamerItemViewModel> sortedList;
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