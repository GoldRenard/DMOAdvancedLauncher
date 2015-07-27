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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AdvancedLauncher.Management;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Tools;
using AdvancedLauncher.UI.Commands;
using AdvancedLauncher.UI.Extension;
using Ninject;

namespace AdvancedLauncher.UI.Pages {

    public partial class Gallery : AbstractPage {
        private const string SCREENSHOTS_DIR = "\\ScreenShot";
        private const string THUMBS_DIR = "\\ScreenShot\\thumbnails";

        private bool IsGalleryInitialized = false;

        private delegate void DoAddThumb(BitmapImage bitmap, string path, string dateTime);

        private GalleryViewModel GalleryModel = new GalleryViewModel();

        private JpegEncoder ImageEncoder = new JpegEncoder();

        public Gallery() {
            InitializeComponent();
            Templates.DataContext = GalleryModel;
        }

        public override void PageActivate() {
            base.PageActivate();
            string gamePath = GameManager.Current.GamePath;
            if (Directory.Exists(gamePath + SCREENSHOTS_DIR)) {
                if (Directory.GetFiles(gamePath + SCREENSHOTS_DIR, "*.jpg").Length > 0) {
                    if (!IsGalleryInitialized || GalleryModel.Count() != Directory.GetFiles(gamePath + SCREENSHOTS_DIR, "*.jpg").Length) {
                        UpdateThumbs(gamePath);
                        EmptyText.Visibility = System.Windows.Visibility.Hidden;
                    }
                    return;
                }
            }
            EmptyText.Visibility = System.Windows.Visibility.Visible;
        }

        protected override void ProfileChanged(object sender, EventArgs e) {
            GalleryModel.UnLoadData();
            IsGalleryInitialized = false;
            if (IsPageVisible) {
                PageActivate();
            }
        }

        private void UpdateThumbs(string gamePath) {
            BackgroundWorker bw = new BackgroundWorker();
            GalleryModel.UnLoadData();
            IsLoading(true);

            bw.DoWork += (s, e) => {
                string[] fileList = Directory.GetFiles(gamePath + SCREENSHOTS_DIR, "*.jpg");
                if (!Directory.Exists(gamePath + THUMBS_DIR)) {
                    try {
                        Directory.CreateDirectory(gamePath + THUMBS_DIR);
                    } catch {
                        return;
                    }
                }

                for (int i = 0; i < fileList.Length; i++) {
                    BitmapImage bitmap;
                    string thumbPath = gamePath + THUMBS_DIR + "\\" + Path.GetFileName(fileList[i]);
                    if (!File.Exists(thumbPath)) {
                        ImageEncoder.ResizeScreenShot(fileList[i], thumbPath);
                    }

                    if (File.Exists(thumbPath)) {
                        bitmap = ReadBitmapFromFile(thumbPath);
                    } else {
                        bitmap = ReadBitmapFromFile(fileList[i]);
                    }

                    DateTime result;
                    try {
                        result = DateTime.ParseExact(Path.GetFileNameWithoutExtension(fileList[i]), "yyMMdd_HHmmss", CultureInfo.InvariantCulture);
                    } catch (FormatException) {
                        result = File.GetCreationTime(fileList[i]);
                    }
                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoAddThumb((bitmap_, path_, date_) => {
                        GalleryModel.Add(new GalleryItemViewModel() {
                            Thumb = bitmap_,
                            FullPath = path_,
                            Date = date_
                        });
                    }), bitmap, fileList[i], result.ToString());
                }
            };
            bw.RunWorkerCompleted += (s, e) => {
                IsGalleryInitialized = true;
                IsLoading(false);
            };
            bw.RunWorkerAsync();
        }

        private void IsLoading(bool _IsLoading) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () {
                ProgressRing.IsActive = _IsLoading;
            }));
        }

        public BitmapImage ReadBitmapFromFile(string path) {
            byte[] img_bytes = File.ReadAllBytes(path);

            MemoryStream ms = new MemoryStream(img_bytes, 0, img_bytes.Length);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.CacheOption = BitmapCacheOption.Default;
            bitmap.CreateOptions = BitmapCreateOptions.None;
            RenderOptions.SetBitmapScalingMode(bitmap, BitmapScalingMode.HighQuality);
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
    }

    public class GalleryItemViewModel : INotifyPropertyChanged {
        private double SCALE_CONSTANT = 0.75;

        private ImageSource _Thumb;

        private string _FullPath;

        private string _Date;

        public string Date {
            get {
                return _Date;
            }
            set {
                if (value != _Date) {
                    _Date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        public ImageSource Thumb {
            get {
                return _Thumb;
            }
            set {
                if (value != _Thumb) {
                    _Thumb = value;
                    NotifyPropertyChanged("Thumb");
                    NotifyPropertyChanged("ThumbWidth");
                    NotifyPropertyChanged("ThumbHeight");
                }
            }
        }

        public double ThumbWidth {
            get {
                return Thumb.Width * SCALE_CONSTANT;
            }
        }

        public double ThumbHeight {
            get {
                return Thumb.Height * SCALE_CONSTANT;
            }
        }

        public ModelCommand Command {
            get {
                return new ModelCommand((p) => {
                    try {
                        Process.Start("rundll32.exe", System.Environment.SystemDirectory + "\\shimgvw.dll,ImageView_Fullscreen " + FullPath);
                    } catch (Exception ex) {
                        ILanguageManager LanguageManager = App.Kernel.Get<ILanguageManager>();
                        DialogsHelper.ShowErrorDialog(LanguageManager.Model.GalleryCantOpenImage + ex.Message);
                    }
                });
            }
        }

        public string FullPath {
            get {
                return _FullPath;
            }
            set {
                if (value != _FullPath) {
                    _FullPath = value;
                    NotifyPropertyChanged("FullPath");
                }
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

    public class GalleryViewModel : INotifyPropertyChanged {

        public GalleryViewModel() {
            this.Items = new ObservableCollection<GalleryItemViewModel>();
        }

        public ObservableCollection<GalleryItemViewModel> Items {
            get;
            private set;
        }

        public bool IsDataLoaded {
            get;
            private set;
        }

        public void Add(GalleryItemViewModel item) {
            this.IsDataLoaded = true;
            this.Items.Add(item);
        }

        public void LoadData(List<GalleryItemViewModel> List) {
            this.IsDataLoaded = true;
            foreach (GalleryItemViewModel item in List)
                this.Items.Add(item);
        }

        public void RemoveAt(int index) {
            if (this.IsDataLoaded && index < this.Items.Count)
                this.Items.RemoveAt(index);
        }

        public void UnLoadData() {
            this.IsDataLoaded = false;
            this.Items.Clear();
        }

        public int Count() {
            return this.Items.Count;
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