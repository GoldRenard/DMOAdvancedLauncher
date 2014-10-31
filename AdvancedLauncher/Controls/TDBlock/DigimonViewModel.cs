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
                        Image = GetImage(item.TypeId),
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
                        Image = GetImage(item.TypeId),
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

        private struct DigiImage {
            public int Id;
            public BitmapImage Image;
        }

        private static List<DigiImage> ImagesCollection = new List<DigiImage>();

        public static BitmapImage GetImage(int digimonId) {
            DigiImage Image = ImagesCollection.Find(i => i.Id == digimonId);
            if (Image.Image != null) {
                return Image.Image;
            }

            string Image3rdDirectory = string.Format("{0}\\Community\\3rd", LauncherEnv.GetResourcesPath());
            string ImageFile = string.Format("{0}\\Community\\{1}.png", LauncherEnv.GetResourcesPath(), digimonId);
            string ImageFile3rd = string.Format("{0}\\{1}.png", Image3rdDirectory, digimonId);

            //If we don't have image, try to download it from author's resource
            if (!File.Exists(ImageFile)) {
                try {
                    LauncherEnv.WebClient.DownloadFile(string.Format("{0}Community/{1}.png", LauncherEnv.REMOTE_PATH, digimonId), ImageFile);
                } catch {
                }
            }

            //If we don't have image yet, try to download it from joymsx
            if (!File.Exists(ImageFile) && !File.Exists(ImageFile3rd)) {
                try {
                    if (!Directory.Exists(Image3rdDirectory)) {
                        Directory.CreateDirectory(Image3rdDirectory);
                    }
                    LauncherEnv.WebClient.DownloadFile(string.Format("http://img.joymax.com/property/digimon/digimon_v1/us/ranking/icon/{0}.gif", digimonId), ImageFile3rd);
                } catch {
                }
            }

            //If we don't have image yet, try to download it from IMBC
            if (!File.Exists(ImageFile) && !File.Exists(ImageFile3rd)) {
                try {
                    LauncherEnv.WebClient.DownloadFile(string.Format("http://dm.imbc.com/images/ranking/icon/{0}.gif", digimonId), ImageFile3rd);
                } catch {
                }
            }

            //If we don't have our own image but downloaded 3rd one, use it
            if (!File.Exists(ImageFile) && File.Exists(ImageFile3rd)) {
                ImageFile = ImageFile3rd;
            }

            if (File.Exists(ImageFile)) {
                Stream str = File.OpenRead(ImageFile);
                if (str == null) {
                    return null;
                }
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
                    Id = digimonId
                });
                return bitmap;
            }
            return null;
        }
    }
}