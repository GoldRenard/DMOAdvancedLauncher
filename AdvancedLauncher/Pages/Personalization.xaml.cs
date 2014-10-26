﻿// ======================================================================
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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.Win32;

using AdvancedLauncher.Service;
using AdvancedLauncher.Environment;
using DMOLibrary.DMOFileSystem;

namespace AdvancedLauncher.Pages {
    public partial class Personalization : UserControl {
        byte[] current_image_bytes, selected_image_bytes;
        BitmapSource selected_image;
        Storyboard ShowWindow;
        ResourceViewModel Resource_DC = new ResourceViewModel();
        TargaImage ti = new TargaImage();

        //Microsoft.Win32.OpenFileDialog oFileDialog = new Microsoft.Win32.OpenFileDialog() { Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png" };
        OpenFileDialog oFileDialog = new OpenFileDialog() { Filter = "Targa Image (*.tga) | *.tga" };
        SaveFileDialog sFileDialog = new SaveFileDialog() { Filter = "Targa Image (*.tga) | *.tga" };

        const string RES_LIST_FILE = "\\ResourceList_{0}.cfg";
        bool isGameImageLoaded = false;

        public Personalization() {

            InitializeComponent();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
                LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
                LauncherEnv.Settings.ProfileChanged += ProfileChanged;
                ItemsComboBox.ItemsSource = Resource_DC.Items;
                this.Loaded += Personalization_Loaded;
                ProfileChanged();
            }
        }

        /// <summary>
        /// Как только контрол грузится, мы должны получить текущую картинку из игры
        /// Поэтому, если ресурсы загружены - принудительно вызываем функцию смены выбора комбобокса
        /// Загрузив тем самым текущее изображение
        /// </summary>
        /// <param name="sender">Объект-отправитель</param>
        /// <param name="e">Параметры события</param>
        void Personalization_Loaded(object sender, RoutedEventArgs e) {
            if (ItemsComboBox.Items.Count > 0)
                ItemsComboBox_SelectionChanged_1(ItemsComboBox, null);
        }

        /// <summary>
        /// Во время смены профиля нам нужно считать файл ресурсов и сбросить настройки
        /// </summary>
        void ProfileChanged() {
            LoadResourceList();
            ResetCurrent();
            ResetSelect();
        }

        /// <summary>
        /// Активация страницы. При активации нам необходимо проверить не загружено ли изображение.
        /// Если не загружено и загружен список ресурсов - загружаем изображение
        /// </summary>
        public void Activate() {
            if (!isGameImageLoaded && ItemsComboBox.Items.Count > 0) {
                if (ItemsComboBox.SelectedIndex == 0)
                    ItemsComboBox_SelectionChanged_1(ItemsComboBox, null);
                else
                    ItemsComboBox.SelectedIndex = 0;
            }
            ShowWindow.Begin();
        }

        /// <summary>
        /// Загрузка и парсинг файла с ресурсами. Синтакс:
        /// 1) DESCRIPTION;PATH
        /// 2) DESCRIPTION;ID
        /// </summary>
        private void LoadResourceList() {
            Resource_DC.UnLoadData();
            string[] rlines = null;
            string rFile = (LauncherEnv.GetResourcesPath() + string.Format(RES_LIST_FILE, LauncherEnv.Settings.pCurrent.DMOProfile.GetTypeName()));
            if (File.Exists(rFile)) {
                rlines = System.IO.File.ReadAllLines(rFile);

                for (int i = 0; i < rlines.Length; i++) {
                    if (rlines[i].Length == 0)
                        continue;
                    rlines[i] = rlines[i].Trim();
                    if (rlines[i][0] == '#')
                        continue;
                    string[] vars = rlines[i].Split(';');
                    if (vars.Length > 1) {
                        ResourceItemViewModel item = new ResourceItemViewModel();
                        item.RName = vars[0].ToUpper();
                        uint n = 0;
                        item.IsRID = uint.TryParse(vars[1], out n);
                        if (item.IsRID)
                            item.RID = n;
                        else
                            item.RPath = vars[1];
                        Resource_DC.AddData(item);
                    }
                }
            }
        }

