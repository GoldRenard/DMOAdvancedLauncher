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
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shell;
using AdvancedLauncher.Management;
using AdvancedLauncher.Management.Execution;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Management.Execution;
using AdvancedLauncher.SDK.Model;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.UI.Extension;
using AdvancedLauncher.UI.Windows;
using MahApps.Metro.Controls.Dialogs;
using Ninject;

namespace AdvancedLauncher.UI.Controls {

    public partial class GameControl : AbstractUserControl, IDisposable {
        private TaskEntry UpdateTask;
        private bool UpdateRequired = false;

        private TaskbarItemInfo TaskBar = new TaskbarItemInfo();
        private readonly BackgroundWorker CheckWorker = new BackgroundWorker();

        private Binding StartButtonBinding = new Binding("StartButton");
        private Binding WaitingButtonBinding = new Binding("GameButton_Waiting");
        private Binding UpdateButtonBinding = new Binding("GameButton_UpdateGame");

        [Inject]
        public ILauncherManager LauncherManager {
            get; set;
        }

        [Inject]
        public ITaskManager TaskManager {
            get; set;
        }

        [Inject]
        public IConfigurationManager ConfigurationManager {
            get; set;
        }

        [Inject]
        public IDialogManager DialogManager {
            get; set;
        }

        private IUpdateManager _UpdateManager;

        [Inject]
        public IUpdateManager UpdateManager {
            get {
                return _UpdateManager;
            }
            set {
                if (_UpdateManager == null) {
                    _UpdateManager = value;
                    _UpdateManager.FileSystemOpenError += UpdateManager_FileSystemOpenError;
                    _UpdateManager.StatusChanged += OnUpdateStatusChanged;
                }
            }
        }

        public delegate void SetProgressBar(double value, double maxvalue);

        public delegate void SetProgressBarVal(double value);

        public delegate void SetInfoText(string text);

        public GameControl() {
            InitializeComponent();
            UpdateTask = new TaskEntry() {
                Owner = this
            };
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                ElementHolder.RemoveChild(StartButton);
                ElementHolder.RemoveChild(UpdateBlock);
                WrapElement.Content = StartButton;
                Application.Current.MainWindow.TaskbarItemInfo = TaskBar;
                LanguageManager.LanguageChanged += OnLanguageChanged;
                App.Kernel.Get<LoginManager>().LoginCompleted += OnGameStartCompleted;
                ProfileManager.ProfileChanged += OnProfileChanged;
                CheckWorker.DoWork += CheckWorker_DoWork;
                OnProfileChanged(this, SDK.Model.Events.EventArgs.Empty);
            }
        }

