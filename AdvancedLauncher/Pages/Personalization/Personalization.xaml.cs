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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.IO;
using System.ComponentModel;
using Ookii.Dialogs.Wpf;
using DMOLibrary.DMOFileSystem;

namespace AdvancedLauncher
{
    public partial class Personalization : UserControl
    {
        byte[] current_image_bytes, selected_image_bytes;
        BitmapSource selected_image;
        Storyboard ShowWindow;
        VistaOpenFileDialog op_file = new VistaOpenFileDialog();
        VistaSaveFileDialog sv_file = new VistaSaveFileDialog();
        Personalization_DC DContext = new Personalization_DC();
        ResourceViewModel Resource_DC = new ResourceViewModel();
        DMOFileSystem dmo_fs = null;
        TargaImage ti = new TargaImage();
        bool isGameImageLoaded = false;
        bool isFSInitialized = false;

        public Personalization()
        {
            op_file.FileName = "";
            op_file.Filter = sv_file.Filter = "TGA|*.tga";
            LoadResourceList();
            InitializeComponent();
            ItemsComboBox.ItemsSource = Resource_DC.Items;
            LayoutRoot.DataContext = DContext;

            ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
        }

        public void InitFS()
        {
            dmo_fs = new DMOFileSystem(16, App.DMOProfile.GetPackHFPath(), App.DMOProfile.GetPackPFPath());
            if (dmo_fs.ArchiveEntries.Count == 0) // if FS not opened
            {
                SelectBtn.IsEnabled = false;
                CurrentHelp.Text = LanguageProvider.strings.CUST_CANT_OPEN_FS;
            }
            else
            {
                SelectBtn.IsEnabled = true;
                CurrentHelp.Text = string.Empty;
                isFSInitialized = true;
            }

            if (Resource_DC.Items.Count > 0)
                LoadGameImage((ResourceItemViewModel)ItemsComboBox.SelectedValue);
            else
            {
                Utils.MSG_ERROR(string.Format(LanguageProvider.strings.CUST_RES_FILE_NOT_FOUND, string.Format(SettingsProvider.RES_LIST_FILE, App.DMOProfile.GetTypeName())));
                ItemsComboBox.IsEnabled = false;
            }
        }

        public bool Activate()
        {
            if (App.DMOProfile.CheckUpdateAccess())
            {
                if (!isFSInitialized)
                    InitFS();
                ShowWindow.Begin();
                return true;
            }
            MessageBox.Show(LanguageProvider.strings.GAME_FILES_IN_USE, LanguageProvider.strings.ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private void LoadResourceList()
        {
            string[] lines = null;
            if (File.Exists(SettingsProvider.APP_PATH + string.Format(SettingsProvider.RES_LIST_FILE, App.DMOProfile.GetTypeName())))
            {
                lines = System.IO.File.ReadAllLines(SettingsProvider.APP_PATH + string.Format(SettingsProvider.RES_LIST_FILE, App.DMOProfile.GetTypeName()));

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Length == 0)
                        continue;
                    lines[i] = lines[i].Trim();
                    if (lines[i][0] == '#')
                        continue;
                    string[] vars = lines[i].Split(';');
                    if (vars.Length > 1)
                    {
                        ResourceItemViewModel item = new ResourceItemViewModel();
                        item.RName = vars[0].ToUpper();
                        uint n = 0;
                        if (uint.TryParse(vars[1], out n))
                            item.RID = n;
                        else
                            item.RPath = vars[1];
                        Resource_DC.AddData(item);
                    }
                }
            }
        }

        private void Select_Picture_Click(object sender, RoutedEventArgs e)
        {
            op_file.Title = LanguageProvider.strings.CUST_SELECT_TITLE;
            var result =  op_file.ShowDialog();
            if (result == true)
            {
                ResetSelect();
                bool isSuccess = true;
                try
                {
                    selected_image_bytes = File.ReadAllBytes(op_file.FileName);
                    selected_image = LoadTGA(selected_image_bytes);
                }
                catch { isSuccess = false; }

                if (isSuccess)
                {
                    SelecterHelp.Visibility = Visibility.Collapsed;
                    Selected_Image.Source = selected_image;

                    if (isGameImageLoaded)
                        BtnApply.IsEnabled = true;

                    return;
                }
                Utils.MSG_ERROR(LanguageProvider.strings.CUST_WRONG_TGA);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isGameImageLoaded)
            {
                sv_file.Title = LanguageProvider.strings.CUST_SAVE_TITLE;

                ResourceItemViewModel item = (ResourceItemViewModel)ItemsComboBox.SelectedValue;
                if (item.RID == 0)
                    sv_file.FileName = Path.GetFileName(item.RPath);
                else
                    sv_file.FileName = item.RID.ToString() + ".tga";

                var result = sv_file.ShowDialog();
                if (result == true)
                {
                    try { File.WriteAllBytes(sv_file.FileName, current_image_bytes); }
                    catch (Exception ex)
                    {
                        Utils.MSG_ERROR(LanguageProvider.strings.CUST_CANT_SAVE + " " + ex.Message);
                    }
                }
            }
        }

        private void ItemsComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded) // cuz dmo_fs may still not loaded 
            {
                ResetCurrent();
                ResetSelect();
                isGameImageLoaded = LoadGameImage((ResourceItemViewModel)ItemsComboBox.SelectedValue);
            }
        }

        private bool LoadGameImage(ResourceItemViewModel item)
        {
            if (dmo_fs.ArchiveEntries.Count > 0)
            {
                Stream file = null;
                if (item.RID != 0)
                    file = dmo_fs.ReadFile(item.RID);
                else
                    file = dmo_fs.ReadFile(item.RPath);
                if (file != null)
                {
                    isGameImageLoaded = true;
                    MemoryStream ms = new MemoryStream();
                    file.CopyTo(ms);
                    current_image_bytes = ms.ToArray();
                    Current_Image.Source = LoadTGA(current_image_bytes);
                    SaveBtn.Visibility = Visibility.Visible;
                    return true;
                }
                else
                    Utils.MSG_ERROR(string.Format(LanguageProvider.strings.CUST_GAME_FILE_NOT_FOUND, ((item.RID != 0) ? item.RID.ToString() : item.RPath)));
            }
            return false;
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            if (App.DMOProfile.CheckUpdateAccess())
            {
                if (!dmo_fs.WriteStream(new MemoryStream(selected_image_bytes), ((ResourceItemViewModel)ItemsComboBox.SelectedValue).RPath))
                    Utils.MSG_ERROR(LanguageProvider.strings.CUST_CANT_WRITE);
                else
                    isGameImageLoaded = LoadGameImage((ResourceItemViewModel)ItemsComboBox.SelectedValue);
            }
        }

        #region Utils

        private void ResetSelect()
        {
            SelecterHelp.Visibility = Visibility.Visible;
            Selected_Image.ClearValue(Image.SourceProperty);
            BtnApply.IsEnabled = false;
        }

        private void ResetCurrent()
        {
            SaveBtn.Visibility = Visibility.Collapsed;
            isGameImageLoaded = false;
            Current_Image.ClearValue(Image.SourceProperty);
        }

        private BitmapSource LoadTGA(string file)
        {
            System.Drawing.Bitmap bmp = TargaImage.LoadTargaImage(file);
            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
            bs.Freeze();
            return bs;
        }

        private BitmapSource LoadTGA(byte[] bytes)
        {
            System.Drawing.Bitmap bmp = TargaImage.LoadTargaImage(bytes);
            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
            bs.Freeze();
            return bs;
        }

        #endregion
    }
}
