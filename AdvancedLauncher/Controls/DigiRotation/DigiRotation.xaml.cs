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
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Service;
using DMOLibrary;
using DMOLibrary.Database;
using DMOLibrary.Database.Context;
using DMOLibrary.Database.Entity;
using DMOLibrary.Profiles;

namespace AdvancedLauncher.Controls {

    public partial class DigiRotation : UserControl {
        private static int ROTATION_INTERVAL = 5000;
        private TaskManager.Task LoadingTask;
        private const string TNameFormat = "{0}: {1} (Lv. {2})";

        private string rGuild;
        private string rTamer;
        private Server rServ;
        private Guild rGuildEntity;

        private bool IsSourceLoaded = false;
        private bool IsErrorOccured = false;

        private bool _IsLoading = false;

        public bool IsLoading {
            get {
                return _IsLoading;
            }
        }

        private bool IsStatic = false;       //Используется для указания ротации без информации о дигимоне и теймере (просто ротация картинок)

        private static BitmapImage unknownDigimon = new BitmapImage(new Uri(@"images/unknown.png", UriKind.Relative));
        private static BitmapImage medalGold = new BitmapImage(new Uri(@"images/gold.png", UriKind.Relative));
        private static BitmapImage medalSilver = new BitmapImage(new Uri(@"images/silver.png", UriKind.Relative));
        private static BitmapImage medalBronze = new BitmapImage(new Uri(@"images/bronze.png", UriKind.Relative));

        private Storyboard sbb1First, sbb1, sbb2;   //Storyboards показа блоков

        private BackgroundWorker MainWorker = new BackgroundWorker();

        private DInfoItemViewModel Block1Model = new DInfoItemViewModel();
        private DInfoItemViewModel Block2Model = new DInfoItemViewModel();

        private delegate void UpdateInfo(string dType, int lvl, string tamerName, int tamerLevel, ImageSource image, ImageSource medal);

        private AbstractWebProfile WebProfile = null;

        //Данная структура и список используются для хранения и использования уже загруженных изображений и предотвращения их повторной загрузки
        private struct DigiImage {
            public int Id;
            public BitmapImage Image;
        }

        private List<DigiImage> ImagesCollection = new List<DigiImage>();

        public DigiRotation() {
            InitializeComponent();
            LoadingTask = new TaskManager.Task() {
                Owner = this
            };
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                LanguageEnv.Languagechanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
                LauncherEnv.Settings.ProfileChanged += delegate() {
                    IsSourceLoaded = false;
                };

                sbb1First = ((Storyboard)this.FindResource("ShowBlock1_1st"));
                sbb1 = ((Storyboard)this.FindResource("ShowBlock1"));
                sbb2 = ((Storyboard)this.FindResource("ShowBlock2"));

                Block1.DataContext = Block1Model;
                Block2.DataContext = Block2Model;

                MainWorker.DoWork += MainWorkerFunc;
                MainWorker.RunWorkerAsync();
            }
        }

        private void MainWorkerFunc(object sender, DoWorkEventArgs e) {
            //Ротация в цикле
            while (true) {
                //Если источник не загружен
                if (!IsSourceLoaded) {
                    //Добавляем задачу загрузки
                    TaskManager.Tasks.Add(LoadingTask);
                    //Показываем анимацию загрузки
                    IsLoadingAnim(true);

                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                        //Останавливаем все сторибоарды
                        sbb1First.Stop();
                        sbb1.Stop();
                        sbb2.Stop();

                        //Останавливаем все свойства в начальные
                        LoadingPB.Value = 0;
                        IsStatic = false;
                        IsErrorOccured = false;
                        Block1.Opacity = 0;
                        Block2.Opacity = 0;
                        BlockPanel1.Visibility = Visibility.Visible;
                        BlockPanel2.Visibility = Visibility.Visible;

                        //Получаем информацию, необходимую для ротации
                        rGuild = LauncherEnv.Settings.CurrentProfile.Rotation.Guild;
                        rTamer = LauncherEnv.Settings.CurrentProfile.Rotation.Tamer;
                    }));

                    WebProfile = LauncherEnv.Settings.CurrentProfile.DMOProfile.GetWebProfile();