        /// <summary>
        /// Выбор изображения для записи в игру
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Параметры события</param>
        private void Select_Picture_Click(object sender, RoutedEventArgs e) {
            var result = oFileDialog.ShowDialog(); //показываем диалог
            if (result == true) //Если результат положителен {
                ResetSelect();
                bool isSuccess = true;
                try {
                    selected_image_bytes = File.ReadAllBytes(oFileDialog.FileName); //считываем данные
                    selected_image = LoadTGA(selected_image_bytes);                 //и пытаемся открыть их как гта
                } catch { isSuccess = false; }

                if (isSuccess)                                                      //Если успешно открыли, скрываем строку помощи и показываем картинку {
                    SelecterHelp.Visibility = Visibility.Collapsed;
                    Selected_Image.Source = selected_image;

                    if (isGameImageLoaded)                                          //Если картинка из игры была загружена (что подтверждает доступность ресурсов игры)
                        BtnApply.IsEnabled = true;                                  //Разрешаем запись этой картинки в игру

                    return;
                }
                Utils.MSG_ERROR(LanguageEnv.Strings.PersonalizationWrongTGA);       //Иначе говорим, что это не ТГА-картинка.
            }
        }

        /// <summary>
        /// Сохранение текущего изображения
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Параметры события</param>
        private void SaveBtn_Click(object sender, RoutedEventArgs e) {
            if (isGameImageLoaded)  //Сохраняем только если картинка загружена {
                ResourceItemViewModel item = (ResourceItemViewModel)ItemsComboBox.SelectedValue;
                if (item.RID == 0)                                           //Если ID = 0, считаем, то у нас есть путь ресурса, откуда берем имя файла
                    sFileDialog.FileName = Path.GetFileName(item.RPath);
                else
                    sFileDialog.FileName = item.RID.ToString() + ".tga";    //Иначе сохраняем именем ID

                var result = sFileDialog.ShowDialog();
                if (result == true) {
                    try { File.WriteAllBytes(sFileDialog.FileName, current_image_bytes); } catch (Exception ex) {
                        Utils.MSG_ERROR(LanguageEnv.Strings.PersonalizationCantSave + " " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик выбора ресурса. Загружает текущее изображение
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Параметры события</param>
        private void ItemsComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e) {
            if (this.IsLoaded) {
                ResetCurrent();
                ResetSelect();
                isGameImageLoaded = LoadGameImage((ResourceItemViewModel)ItemsComboBox.SelectedValue);
            }
        }

        /// <summary>
        /// Загрузка текущего изображения из игры
        /// </summary>
        /// <param name="item">VM-объект с данными</param>
        /// <returns></returns>
        private bool LoadGameImage(ResourceItemViewModel item) {
            if (item == null)
                return false;

            DMOFileSystem dmoFS = LauncherEnv.Settings.pCurrent.GameEnv.GetFS();

            //Открываем файловую систему игры
            bool IsOpened = false;
            try { IsOpened = dmoFS.Open(FileAccess.Read, 16, LauncherEnv.Settings.pCurrent.GameEnv.GetHFPath(), LauncherEnv.Settings.pCurrent.GameEnv.GetPFPath()); } catch { IsOpened = false; }

            if (IsOpened) {
                Stream file = null;
                if (item.RID != 0)                      //Если есть ИД, считываем по нему
                    file = dmoFS.ReadFile(item.RID);
                else
                    file = dmoFS.ReadFile(item.RPath);  //Иначе считываем по пути ресурса
                if (file != null) {
                    isGameImageLoaded = true;
                    MemoryStream ms = new MemoryStream();
                    file.CopyTo(ms);
                    current_image_bytes = ms.ToArray();
                    Current_Image.Source = LoadTGA(current_image_bytes);
                    SaveBtn.Visibility = Visibility.Visible;
                    dmoFS.Close();
                    return true;
                }
                dmoFS.Close();
            } else
                MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            return false;
        }


        /// <summary>
        /// Применяет изменения в игру. Записывает выбранное изображение.
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Параметры события</param>
        private void BtnApply_Click(object sender, RoutedEventArgs e) {

            //Открываем файловую систему игры
            DMOFileSystem dmoFS = LauncherEnv.Settings.pCurrent.GameEnv.GetFS();
            bool IsOpened = false;
            try { IsOpened = dmoFS.Open(FileAccess.Write, 16, LauncherEnv.Settings.pCurrent.GameEnv.GetHFPath(), LauncherEnv.Settings.pCurrent.GameEnv.GetPFPath()); } catch { IsOpened = false; }

            if (IsOpened) {
                ResourceItemViewModel r_selected = (ResourceItemViewModel)ItemsComboBox.SelectedValue;
                bool wr_result = false;
                if (r_selected.IsRID)
                    wr_result = dmoFS.WriteStream(new MemoryStream(selected_image_bytes), r_selected.RID);
                else
                    wr_result = dmoFS.WriteStream(new MemoryStream(selected_image_bytes), r_selected.RPath);
                dmoFS.Close();

                if (!wr_result)
                    Utils.MSG_ERROR(LanguageEnv.Strings.PersonalizationCantWrite);
                else
                    isGameImageLoaded = LoadGameImage(r_selected);
            }

        }

        #region Utils

        private void ResetSelect() {
            SelecterHelp.Visibility = Visibility.Visible;
            Selected_Image.ClearValue(Image.SourceProperty);
            BtnApply.IsEnabled = false;
        }

        private void ResetCurrent() {
            SaveBtn.Visibility = Visibility.Collapsed;
            isGameImageLoaded = false;
            Current_Image.ClearValue(Image.SourceProperty);
        }

        private BitmapSource LoadTGA(string file) {
            System.Drawing.Bitmap bmp = TargaImage.LoadTargaImage(file);
            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
            bs.Freeze();
            return bs;
        }

        private BitmapSource LoadTGA(byte[] bytes) {
            System.Drawing.Bitmap bmp = TargaImage.LoadTargaImage(bytes);
            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
            bs.Freeze();
            return bs;
        }

        #endregion

    }

    public class ResourceItemViewModel : INotifyPropertyChanged {
        private string _RName;
        public string RName {
            get {
                return _RName;
            }
            set {
                if (value != _RName) {
                    _RName = value;
                    NotifyPropertyChanged("RName");
                }
            }
        }

        private uint _RID;
        public uint RID {
            get {
                return _RID;
            }
            set {
                if (value != _RID) {
                    _RID = value;
                    NotifyPropertyChanged("RID");
                }
            }
        }

        private bool _IsRID;
        public bool IsRID {
            get {
                return _IsRID;
            }
            set {
                if (value != _IsRID) {
                    _IsRID = value;
                    NotifyPropertyChanged("IsRID");
                }
            }
        }

        public ResourceItemViewModel Item {
            get {
                return this;
            }
            set { }
        }

        private string _RPath;
        public string RPath {
            get {
                return _RPath;
            }
            set {
                _RPath = value;
                NotifyPropertyChanged("RPath");
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

    public class ResourceViewModel : INotifyPropertyChanged {
        public ResourceViewModel() {
            this.Items = new ObservableCollection<ResourceItemViewModel>();
        }

        public ObservableCollection<ResourceItemViewModel> Items { get; private set; }

        public bool IsDataLoaded {
            get;
            private set;
        }

        public void LoadData(List<ResourceItemViewModel> List) {
            this.IsDataLoaded = true;
            foreach (ResourceItemViewModel item in List)
                this.Items.Add(item);
            NotifyPropertyChanged("Items");
        }

        public void AddData(ResourceItemViewModel item) {
            this.IsDataLoaded = true;
            this.Items.Add(item);
            NotifyPropertyChanged("Items");
        }

        public void UnLoadData() {
            this.IsDataLoaded = false;
            this.Items.Clear();
            NotifyPropertyChanged("Items");
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