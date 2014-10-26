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
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.IO;
using System.Diagnostics;

using AdvancedLauncher.Environment;
using AdvancedLauncher.Service;

namespace AdvancedLauncher.Pages {
    public partial class Gallery : UserControl {
        bool IsGalleryInitialized = false;
        Storyboard ShowWindow;
        private delegate void DoAddThumb(BitmapImage bitmap, string path);
        private GalleryViewModel GalleryVM = new GalleryViewModel();

        Binding GalleryHint = new Binding("GalleryHint");
        Binding GalleryNoScreenshots = new Binding("GalleryNoScreenshots");
        Binding GalleryCantOpenImage = new Binding("GalleryCantOpenImage");

        string game_path = string.Empty;
        string screenshot_path = "\\ScreenShot";
        string thumbnails_path = "\\ScreenShot\\thumbnails";
        static string thumb_path;
        JpegEncoder ImageEncoder = new JpegEncoder();

        public Gallery() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                Templates.DataContext = GalleryVM;
                ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
                LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
                LauncherEnv.Settings.ProfileChanged += ProfileChanged;
            }
        }

        void ProfileChanged() {
            GalleryVM.UnLoadData();
            IsGalleryInitialized = false;
        }

        public void Activate() {
            game_path = LauncherEnv.Settings.pCurrent.GameEnv.GamePath;
            try {
                ShowWindow.Begin();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            if (Directory.Exists(game_path + screenshot_path)) {
                if (Directory.GetFiles(game_path + screenshot_path, "*.jpg").Length == 0) {
                    Info.SetBinding(TextBlock.TextProperty, GalleryNoScreenshots);
                    return;
                } else
                    Info.SetBinding(TextBlock.TextProperty, GalleryHint);
            } else {
                Info.SetBinding(TextBlock.TextProperty, GalleryNoScreenshots);
                return;
            }

            if (!IsGalleryInitialized || GalleryVM.Count() != Directory.GetFiles(game_path + screenshot_path, "*.jpg").Length)
                UpdateThumbs();
        }

        private void UpdateThumbs() {
            BackgroundWorker bw = new BackgroundWorker();
            GalleryVM.UnLoadData();
            IsLoading(true);

            bw.DoWork += (s, e) => {
                string[] file_list = Directory.GetFiles(game_path + screenshot_path, "*.jpg");
                if (!Directory.Exists(game_path + thumbnails_path)) {
                    try { Directory.CreateDirectory(game_path + thumbnails_path); } catch { return; }
                }

                for (int i = 0; i < file_list.Length; i++) {
                    BitmapImage bitmap;
                    thumb_path = game_path + thumbnails_path + "\\" + Path.GetFileName(file_list[i]);
                    if (!File.Exists(thumb_path))
                        ImageEncoder.ResizeScreenShot(file_list[i], thumb_path);

                    if (File.Exists(thumb_path))
                        bitmap = ReadBitmapFromFile(thumb_path);
                    else
                        bitmap = ReadBitmapFromFile(file_list[i]);

                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoAddThumb((bitmap_, path_) => { GalleryVM.Add(new GalleryItemViewModel() { Thumb = bitmap_, full_path = path_ }); }), bitmap, file_list[i]);
                }
            };
            bw.RunWorkerCompleted += (s, e) => {
                IsGalleryInitialized = true;
                IsLoading(false);
            };
            bw.RunWorkerAsync();
        }

        void lbi_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            string file = ((GalleryItemViewModel)Templates.SelectedItem).full_path;
            if (!File.Exists(file)) {
                GalleryVM.RemoveAt(Templates.SelectedIndex);
                return;
            }
            try { Process.Start("rundll32.exe", System.Environment.SystemDirectory + "\\shimgvw.dll,ImageView_Fullscreen " + file); } catch (Exception ex) { Utils.MSG_ERROR(LanguageEnv.Strings.GalleryCantOpenImage + ex.Message); }
        }

        Storyboard sbAnimShow = new Storyboard();
        Storyboard sbAnimHide = new Storyboard();
        private bool IsAnimInitialized = false;

        private void InitializeAnim() {
            if (IsAnimInitialized)
                return;

            DoubleAnimation dbl_anim_show = new DoubleAnimation();
            dbl_anim_show.To = 1;
            dbl_anim_show.Duration = new Duration(TimeSpan.FromMilliseconds(300));

            DoubleAnimation dbl_anim_hide = new DoubleAnimation();
            dbl_anim_hide.To = 0;
            dbl_anim_hide.Duration = new Duration(TimeSpan.FromMilliseconds(300));

            Storyboard.SetTarget(dbl_anim_show, LoaderIcon);
            Storyboard.SetTarget(dbl_anim_hide, LoaderIcon);
            Storyboard.SetTargetProperty(dbl_anim_show, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(dbl_anim_hide, new PropertyPath(OpacityProperty));

            sbAnimShow.Children.Add(dbl_anim_show);
            sbAnimHide.Children.Add(dbl_anim_hide);

            sbAnimHide.Completed += (s, e) => { LoaderIcon.Visibility = Visibility.Collapsed; LoaderIcon.IsEnabled = false; };
        }

        private void IsLoading(bool _IsLoading) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                InitializeAnim();
                if (_IsLoading) {
                    LoaderIcon.IsEnabled = true;
                    LoaderIcon.Visibility = Visibility.Visible;
                    sbAnimShow.Begin();
                } else
                    sbAnimHide.Begin();
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

        private string _full_path;
        public string full_path {
            get {
                return _full_path;
            }
            set {
                if (value != _full_path) {
                    _full_path = value;
                    NotifyPropertyChanged("full_path");
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

        public ObservableCollection<GalleryItemViewModel> Items { get; private set; }

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
