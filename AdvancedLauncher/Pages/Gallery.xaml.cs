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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Service;

namespace AdvancedLauncher.Pages {
    public partial class Gallery : UserControl {
        private bool IsGalleryInitialized = false;
        private Storyboard ShowWindow;
        private delegate void DoAddThumb(BitmapImage bitmap, string path);
        private GalleryViewModel GalleryModel = new GalleryViewModel();

        private Binding GalleryHint = new Binding("GalleryHint");
        private Binding GalleryNoScreenshots = new Binding("GalleryNoScreenshots");
        private Binding GalleryCantOpenImage = new Binding("GalleryCantOpenImage");

        private string gamePath = string.Empty;
        private string screenshotPath = "\\ScreenShot";
        private string thumbsPath = "\\ScreenShot\\thumbnails";
        private static string thumbPath;
        private JpegEncoder ImageEncoder = new JpegEncoder();

        private Storyboard AnimShow = new Storyboard();
        private Storyboard AnimHide = new Storyboard();
        private bool IsAnimInitialized = false;

        public Gallery() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                Templates.DataContext = GalleryModel;
                ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
                LanguageEnv.Languagechanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
                LauncherEnv.Settings.ProfileChanged += ProfileChanged;
            }
        }

        private void ProfileChanged() {
            GalleryModel.UnLoadData();
            IsGalleryInitialized = false;
        }

        public void Activate() {
            gamePath = LauncherEnv.Settings.CurrentProfile.GameEnv.GamePath;
            try {
                ShowWindow.Begin();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            if (Directory.Exists(gamePath + screenshotPath)) {
                if (Directory.GetFiles(gamePath + screenshotPath, "*.jpg").Length == 0) {
                    Info.SetBinding(TextBlock.TextProperty, GalleryNoScreenshots);
                    return;
                } else {
                    Info.SetBinding(TextBlock.TextProperty, GalleryHint);
                }
            } else {
                Info.SetBinding(TextBlock.TextProperty, GalleryNoScreenshots);
                return;
            }

            if (!IsGalleryInitialized || GalleryModel.Count() != Directory.GetFiles(gamePath + screenshotPath, "*.jpg").Length) {
                UpdateThumbs();
            }
        }

        private void UpdateThumbs() {
            BackgroundWorker bw = new BackgroundWorker();
            GalleryModel.UnLoadData();
            IsLoading(true);

            bw.DoWork += (s, e) => {
                string[] fileList = Directory.GetFiles(gamePath + screenshotPath, "*.jpg");
                if (!Directory.Exists(gamePath + thumbsPath)) {
                    try {
                        Directory.CreateDirectory(gamePath + thumbsPath);
                    } catch {
                        return;
                    }
                }

                for (int i = 0; i < fileList.Length; i++) {
                    BitmapImage bitmap;
                    thumbPath = gamePath + thumbsPath + "\\" + Path.GetFileName(fileList[i]);
                    if (!File.Exists(thumbPath)) {
                        ImageEncoder.ResizeScreenShot(fileList[i], thumbPath);
                    }

                    if (File.Exists(thumbPath)) {
                        bitmap = ReadBitmapFromFile(thumbPath);
                    } else {
                        bitmap = ReadBitmapFromFile(fileList[i]);
                    }

                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoAddThumb((bitmap_, path_) => {
                        GalleryModel.Add(new GalleryItemViewModel() {
                            Thumb = bitmap_,
                            FullPath = path_
                        });
                    }), bitmap, fileList[i]);
                }
            };
            bw.RunWorkerCompleted += (s, e) => {
                IsGalleryInitialized = true;
                IsLoading(false);
            };
            bw.RunWorkerAsync();
        }

        private void OnShowScreenshot(object sender, MouseButtonEventArgs e) {
            string file = ((GalleryItemViewModel)Templates.SelectedItem).FullPath;
            if (!File.Exists(file)) {
                GalleryModel.RemoveAt(Templates.SelectedIndex);
                return;
            }
            try {
                Process.Start("rundll32.exe", System.Environment.SystemDirectory + "\\shimgvw.dll,ImageView_Fullscreen " + file);
            } catch (Exception ex) {
                Utils.MSG_ERROR(LanguageEnv.Strings.GalleryCantOpenImage + ex.Message);
            }
        }

        private void InitializeAnim() {
            if (IsAnimInitialized) {
                return;
            }

            DoubleAnimation dblAnimShow = new DoubleAnimation();
            dblAnimShow.To = 1;
            dblAnimShow.Duration = new Duration(TimeSpan.FromMilliseconds(300));

            DoubleAnimation dblAnimHide = new DoubleAnimation();
            dblAnimHide.To = 0;
            dblAnimHide.Duration = new Duration(TimeSpan.FromMilliseconds(300));

            Storyboard.SetTarget(dblAnimShow, LoaderIcon);
            Storyboard.SetTarget(dblAnimHide, LoaderIcon);
            Storyboard.SetTargetProperty(dblAnimShow, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(dblAnimHide, new PropertyPath(OpacityProperty));

            AnimShow.Children.Add(dblAnimShow);
            AnimHide.Children.Add(dblAnimHide);

            AnimHide.Completed += (s, e) => {
                LoaderIcon.Visibility = Visibility.Collapsed; LoaderIcon.IsEnabled = false;
            };
        }

        private void IsLoading(bool _IsLoading) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                InitializeAnim();
                if (_IsLoading) {
                    LoaderIcon.IsEnabled = true;
                    LoaderIcon.Visibility = Visibility.Visible;
                    AnimShow.Begin();
                } else {
                    AnimHide.Begin();
                }
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
        private ImageSource _Thumb;
        public ImageSource Thumb {
            get {
                return _Thumb;
            }
            set {
                if (value != _Thumb) {
                    _Thumb = value;
                    NotifyPropertyChanged("Thumb");
                }
            }
        }

        private string _FullPath;
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
