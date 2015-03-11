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
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using AdvancedLauncher.Environment;
using DMOLibrary;
using DMOLibrary.Database.Context;
using DMOLibrary.Database.Entity;

namespace AdvancedLauncher.Controls {

    public class TamerViewModel : INotifyPropertyChanged {

        public TamerViewModel() {
            this.Items = new ObservableCollection<TamerItemViewModel>();
        }

        public ObservableCollection<TamerItemViewModel> Items {
            get;
            private set;
        }

        public bool IsDataLoaded {
            get;
            private set;
        }

        public void LoadData(List<TamerOld> List) {
            this.IsDataLoaded = true;
            foreach (TamerOld item in List) {
                TamerType tamerType =  MainContext.Instance.FindTamerTypeByCode(item.TypeId);
                string tamerName = tamerType != null ? tamerType.Name : "???";
                this.Items.Add(new TamerItemViewModel {
                    TName = item.Name,
                    TType = MainContext.Instance.FindTamerTypeByCode(item.TypeId).Name,
                    Level = item.Lvl,
                    PName = item.PartnerName,
                    Rank = item.Rank,
                    DCnt = item.Digimons.Count,
                    Tamer = item,
                    Image = GetImage(item.TypeId)
                });
            }
        }

        public void UnLoadData() {
            this.IsDataLoaded = false;
            this.Items.Clear();
        }

        private bool _sortASC;
        private Type last_type;

        public void Sort<TType>(Func<TamerItemViewModel, TType> keySelector) {
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

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private struct DigiImage {
            public int Id;
            public BitmapImage Image;
        }

        private static List<DigiImage> ImagesCollection = new List<DigiImage>();

        public static BitmapImage GetImage(int digi_id) {
            DigiImage Image = ImagesCollection.Find(i => i.Id == digi_id);
            if (Image.Image != null)
                return Image.Image;

            string ImageFile = string.Format("{0}\\Community\\{1}.png", LauncherEnv.GetResourcesPath(), digi_id);

            //If we don't have image, try to download it
            if (!File.Exists(ImageFile)) {
                try {
                    LauncherEnv.WebClient.DownloadFile(string.Format("{0}Community/{1}.png", LauncherEnv.REMOTE_PATH, digi_id), ImageFile);
                } catch {
                }
            }

            if (File.Exists(ImageFile)) {
                Stream str = File.OpenRead(ImageFile);
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
                ImagesCollection.Add(new DigiImage() {
                    Image = bitmap,
                    Id = digi_id
                });
                return bitmap;
            }
            return null;
        }
    }
}