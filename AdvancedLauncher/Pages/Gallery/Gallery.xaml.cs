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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace AdvancedLauncher
{
    public partial class Gallery : UserControl
    {
        Gallery_DC DContext;
        bool isInitialized = false;
        Storyboard ShowWindow;
        private delegate void DoAddThumb(BitmapImage bitmap, string path);
        private GalleryViewModel GalleryVM = new GalleryViewModel();

        string game_path = string.Empty;
        string screenshot_path = "\\ScreenShot";
        string thumbnails_path = "\\ScreenShot\\thumbnails";
        static string thumb_path;
        jpeg_enc ImageEncoder = new jpeg_enc();

        public Gallery()
        {
            InitializeComponent();
            ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
            DContext = new Gallery_DC();
            LayoutRoot.DataContext = DContext;
            Templates.DataContext = GalleryVM;
        }

        private void UpdateThumbs()
        {
            BackgroundWorker bw = new BackgroundWorker();
            GalleryVM.UnLoadData();
            isLoading(true);

            bw.DoWork += (s, e) =>
            {
                string[] file_list = Directory.GetFiles(game_path + screenshot_path, "*.jpg");
                if (!Directory.Exists(game_path + thumbnails_path))
                {
                    try { Directory.CreateDirectory(game_path + thumbnails_path); }
                    catch { return; }
                }

                for (int i = 0; i < file_list.Length; i++)
                {
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
            bw.RunWorkerCompleted += (s, e) =>
            {
                isInitialized = true;
                isLoading(false);
            };
            bw.RunWorkerAsync();
        }

        #region Interface processing
        public void Activate()
        {
            game_path = App.DMOProfile.GetGamePath();
            try
            {
                ShowWindow.Begin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (Directory.Exists(game_path + screenshot_path))
            {
                if (Directory.GetFiles(game_path + screenshot_path, "*.jpg").Length == 0)
                {
                    Info.Text = LanguageProvider.strings.GAL_NOSCREENSHOTS;
                    return;
                }
                else
                    Info.Text = LanguageProvider.strings.GAL_HELP;
            }
            else
            {
                Info.Text = LanguageProvider.strings.GAL_NOSCREENSHOTS;
                return;
            }

            if (!isInitialized || GalleryVM.Count() != Directory.GetFiles(game_path + screenshot_path, "*.jpg").Length)
                UpdateThumbs();
        }

        void lbi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string file = ((GalleryItemViewModel)Templates.SelectedItem).full_path;
            if (!File.Exists(file))
            {
                GalleryVM.RemoveAt(Templates.SelectedIndex);
                return;
            }
            try { Process.Start("rundll32.exe", Environment.SystemDirectory + "\\shimgvw.dll,ImageView_Fullscreen " + file); }
            catch (Exception ex) { Utils.MSG_ERROR(LanguageProvider.strings.GAL_CANT_OPEN + ex.Message); }
        }

        private void isLoading(bool state)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
            {
                if (state)
                {
                    LoaderIcon.IsEnabled = state;
                    LoaderIcon.Visibility = Visibility.Visible;
                }
                Storyboard sb = new Storyboard();
                DoubleAnimation dbl_anim = new DoubleAnimation();
                dbl_anim.From = state ? 0 : 1;
                dbl_anim.To = state ? 1 : 0;
                dbl_anim.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                Storyboard.SetTarget(dbl_anim, LoaderIcon);
                Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
                sb.Children.Add(dbl_anim);
                sb.Completed += (s, e) =>
                {
                    LoaderIcon.Visibility = state ? Visibility.Visible : Visibility.Collapsed;
                    LoaderIcon.IsEnabled = state;
                };
                sb.Begin();
            }));
        }

        #endregion

        public BitmapImage ReadBitmapFromFile(string path)
        {
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
}
