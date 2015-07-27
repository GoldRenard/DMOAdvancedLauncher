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
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AdvancedLauncher.Management;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Tools;
using DMOLibrary.Database;
using DMOLibrary.Database.Context;
using DMOLibrary.Database.Entity;
using DMOLibrary.Events;
using DMOLibrary.Profiles;
using MahApps.Metro.Controls;
using Ninject;

namespace AdvancedLauncher.UI.Controls {

    public partial class DigiRotation : TransitioningContentControl, IDisposable {
        private static int MIN_LVL = 41;

        private static int ROTATION_INTERVAL = 5000;
        private const string TAMER_NAME_FORMAT = "{0}: {1} (Lv. {2})";

        private string GuildName;
        private string TamerName;
        private Server Server;
        private Guild Guild;

        private bool IsSourceLoaded = false;
        private bool IsErrorOccured = false;
        private bool IsStatic = false;

        private static Binding LoadingTitleBinding = new Binding("RotationDownloading");
        private static Binding LoadingSummaryBinding = new Binding("PleaseWait");

        private static Brush medalGold = new SolidColorBrush(Color.FromRgb(230, 230, 0));
        private static Brush medalSilver = new SolidColorBrush(Color.FromRgb(180, 180, 180));
        private static Brush medalBronze = new SolidColorBrush(Color.FromRgb(250, 180, 110));

        private TaskManager.Task LoadingTask;
        private readonly BackgroundWorker MainWorker = new BackgroundWorker();
        private AbstractWebProfile WebProfile = null;
        private RotationElement tempRotationElement = null;

        private HaguruLoader loader = new HaguruLoader() {
            IsEnabled = false
        };

        private delegate void UpdateInfo(string dType, int lvl, string tamerName, int tamerLevel, ImageSource image, Brush medal);

        private List<DigiImage> ImagesCollection = new List<DigiImage>();

        private struct DigiImage {
            public int Id;
            public BitmapImage Image;
        }

        [Inject]
        public ILanguageManager LanguageManager {
            get; set;
        }

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get; set;
        }

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        public DigiRotation() {
            App.Kernel.Inject(this);
            InitializeComponent();
            LoadingTask = new TaskManager.Task() {
                Owner = this
            };
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                LanguageManager.LanguageChanged += (s, e) => {
                    this.DataContext = LanguageManager.Model;
                };
                ProfileManager.ProfileChanged += (s, e) => {
                    IsSourceLoaded = false;
                };

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
                    IsLoadingAnim(true, true);
                    IsStatic = false;
                    IsErrorOccured = false;
                    //Получаем информацию, необходимую для ротации
                    GuildName = ProfileManager.CurrentProfile.Rotation.Guild;
                    TamerName = ProfileManager.CurrentProfile.Rotation.Tamer;

                    //Проверяем, доступен ли веб-профиль и необходимая информация
                    WebProfile = GameManager.CurrentProfile.GetWebProfile();
                    IsStatic = WebProfile == null || string.IsNullOrEmpty(GuildName);
                    if (!IsStatic) {
                        Server = GameManager.CurrentProfile.GetServerById(ProfileManager.CurrentProfile.Rotation.ServerId + 1);
                        //Регистрируем ивенты загрузки
                        WebProfile.StatusChanged += OnStatusChange;
                        WebProfile.DownloadCompleted += OnDownloadComplete;
                        //Получаем информацию о списках гильдии
                        AbstractWebProfile.GetActualGuild(WebProfile, Server, GuildName, false, ProfileManager.CurrentProfile.Rotation.UpdateInterval + 1);
                        //Убираем обработку ивентов
                        WebProfile.DownloadCompleted -= OnDownloadComplete;
                        WebProfile.StatusChanged -= OnStatusChange;
                    }
                    //Проверяем не произошла ли ошибка
                    if (!IsErrorOccured) {
                        //Закрываем анимацию, устанавливаем флаг загрузки
                        IsLoadingAnim(false);
                    }
                    TaskManager.Tasks.TryTake(out LoadingTask);
                    IsSourceLoaded = true;
                }