        private void OnLanguageChanged(object sender, SDK.Model.Events.EventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SDK.Model.Events.EventHandler((s, e2) => {
                    OnLanguageChanged(sender, e2);
                }), sender, e);
                return;
            }
            this.DataContext = LanguageManager.Model;
        }

        private void OnProfileChanged(object sender, SDK.Model.Events.EventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SDK.Model.Events.EventHandler((s, e2) => {
                    OnProfileChanged(sender, e2);
                }), sender, e);
                return;
            }
            StartButton.IsEnabled = false;
            StartButton.SetBinding(Button.ContentProperty, WaitingButtonBinding);
            CheckWorker.RunWorkerAsync();
        }

        private void RemoveTask() {
            try {
                TaskManager.Tasks.TryTake(out UpdateTask);
            } catch (ArgumentOutOfRangeException) {
                // TODO Wtf this happening here?
            }
        }

        #region Update Section

        private async void CheckWorker_DoWork(object sender, DoWorkEventArgs e) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () {
                //Добавляем задачу обновления
                TaskManager.Tasks.Add(UpdateTask);
                ProfileManager.OnProfileLocked(true);
                EnvironmentManager.OnFileSystemLocked(true);
                EnvironmentManager.OnClosingLocked(true);
                UpdateRequired = false;
                StartButton.IsEnabled = false;
                StartButton.SetBinding(Button.ContentProperty, WaitingButtonBinding);
            }));

            GameModel model = ProfileManager.CurrentProfile.GameModel;

            //Проверяем наличие необходимых файлов игры
            if (!ConfigurationManager.CheckGame(model)) {
                SetStartEnabled(false); //Если необходимых файлов нет, просто вызываю этот метод. он при вышеуказанном условии покажет неактивную кнопку и сообщение о неправильном пути
                return;                      //Далее идти нет смысла
            }

            //Проверяем наличие обновления Pack01 файлами. Возвражающее значение говорит, можно ли проходить далее по алгоритму
            if (!await ImportPackages()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () {
                    RemoveTask();
                    ProfileManager.OnProfileLocked(false);
                    EnvironmentManager.OnFileSystemLocked(false);
                    EnvironmentManager.OnClosingLocked(false);
                }));
                return;
            }

            //Проверяем наличие новых обновлений
            VersionPair pair = UpdateManager.CheckUpdates(ProfileManager.CurrentProfile.GameModel);
            if (pair == null) {
                SetStartEnabled(false);
                DialogManager.ShowMessageDialog(LanguageManager.Model.ErrorOccured, LanguageManager.Model.ConnectionError);
                return;
            }
            //Если обновление требуется
            if (pair.IsUpdateRequired) {
                //Если включен интегрированных движок обновления, пытаемся обновиться
                if (ProfileManager.CurrentProfile.UpdateEngineEnabled) {
                    ShowProgressBar();
                    SetStartEnabled(await BeginUpdate(pair));
                } else { //Если интегрированный движок отключен - показываем кнопку "Обновить игру"
                    SetUpdateEnabled(true);
                }
            } else { //Если обновление не требуется, показываем кнопку "Начать игру".
                SetStartEnabled(true);
            }
        }

        private async Task<bool> ImportPackages() {
            GameModel model = ProfileManager.CurrentProfile.GameModel;
            if (Directory.Exists(ConfigurationManager.GetImportPath(model))) {
                if (ProfileManager.CurrentProfile.UpdateEngineEnabled) {
                    ShowProgressBar();
                    //Проверяем наличие доступа к игре
                    if (!await CheckGameAccessLoop()) {
                        return false;
                    }
                    if (!UpdateManager.ImportPackages(model)) {
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () {
                            DialogManager.ShowErrorDialog(LanguageManager.Model.GameFilesInUse);
                        }));
                        return false;
                    }
                } else {
                    //Интегрированный движок отключен, поэтому мы активируем кнопку обновления.
                    SetUpdateEnabled(true);
                    return false; //Далее по алгоритму идти не нужно, поэтому false
                }
            }
            return true;
        }

        private async Task<bool> BeginUpdate(VersionPair versionPair) {
            if (!await CheckGameAccessLoop()) {
                return false;
            }
            if (!UpdateManager.DownloadUpdates(ProfileManager.CurrentProfile.GameModel, versionPair)) {
                DialogManager.ShowMessageDialog(LanguageManager.Model.ErrorOccured, LanguageManager.Model.ConnectionError);
            }
            if (!await CheckGameAccessLoop()) {
                return false;
            }
            return await ImportPackages();
        }

        private async Task<bool> CheckGameAccessMessage() {
            MainWindow MainWindow = App.Kernel.Get<MainWindow>();
            return await MainWindow.Dispatcher.Invoke<Task<bool>>(new Func<Task<bool>>(async () => {
                MessageDialogResult result = await MainWindow.ShowMessageAsync(LanguageManager.Model.PleaseCloseGame, LanguageManager.Model.GameFilesInUse,
                    MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() {
                        AffirmativeButtonText = "OK",
                        NegativeButtonText = LanguageManager.Model.CancelButton,
                        ColorScheme = MetroDialogColorScheme.Accented
                    });
                return result == MessageDialogResult.Negative;
            }));
        }

        private async Task<bool> CheckGameAccessLoop() {
            //Проверяем наличие доступа к игре
            while (!ConfigurationManager.CheckUpdateAccess(ProfileManager.CurrentProfile.GameModel)) {
                if (await CheckGameAccessMessage()) {
                    return false;
                }
            }
            return true;
        }

        private async void UpdateManager_FileSystemOpenError(object sender, SDK.Model.Events.EventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SDK.Model.Events.EventHandler((s, e2) => {
                    UpdateManager_FileSystemOpenError(sender, e2);
                }), sender, e);
                return;
            }
            await CheckGameAccessMessage();
            SetUpdateEnabled(false);
        }

        private void OnUpdateStatusChanged(object sender, UpdateStatusEventEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new UpdateStatusEventHandler((s, e2) => {
                    OnUpdateStatusChanged(sender, e2);
                }), sender, e);
                return;
            }
            UpdateMainProgressBar(e.Progress, e.MaxProgress);
            UpdateSubProgressBar(e.SummaryProgress, e.SummaryMaxProgress);

            string updateText = string.Empty;
            switch (e.UpdateStage) {
                case UpdateStatusEventEventArgs.Stage.DOWNLOADING:
                    updateText = string.Format(LanguageManager.Model.UpdateDownloading, e.CurrentPatch, e.MaxPatch, e.SummaryProgress, e.SummaryMaxProgress);
                    break;

                case UpdateStatusEventEventArgs.Stage.EXTRACTING:
                    updateText = string.Format(LanguageManager.Model.UpdateExtracting, e.CurrentPatch, e.MaxPatch, e.SummaryProgress, e.SummaryMaxProgress);
                    break;

                case UpdateStatusEventEventArgs.Stage.INSTALLING:
                    updateText = string.Format(LanguageManager.Model.UpdateInstalling, e.Progress, e.MaxProgress);
                    break;
            }
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetInfoText((text_) => {
                UpdateText.Text = text_;
            }), updateText);
        }

        #endregion Update Section

        #region Game Start/Login Section

        private void StartGame(string args) {
            StartButton.IsEnabled = false;

            Profile currentProfile = ProfileManager.CurrentProfile;
            GameModel model = currentProfile.GameModel;
            IConfiguration configuration = ConfigurationManager.GetConfiguration(model);
            ILauncher launcher = LauncherManager.GetLauncher(currentProfile);
            ConfigurationManager.UpdateRegistryPaths(model);

            bool executed = launcher.Execute(
                UpdateRequired ? ConfigurationManager.GetLauncherEXE(model) : ConfigurationManager.GetGameEXE(model),
                UpdateRequired ? configuration.ConvertLauncherStartArgs(args) : configuration.ConvertGameStartArgs(args));

            if (executed) {
                StartButton.SetBinding(Button.ContentProperty, WaitingButtonBinding);
                if (currentProfile.KBLCServiceEnabled) {
                    launcher = LauncherManager.findByType<DirectLauncher>(typeof(DirectLauncher));
                    launcher.Execute(EnvironmentManager.KBLCFile, "-attach -notray");
                }
                TaskManager.CloseApp();
            } else {
                ProfileManager.OnProfileLocked(false);
                EnvironmentManager.OnFileSystemLocked(false);
                StartButton.IsEnabled = true;
            }
        }

        private void OnGameStartCompleted(object sender, LoginCompleteEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new LoginCompleteEventHandler((s, e2) => {
                    OnGameStartCompleted(sender, e2);
                }), sender, e);
                return;
            }
            //Если результат НЕУСПЕШЕН, возвращаем кнопку старта и возможность смены профиля
            if (e.Code != LoginCode.SUCCESS) {
                StartButton.IsEnabled = true;
                if (UpdateRequired) {
                    StartButton.SetBinding(Button.ContentProperty, UpdateButtonBinding);
                } else {
                    StartButton.SetBinding(Button.ContentProperty, StartButtonBinding);
                }
                ProfileManager.OnProfileLocked(false);
            }

            //Получаем результат логина
            switch (e.Code) {
                case LoginCode.SUCCESS:
                    {
                        //Если логин успешен, сохраняем аргументы текущей сессии вместе с настройками и запускаем игру
                        App.Kernel.Get<LoginManager>().UpdateLastSessionArgs(ProfileManager.CurrentProfile, e.Arguments);
                        EnvironmentManager.Save();
                        StartGame(e.Arguments);
                        break;
                    }
                case LoginCode.WRONG_PAGE:       //Если получены результаты ошибки на странице, отображаем сообщение с кодом ошибки
                case LoginCode.UNKNOWN_URL:
                    {
                        //И возвращаем в форму ввода
                        DialogManager.ShowMessageDialog(LanguageManager.Model.LoginLogIn,
                                    LanguageManager.Model.LoginWrongPage + string.Format(" [{0}]", e.Code));
                        break;
                    }
            }
        }

        #endregion Game Start/Login Section

        #region Interface Section

        #region Start Button

        private void SetStartEnabled(bool IsEnabled) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () {
                //Убираем задачу обновления
                RemoveTask();
                TaskBar.ProgressState = TaskbarItemProgressState.None;
                ProfileManager.OnProfileLocked(false);
                EnvironmentManager.OnClosingLocked(false);
                EnvironmentManager.OnFileSystemLocked(false);
                UpdateRequired = false;
                WrapElement.Content = StartButton;
                StartButton.SetBinding(Button.ContentProperty, StartButtonBinding);
                StartButton.IsEnabled = false;
                //Проверяем наличие необходимых файлов стандартного лаунчера. Если нету - просто показываем неактивную кнопку "Обновить игру" и сообщение об ошибке.
                if (!ConfigurationManager.CheckGame(ProfileManager.CurrentProfile.GameModel)) {
                    DialogManager.ShowErrorDialog(LanguageManager.Model.PleaseSelectGamePath);
                    return;
                }
                StartButton.IsEnabled = IsEnabled;
            }));
        }

        private void SetUpdateEnabled(bool IsEnabled) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () {
                //Убираем задачу обновления
                RemoveTask();
                TaskBar.ProgressState = TaskbarItemProgressState.None;
                ProfileManager.OnProfileLocked(false);
                EnvironmentManager.OnFileSystemLocked(false);
                EnvironmentManager.OnClosingLocked(false);
                UpdateRequired = true;
                WrapElement.Content = StartButton;
                StartButton.SetBinding(Button.ContentProperty, UpdateButtonBinding);
                StartButton.IsEnabled = false;
                //Проверяем наличие необходимых файлов стандартного лаунчера. Если нету - просто показываем неактивную кнопку "Обновить игру" и сообщение об ошибке.
                if (!ConfigurationManager.CheckLauncher(ProfileManager.CurrentProfile.GameModel)) {
                    DialogManager.ShowErrorDialog(LanguageManager.Model.PleaseSelectLauncherPath);
                    return;
                }
                StartButton.IsEnabled = IsEnabled;
            }));
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e) {
            ProfileManager.OnProfileLocked(true);
            EnvironmentManager.OnFileSystemLocked(true);
            //Проверяем, требуется ли логин
            if (ConfigurationManager.GetConfiguration(ProfileManager.CurrentProfile.GameModel).IsLoginRequired) {
                App.Kernel.Get<LoginManager>().Login(ProfileManager.CurrentProfile);
            } else { //Логин не требуется, запускаем игру как есть
                StartGame(string.Empty);
            }
        }

        #endregion Start Button

        #region ProgressBar

        private void ShowProgressBar() {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () {
                WrapElement.Content = UpdateBlock;
                UpdateText.Text = string.Empty;
                UpdateMainProgressBar(0, 100);
                UpdateSubProgressBar(0, 100);
                TaskBar.ProgressState = TaskbarItemProgressState.Normal;
            }));
        }

        private void UpdateMainProgressBar(double value, double maxvalue) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBar((value_, maxvalue_) => {
                MainProgressBar.Maximum = maxvalue_;
                MainProgressBar.Value = value_;
                TaskBar.ProgressValue = MainProgressBar.Value / MainProgressBar.Maximum;
            }), value, maxvalue);
        }

        private void UpdateSubProgressBar(double value, double maxvalue) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBar((value_, maxvalue_) => {
                SubProgressBar.Maximum = maxvalue_;
                SubProgressBar.Value = value_;
            }), value, maxvalue);
        }

        #endregion ProgressBar

        #endregion Interface Section

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose) {
            if (dispose) {
                CheckWorker.Dispose();
            }
        }
    }
}