                    //Проверяем, доступен ли веб-профиль и необходимая информация
                    if (WebProfile != null && !string.IsNullOrEmpty(rGuild)) {
                        rServ = LauncherEnv.Settings.CurrentProfile.DMOProfile.GetServerById(LauncherEnv.Settings.CurrentProfile.Rotation.ServerId + 1);
                        //Регистрируем ивенты загрузки
                        WebProfile.StatusChanged += OnStatusChange;
                        WebProfile.DownloadCompleted += OnDownloadComplete;
                        //Получаем информацию о списках гильдии
                        AbstractWebProfile.GetActualGuild(WebProfile, rServ, rGuild, false, LauncherEnv.Settings.CurrentProfile.Rotation.UpdateInterval + 1);
                        //Убираем обработку ивентов
                        WebProfile.DownloadCompleted -= OnDownloadComplete;
                        WebProfile.StatusChanged -= OnStatusChange;
                    } else {
                        //Блоки с инфой нам не нужны, скрываем
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                            BlockPanel1.Visibility = Visibility.Collapsed;
                            BlockPanel2.Visibility = Visibility.Collapsed;
                        }));
                        //Помечаем как статическую
                        IsStatic = true;
                    }
                    //Проверяем не произошла ли ошибка
                    if (!IsErrorOccured) {
                        //Закрываем анимацию, устанавливаем флаг загрузки
                        IsLoadingAnim(false);
                        IsSourceLoaded = true;

                        //Получаем информацию и дигимоне и показываем его в блоке
                        UpdateDigiInfo(ref Block1, Block1Model);
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                            sbb1First.Begin();
                        }));
                        TaskManager.Tasks.Remove(LoadingTask);
                        System.Threading.Thread.Sleep(ROTATION_INTERVAL);
                    } else {
                        _IsLoading = false;
                        TaskManager.Tasks.Remove(LoadingTask);
                        IsSourceLoaded = true;
                    }
                    //Убираем задачу загрузки
                }

                if (!IsErrorOccured) {
                    if (IsSourceLoaded) {
                        UpdateDigiInfo(ref Block2, Block2Model);
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                            sbb2.Begin();
                        }));
                        System.Threading.Thread.Sleep(ROTATION_INTERVAL);

                        UpdateDigiInfo(ref Block1, Block1Model);
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                            sbb1.Begin();
                        }));
                        System.Threading.Thread.Sleep(ROTATION_INTERVAL);
                    }
                } else {
                }
            }
        }

        private void OnStatusChange(object sender, DownloadStatus status) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => {
                LoadingPB.Maximum = status.MaxProgress;
                LoadingPB.Value = status.Progress;
            }));
        }

        private void OnDownloadComplete(object sender, DMODownloadResultCode code, Guild result) {
            if (code != DMODownloadResultCode.OK) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => {
                    ErrorMessage1.Text = LanguageEnv.Strings.ErrorOccured + " [" + code + "]";
                    switch (code) {
                        case DMODownloadResultCode.CANT_GET: {
                                ErrorMessage2.Text = LanguageEnv.Strings.CantGetError;
                                break;
                            }
                        case DMODownloadResultCode.DB_CONNECT_ERROR: {
                                ErrorMessage2.Text = LanguageEnv.Strings.DBConnectionError;
                                break;
                            }
                        case DMODownloadResultCode.NOT_FOUND: {
                                ErrorMessage2.Text = LanguageEnv.Strings.GuildNotFoundError;
                                break;
                            }
                        case DMODownloadResultCode.WEB_ACCESS_ERROR: {
                                ErrorMessage2.Text = LanguageEnv.Strings.ConnectionError;
                                break;
                            }
                    }
                    LoadingBlock.Visibility = Visibility.Hidden;
                    ErrorBlock.Visibility = Visibility.Visible;
                    IsErrorOccured = true;
                }));
                return;
            }
            MergeHelper.Merge(result);
            rGuildEntity = result;
        }

        #region Utils

        public void UpdateDigiInfo(ref Grid block, DInfoItemViewModel vmodel) {
            if (!IsStatic && rGuildEntity != null) {
                //Если не статическое, получаем рандомного дигимона из базы данных
                BitmapImage Medal = null;
                Digimon d = null;

                Tamer tamer = null;
                if (!string.IsNullOrEmpty(rTamer.Trim())) {
                    tamer = MainContext.Instance.FindTamerByGuildAndName(rGuildEntity, rTamer.Trim());
                }

                if (tamer != null) {
                    d = MainContext.Instance.FindRandomDigimon(tamer, 70);
                }
                if (d == null) {
                    d = MainContext.Instance.FindRandomDigimon(rGuildEntity, 70);
                }

                //Устанавливаем медали в зависимости от уровня
                if (d.Level >= 70 && d.Level < 75) {
                    Medal = medalBronze;
                } else if (d.Level >= 75 && d.Level < 80) {
                    Medal = medalSilver;
                } else if (d.Level >= 80) {
                    Medal = medalGold;
                }

                block.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new UpdateInfo((DType_, Level_, TName_, TLevel_, Image_, Medal_) => {
                    vmodel.DType = DType_;
                    vmodel.Level = Level_;
                    vmodel.TName = string.Format(TNameFormat, LanguageEnv.Strings.RotationTamer, TName_, TLevel_);
                    vmodel.TLevel = TLevel_;
                    vmodel.Image = Image_;
                    vmodel.Medal = Medal_;
                }), d.Name, d.Level, d.Tamer.Name, d.Tamer.Level, GetDigimonImage(d.Type.Code), Medal);
            } else {
                //Если статика - получаем рандомный тип и показываем
                DigimonType dType = MainContext.Instance.FindRandomDigimonType();
                block.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new UpdateInfo((DType_, Level_, TName_, TLevel_, Image_, Medal_) => {
                    vmodel.DType = DType_;
                    vmodel.Level = Level_;
                    vmodel.TName = string.Format(TNameFormat, LanguageEnv.Strings.RotationTamer, TName_, TLevel_);
                    vmodel.TLevel = TLevel_;
                    vmodel.Image = Image_;
                    vmodel.Medal = Medal_;
                }), string.Empty, 0, string.Empty, 0, GetDigimonImage(dType.Code), null);
            }
        }

        public BitmapImage GetDigimonImage(int digi_id) {
            DigiImage image = ImagesCollection.Find(i => i.Id == digi_id);
            if (image.Image != null) {
                return image.Image;
            }

            string ImageFile = string.Format("{0}\\DigiRotation\\{1}.png", LauncherEnv.GetResourcesPath(), digi_id);

            //If we don't have image, try to download it
            if (!File.Exists(ImageFile)) {
                try {
                    LauncherEnv.WebClient.DownloadFile(string.Format("{0}DigiRotation/{1}.png", LauncherEnv.REMOTE_PATH, digi_id), ImageFile);
                } catch {
                }
            }

            if (File.Exists(ImageFile)) {
                Stream str = File.OpenRead(ImageFile);
                if (str == null) {
                    return unknownDigimon;
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
                    Id = digi_id
                });
                return bitmap;
            }
            return unknownDigimon;
        }

        #region Loading Animation

        private Storyboard sbAnimShow = new Storyboard();
        private Storyboard sbAnimHide = new Storyboard();
        private bool IsAnimInitialized = false;

        private void InitializeAnim() {
            if (IsAnimInitialized) {
                return;
            }

            DoubleAnimation dbl_anim_show = new DoubleAnimation();
            dbl_anim_show.To = 1;
            dbl_anim_show.Duration = new Duration(TimeSpan.FromMilliseconds(300));

            DoubleAnimation dbl_anim_hide = new DoubleAnimation();
            dbl_anim_hide.To = 0;
            dbl_anim_hide.Duration = new Duration(TimeSpan.FromMilliseconds(300));

            Storyboard.SetTarget(dbl_anim_show, LoadingFrame);
            Storyboard.SetTarget(dbl_anim_hide, LoadingFrame);
            Storyboard.SetTargetProperty(dbl_anim_show, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetProperty(dbl_anim_hide, new PropertyPath(OpacityProperty));

            sbAnimShow.Children.Add(dbl_anim_show);
            sbAnimHide.Children.Add(dbl_anim_hide);

            sbAnimHide.Completed += (s, e) => {
                LoadingFrame.Visibility = Visibility.Collapsed;
            };

            IsAnimInitialized = true;
        }

        private void IsLoadingAnim(bool state) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                _IsLoading = state;
                HaguruGear1.IsEnabled = state;
                HaguruGear2.IsEnabled = state;
                InitializeAnim();
                LoadingFrame.Visibility = Visibility.Visible;
                if (_IsLoading) {
                    LoadingBlock.Visibility = Visibility.Visible;
                    ErrorBlock.Visibility = Visibility.Collapsed;
                    sbAnimShow.Begin();
                } else {
                    sbAnimHide.Begin();
                }
            }));
        }

        #endregion Loading Animation

        #endregion Utils
    }
}