                if (!IsErrorOccured && IsSourceLoaded) {
                    UpdateModel();
                    System.Threading.Thread.Sleep(ROTATION_INTERVAL);
                }
            }
        }

        private void OnStatusChange(object sender, DownloadStatusEventArgs e) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => {
                loader.Maximum = e.MaxProgress;
                loader.Value = e.Progress;
            }));
        }

        private void OnDownloadComplete(object sender, DownloadCompleteEventArgs e) {
            if (e.Code != DMODownloadResultCode.OK) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => {
                    loader.Title = LanguageManager.Model.ErrorOccured + " [" + e.Code + "]";
                    switch (e.Code) {
                        case DMODownloadResultCode.CANT_GET:
                            {
                                loader.Summary = LanguageManager.Model.CantGetError;
                                break;
                            }
                        case DMODownloadResultCode.NOT_FOUND:
                            {
                                loader.Summary = LanguageManager.Model.GuildNotFoundError;
                                break;
                            }
                        case DMODownloadResultCode.WEB_ACCESS_ERROR:
                            {
                                loader.Summary = LanguageManager.Model.ConnectionError;
                                break;
                            }
                    }
                    IsErrorOccured = true;
                }));
                return;
            }
            Guild = MergeHelper.Merge(e.Guild);
        }

        #region Utils

        private void UpdateModel() {
            IsLoadingAnim(true);
            using (MainContext context = new MainContext()) {
                if (!IsStatic && Guild != null) {
                    //Если не статическое, получаем рандомного дигимона из базы данных
                    Brush MedalColor = null;
                    Digimon d = null;

                    Tamer tamer = null;
                    if (!string.IsNullOrEmpty(TamerName.Trim())) {
                        tamer = context.FindTamerByGuildAndName(Guild, TamerName.Trim());
                    }
                    if (tamer != null) {
                        d = context.FindRandomDigimon(tamer, MIN_LVL);
                    }
                    if (d == null) {
                        d = context.FindRandomDigimon(Guild, MIN_LVL);
                    }

                    if (d == null) {
                        // если у нас нет вообще такого дигимона, тогда переключаемся на статику
                        IsStatic = true;
                        UpdateModel();
                        return;
                    }

                    //Устанавливаем медали в зависимости от уровня
                    if (d.Level < 75) {
                        MedalColor = medalBronze;
                    } else if (d.Level >= 75 && d.Level < 80) {
                        MedalColor = medalSilver;
                    } else if (d.Level >= 80) {
                        MedalColor = medalGold;
                    }

                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new UpdateInfo((DType_, Level_, TName_, TLevel_, Image_, Medal_) => {
                            RotationElement vmodel = null;
                            if (this.Content != null && !this.Content.GetType().IsAssignableFrom(typeof(HaguruLoader))) {
                                vmodel = tempRotationElement;
                                tempRotationElement = (RotationElement)this.Content;
                            }
                            if (vmodel == null) {
                                vmodel = new RotationElement();
                            }
                            vmodel.DType = DType_;
                            vmodel.Level = Level_;
                            vmodel.TName = string.Format(TAMER_NAME_FORMAT, LanguageManager.Model.RotationTamer, TName_, TLevel_);
                            vmodel.TLevel = TLevel_;
                            vmodel.Image = Image_;
                            vmodel.Medal = Medal_;
                            vmodel.ShowInfo = true;
                            this.Content = vmodel;
                        }), d.Name, d.Level, d.Tamer.Name, d.Tamer.Level, GetDigimonImage(d.Type.Code), MedalColor);
                } else {
                    DigimonType dType = context.FindRandomDigimonType();
                    this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new UpdateInfo((DType_, Level_, TName_, TLevel_, Image_, Medal_) => {
                        RotationElement vmodel = null;
                        if (this.Content != null && !this.Content.GetType().IsAssignableFrom(typeof(HaguruLoader))) {
                            vmodel = tempRotationElement;
                            tempRotationElement = (RotationElement)this.Content;
                        }
                        if (vmodel == null) {
                            vmodel = new RotationElement();
                        }
                        vmodel.Image = Image_;
                        vmodel.ShowInfo = false;
                        this.Content = vmodel;
                    }), string.Empty, 0, string.Empty, 0, GetDigimonImage(dType.Code), null);
                }
            }
            IsLoadingAnim(false);
        }

        private const string DIGIROTATION_DIR = "DigiRotation";
        private const string PNG_FORMAT = "{0}.png";

        public BitmapImage GetDigimonImage(int digimonId) {
            DigiImage image = ImagesCollection.Find(i => i.Id == digimonId);
            if (image.Image != null) {
                return image.Image;
            }

            string resource = EnvironmentManager.ResolveResource(DIGIROTATION_DIR,
                string.Format(PNG_FORMAT, digimonId),
                string.Format(URLUtils.DIGIROTATION_IMAGE_REMOTE_FORMAT, digimonId));

            if (resource != null) {
                Stream str = File.OpenRead(resource);
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

        private void IsLoadingAnim(bool state, bool transition = false) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () {
                loader.IsEnabled = state;
                if (state) {
                    loader.SetBinding(HaguruLoader.TitleProperty, LoadingTitleBinding);
                    loader.SetBinding(HaguruLoader.SummaryProperty, LoadingSummaryBinding);
                    loader.Value = 0;
                    if (transition) {
                        this.Content = loader;
                    }
                }
            }));
        }

        #endregion Utils

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose) {
            if (dispose) {
                MainWorker.Dispose();
            }
        }
    }
}