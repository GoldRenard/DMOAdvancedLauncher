// ======================================================================
// GLOBAL DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
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
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.IO;
using System.ComponentModel;
using DMOLibrary;
using DMOLibrary.DMOWebInfo;
using DMOLibrary.DMOFileSystem;

namespace AdvancedLauncher
{
    public partial class DigiRotation : UserControl
    {

        string ROT_GUILD;
        server ROT_SERV;
        DigiRotation_DC DContext;
        public bool isLoaded = false;

        static string images_path = "rotation_icons\\{0}.png";
        static BitmapImage unk_digi = new BitmapImage(new Uri(@"images/unknown.png", UriKind.Relative));
        static BitmapImage medal_gold = new BitmapImage(new Uri(@"images/gold.png", UriKind.Relative));
        static BitmapImage medal_silver = new BitmapImage(new Uri(@"images/silver.png", UriKind.Relative));
        static BitmapImage medal_bronze = new BitmapImage(new Uri(@"images/bronze.png", UriKind.Relative));

        Storyboard show_bl1_1st, show_bl1, show_bl2;
        BackgroundWorker load_bl1_1st = new BackgroundWorker();
        BackgroundWorker load_bl1 = new BackgroundWorker();
        BackgroundWorker load_bl2 = new BackgroundWorker();

        DInfoItemViewModel Block1_VM = new DInfoItemViewModel();
        DInfoItemViewModel Block2_VM = new DInfoItemViewModel();

        private delegate void UpdateInfo(string DType, int Level, string TName, int TLevel, ImageSource Image, ImageSource Medal);

        DMOWebInfo dmo_db;
        DMOFileSystem res_fs;

        public DigiRotation()
        {
            dmo_db = new DMOWebInfo(SettingsProvider.DMO_DB_PATH());
            dmo_db.DownloadCompleted += dmo_db_DownloadCompleted;

            res_fs = new DMOFileSystem(32, SettingsProvider.APP_PATH + SettingsProvider.RES_HF_FILE, SettingsProvider.APP_PATH + SettingsProvider.RES_PF_FILE);

            InitializeComponent();
            InitializeAnimation();
            DContext = new DigiRotation_DC();
            LayoutRoot.DataContext = DContext;
            Block1.DataContext = Block1_VM;
            Block2.DataContext = Block2_VM;
        }

        void dmo_db_DownloadCompleted(object sender, DMODownloadResultCode code, guild result)
        {
            if (code == DMODownloadResultCode.OK)
            {
                isLoading(false);
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {
                    load_bl1_1st.RunWorkerAsync();
                }));
            }
            else
            {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                {
                    ErrorMessage1.Text = LanguageProvider.strings.ROT_ERRMSG1 + " [" + code + "]";
                    ErrorMessage2.Text = LanguageProvider.strings.ROT_ERRMSG2;
                }));
            }
            isLoaded = true;
        }

        private void InitializeAnimation()
        {
            show_bl1_1st = ((Storyboard)this.FindResource("ShowBlock1_1st"));
            show_bl1 = ((Storyboard)this.FindResource("ShowBlock1"));
            show_bl2 = ((Storyboard)this.FindResource("ShowBlock2"));

            show_bl1_1st.Completed += (s, e) => { load_bl2.RunWorkerAsync(); };
            show_bl1.Completed += (s, e) => { load_bl2.RunWorkerAsync(); };
            show_bl2.Completed += (s, e) => { load_bl1.RunWorkerAsync(); };

            load_bl1_1st.DoWork += (s, e) => { UpdateDigiInfo(ref Block1, Block1_VM); };
            load_bl1_1st.RunWorkerCompleted += (s, e) => { show_bl1_1st.Begin(); };

            load_bl1.DoWork += (s, e) =>
            {
                System.Threading.Thread.Sleep(5000);
                UpdateDigiInfo(ref Block1, Block1_VM);
            };
            load_bl1.RunWorkerCompleted += (s, e) => { show_bl1.Begin(); };

            load_bl2.DoWork += (s, e) =>
            {
                System.Threading.Thread.Sleep(5000);
                UpdateDigiInfo(ref Block2, Block2_VM);
            };
            load_bl2.RunWorkerCompleted += (s, e) => { show_bl2.Begin(); };
        }

        public void InitializeRotation()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                isLoading(true);
                ROT_GUILD = SettingsProvider.ROTATION_GNAME;
                ROT_SERV = SettingsProvider.ROTATION_GSERV;
                dmo_db.GetGuild(ROT_GUILD, ROT_SERV, false, SettingsProvider.ROTATION_URATE);
            };
            bw.RunWorkerAsync();
        }

        #region Utils
        public void UpdateDigiInfo(ref Grid block, DInfoItemViewModel vmodel)
        {
            BitmapImage Medal = null;
            digimon d = dmo_db.GetRandomDigimon(ROT_SERV, ROT_GUILD, 70);

            if (d.Lvl >= 70 && d.Lvl < 75)
                Medal = medal_bronze;
            else if (d.Lvl >= 75 && d.Lvl < 80)
                Medal = medal_silver;
            else if (d.Lvl >= 80)
                Medal = medal_gold;

            block.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new UpdateInfo((DType_, Level_, TName_, TLevel_, Image_, Medal_) =>
            {
                vmodel.DType = DType_;
                vmodel.Level = Level_;
                vmodel.TName = string.Format("{0}: {1} (Lv. {2})", LanguageProvider.strings.ROT_TAMER, TName_, TLevel_);
                vmodel.TLevel = TLevel_;
                vmodel.Image = Image_;
                vmodel.Medal = Medal_;
            }), d.Name, d.Lvl, d.Custom_Tamer_Name, d.Custom_Tamer_lvl, GetDigimonImage(d.Type_id), Medal);
        }

        public BitmapImage GetDigimonImage(int digi_id)
        {
            Stream str = res_fs.ReadFile(string.Format(images_path, digi_id));
            if (str == null)
                return unk_digi;
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

        private void isLoading(bool state)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
            {
                HaguruGear1.IsEnabled = state;
                HaguruGear2.IsEnabled = state;
                Storyboard sb = new Storyboard();
                LoadingFrame.Visibility = Visibility.Visible;
                DoubleAnimation dbl_anim = new DoubleAnimation();
                dbl_anim.From = state ? 0 : 1;
                dbl_anim.To = state ? 1 : 0;
                dbl_anim.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                Storyboard.SetTarget(dbl_anim, LoadingFrame);
                Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
                sb.Children.Add(dbl_anim);
                sb.Completed += (s, e) => { LoadingFrame.Visibility = state ? Visibility.Visible : Visibility.Collapsed; };
                sb.Begin();
            }));
        }

        #endregion
    }
}
