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
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shell;
using AdvancedLauncher.Controls.Dialogs;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Service;
using DMOLibrary.DMOFileSystem;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace AdvancedLauncher.Controls {

    public partial class GameControl : UserControl {
        private TaskManager.Task UpdateTask;
        private bool UpdateRequired = false;

        private TaskbarItemInfo TaskBar = new TaskbarItemInfo();
        private WebClient webClient = new WebClient();
        private DMOFileSystem GameFS = null;
        private BackgroundWorker CheckWorker = new BackgroundWorker();

        private Binding StartButtonBinding = new Binding("StartButton");
        private Binding WaitingButtonBinding = new Binding("GameButton_Waiting");
        private Binding UpdateButtonBinding = new Binding("GameButton_UpdateGame");

        private double dataReceived, dataTotal;
        private int verCurrent = -1;
        private int verRemote = -1;

        private class CheckResult {
            public int LocalVer;
            public int RemoteVer;

            public bool IsUpdateRequired {
                get {
                    return RemoteVer > LocalVer;
                }
            }
        };

        public delegate void SetProgressBar(double value, double maxvalue);

        public delegate void SetProgressBarVal(double value);

        public delegate void SetInfoText(string text);

        public GameControl() {
            InitializeComponent();
            UpdateTask = new TaskManager.Task() {
                Owner = this
            };
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                ElementHolder.RemoveChild(StartButton);
                ElementHolder.RemoveChild(UpdateBlock);
                WrapElement.Content = StartButton;
                Application.Current.MainWindow.TaskbarItemInfo = TaskBar;
                LanguageEnv.Languagechanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
                LoginManager.Instance.LoginCompleted += OnGameStartCompleted;
                LauncherEnv.Settings.ProfileChanged += OnProfileChanged;
                webClient.DownloadProgressChanged += OnDownloadProgressChanged;
                webClient.DownloadFileCompleted += OnDownloadFileCompleted;
                CheckWorker.DoWork += CheckWorker_DoWork;
                OnProfileChanged();
            }
        }

        private void OnProfileChanged() {
            StartButton.IsEnabled = false;
            StartButton.SetBinding(Button.ContentProperty, WaitingButtonBinding);
            CheckWorker.RunWorkerAsync();
        }

        #region Update Section

        private void CheckWorker_DoWork(object sender, DoWorkEventArgs e) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                //Добавляем задачу обновления
                TaskManager.Tasks.Add(UpdateTask);
                LauncherEnv.Settings.OnProfileLocked(true);
                LauncherEnv.Settings.OnFileSystemLocked(true);
                LauncherEnv.Settings.OnClosingLocked(true);
                UpdateRequired = false;
                StartButton.IsEnabled = false;
                StartButton.SetBinding(Button.ContentProperty, WaitingButtonBinding);
            }));
            GameFS = LauncherEnv.Settings.CurrentProfile.GameEnv.GetFS();

            //Проверяем наличие необходимых файлов игры
            if (!LauncherEnv.Settings.CurrentProfile.GameEnv.CheckGame()) {
                SetStartEnabled(false); //Если необходимых файлов нет, просто вызываю этот метод. он при вышеуказанном условии покажет неактивную кнопку и сообщение о неправильном пути
                return;                      //Далее идти нет смысла
            }

            //Проверяем наличие обновления Pack01 файлами. Возвражающее значение говорит, можно ли проходить далее по алгоритму
            if (!ImportPackage()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                    TaskManager.Tasks.Remove(UpdateTask);
                    LauncherEnv.Settings.OnProfileLocked(false);
                    LauncherEnv.Settings.OnFileSystemLocked(false);
                    LauncherEnv.Settings.OnClosingLocked(false);
                }));
                return;
            }

            //Проверяем наличие новых обновлений
            CheckResult cRes = CheckUpdates();
            //Если версии получили успешно
            if (cRes != null) {
                //Если обновление требуется
                if (cRes.IsUpdateRequired) {
                    //Если включен интегрированных движок обновления, пытаемся обновиться
                    if (LauncherEnv.Settings.CurrentProfile.UpdateEngineEnabled) {
                        bool UpdateRes = BeginUpdate(cRes.LocalVer, cRes.RemoteVer);
                        SetStartEnabled(UpdateRes);
                    } else { //Если интегрированный движок отключен - показываем кнопку "Обновить игру"
                        SetUpdateEnabled(true);
                    }
                } else { //Если обновление не требуется, показываем кнопку "Начать игру".
                    SetStartEnabled(true);
                }
            }
        }

        private bool ImportPackage() {
            //Необходимо импортировать директорию (Pack01), если имеется. Проверяем наличие этой папки
            if (Directory.Exists(LauncherEnv.Settings.CurrentProfile.GameEnv.GetImportPath())) {
                //Если включен интегрированных движок обновления, пытаемся импортировать
                if (LauncherEnv.Settings.CurrentProfile.UpdateEngineEnabled) {
                    //Проверяем наличие доступа к игре
                    while (!LauncherEnv.Settings.CurrentProfile.GameEnv.CheckUpdateAccess()) {
                        MessageBoxResult mRes = MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                        if (mRes == MessageBoxResult.Cancel) {
                            return false;
                        }
                    }

                    //Если есть, показываем, вызываем ивент начала и показываем блок с прогрессбарами
                    //Пытаемся открыть файлы игры на запись
                    bool IsOpened = false;
                    try {
                        IsOpened = GameFS.Open(FileAccess.Write, 16, LauncherEnv.Settings.CurrentProfile.GameEnv.GetHFPath(), LauncherEnv.Settings.CurrentProfile.GameEnv.GetPFPath());
                    } catch {
                        IsOpened = false;
                    }

                    //Если успешно открыли - применяем обновления
                    if (IsOpened) {
                        ShowProgressBar();  //Показываем прогрессбар и начинаем запись.
                        GameFS.WriteStatusChanged += OnWriteStatusChanged;
                        bool IsSuccess = GameFS.WriteDirectory(LauncherEnv.Settings.CurrentProfile.GameEnv.GetImportPath(), true);
                        GameFS.WriteStatusChanged -= OnWriteStatusChanged;
                        //Если сфейлилось, отправляем соответствующий ивент и сообщение
                        if (!IsSuccess) {
                            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                                Utils.MSG_ERROR(LanguageEnv.Strings.GameFilesInUse);
                            }));
                        }
                        GameFS.Close();
                        return IsSuccess;
                    } else {    //Файл не открылся, false
                        MessageBoxResult mRes = MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        SetUpdateEnabled(false);
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

        private CheckResult CheckUpdates() {
            //Если локальный файл с версией существует
            if (File.Exists(LauncherEnv.Settings.CurrentProfile.GameEnv.GetLocalVerFile())) {
                verCurrent = -1;
                verRemote = -1;
                //Открываем и парсим его
                StreamReader streamReader = new StreamReader(LauncherEnv.Settings.CurrentProfile.GameEnv.GetLocalVerFile());
                verCurrent = GetVersion(streamReader.ReadToEnd());
                streamReader.Close();

                try {
                    //Получаем и парсим удаленную версию
                    string result = LauncherEnv.WebClient.DownloadString(LauncherEnv.Settings.CurrentProfile.GameEnv.GetRemoteVerURL());
                    verRemote = GetVersion(result);

                    //Если хоть одна не спарсилась, возвращаем нулл
                    if (verRemote < 0 || verCurrent < 0)
                        return null;

                    //Возвращаем нормальное значение
                    return new CheckResult() {
                        LocalVer = verCurrent,
                        RemoteVer = verRemote
                    };
                } catch {
                    return null;
                }
            }
            //локального файла нет - возвращаем нулл
            return null;
        }

        private bool BeginUpdate(int local, int remote) {
            ShowProgressBar();
            bool updateSuccess = true;
            string packageFile;

            //Проверяем наличие доступа к игре
            while (!LauncherEnv.Settings.CurrentProfile.GameEnv.CheckUpdateAccess()) {
                MessageBoxResult mRes = MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                if (mRes == MessageBoxResult.Cancel)
                    return false;
            }

            double WholeContentLength = 0;
            for (int i = local + 1; i <= remote; i++) {
                WholeContentLength += GetFileLength(new Uri(string.Format(LauncherEnv.Settings.CurrentProfile.GameEnv.GetPatchURL(), i)));
            }
            UpdateMainProgressBar(0, WholeContentLength);

            for (int i = local + 1; i <= remote; i++) {
                verCurrent = i;
                updateSuccess = true;
                packageFile = LauncherEnv.Settings.CurrentProfile.GameEnv.GamePath + string.Format("\\UPDATE{0}.zip", i);
                UpdateSubProgressBar(0, 100);

                //downloading
                double CurrentContentLength = GetFileLength(new Uri(string.Format(LauncherEnv.Settings.CurrentProfile.GameEnv.GetPatchURL(), i)));
                try {
                    webClient.DownloadFileAsync(new Uri(string.Format(LauncherEnv.Settings.CurrentProfile.GameEnv.GetPatchURL(), i)), packageFile);
                    while (webClient.IsBusy) {
                        System.Threading.Thread.Sleep(100);
                    }
                } catch {
                    updateSuccess = false;
                }

                if (updateSuccess) {
                    ExtractUpdate(verCurrent, verRemote, packageFile, LauncherEnv.Settings.CurrentProfile.GameEnv.GamePath, true);
                }
                MainPBValue += CurrentContentLength;

                File.WriteAllLines(LauncherEnv.Settings.CurrentProfile.GameEnv.GetLocalVerFile(), new string[] { "[VERSION]", "version=" + verCurrent.ToString() });
            }

            //Проверяем наличие доступа к игре еще раз
            while (!LauncherEnv.Settings.CurrentProfile.GameEnv.CheckUpdateAccess()) {
                MessageBoxResult mRes = MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                if (mRes == MessageBoxResult.Cancel) {
                    return false;
                }
            }

            if (Directory.Exists(LauncherEnv.Settings.CurrentProfile.GameEnv.GetImportPath())) {
                //Открываем файловую систему игры
                bool IsOpened = false;
                try {
                    IsOpened = GameFS.Open(FileAccess.Write, 16, LauncherEnv.Settings.CurrentProfile.GameEnv.GetHFPath(), LauncherEnv.Settings.CurrentProfile.GameEnv.GetPFPath());
                } catch {
                    IsOpened = false;
                }

                //Если успешно открыли - применяем обновления
                if (IsOpened) {
                    GameFS.WriteStatusChanged += OnWriteStatusChanged;
                    bool IsSuccess = GameFS.WriteDirectory(LauncherEnv.Settings.CurrentProfile.GameEnv.GetImportPath(), true);
                    GameFS.WriteStatusChanged -= OnWriteStatusChanged;
                    //Если сфейлилось, отправляем соответствующее сообщение
                    if (!IsSuccess) {
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                            Utils.MSG_ERROR(LanguageEnv.Strings.GameFilesInUse);
                        }));
                    }
                    GameFS.Close();
                } else {
                    MessageBoxResult mRes = MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    SetUpdateEnabled(false);
                    return false;
                }
            }
            return true;
        }

        private void ExtractUpdate(int upd_num, int upd_num_of, string archiveFilenameIn, string outFolder, bool DeleteAfterExtract) {
            ZipFile zf = null;
            FileStream fs = null;
            try {
                fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);

                UpdateSubProgressBar(0, (int)zf.Count);
                int z_num = 1;
                foreach (ZipEntry zipEntry in zf) {
                    UpdateInfoText(1, upd_num, upd_num_of, z_num, zf.Count);
                    if (!zipEntry.IsFile) {
                        continue;
                    }
                    byte[] buffer = new byte[4096];
                    Stream zipStream = zf.GetInputStream(zipEntry);
                    string fullZipToPath = Path.Combine(outFolder, zipEntry.Name);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    using (FileStream streamWriter = File.Create(fullZipToPath)) {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                    UpdateSubProgressBar(z_num, (int)zf.Count);
                    z_num++;
                }
            } finally {
                if (zf != null) {
                    zf.IsStreamOwner = true;
                    zf.Close();
                }
                if (fs != null) {
                    fs.Close();
                }
            }

            if (DeleteAfterExtract) {
                try {
                    File.Delete(archiveFilenameIn);
                } catch {
                }
            }
        }

        /// <summary> Parse version file (like vGDMO.ini) </summary>
        /// <param name="text">Version file content</param>
        /// <returns> Version (integer) or -1 if version not found </returns>
        public static int GetVersion(string text) {
            string expr = "(version)(=)(\\d+)";
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(expr, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            System.Text.RegularExpressions.Match m = r.Match(text);
            if (m.Success) {
                return Convert.ToInt32(m.Groups[3].ToString());
            }
            return -1;
        }

        /// <summary> Returns Length of remote file </summary>
        /// <param name="url">Remote file Uri</param>
        /// <returns> ength of remote file </returns>
        public static double GetFileLength(Uri url) {
            System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
            req.Method = "HEAD";
            double ContentLength = 0;
            using (System.Net.WebResponse resp = req.GetResponse()) {
                double.TryParse(resp.Headers.Get("Content-Length"), out ContentLength);
            }
            return ContentLength;
        }

        #endregion Update Section

        #region Game Start/Login Section

        private void StartGame(string args) {
            StartButton.IsEnabled = false;
            if (ApplicationLauncher.StartGame(args, UpdateRequired)) {
                StartButton.SetBinding(Button.ContentProperty, WaitingButtonBinding);
                TaskManager.CloseApp();
            } else {
                LauncherEnv.Settings.OnProfileLocked(false);
                LauncherEnv.Settings.OnFileSystemLocked(false);
                StartButton.IsEnabled = true;
            }
        }

        private void OnGameStartCompleted(object sender, DMOLibrary.LoginCode code, string result) {
            //Если результат НЕУСПЕШЕН, возвращаем кнопку старта и возможность смены профиля
            if (code != DMOLibrary.LoginCode.SUCCESS) {
                StartButton.IsEnabled = true;
                if (UpdateRequired) {
                    StartButton.SetBinding(Button.ContentProperty, UpdateButtonBinding);
                } else {
                    StartButton.SetBinding(Button.ContentProperty, StartButtonBinding);
                }
                LauncherEnv.Settings.OnProfileLocked(false);
            }

            //Получаем результат логина
            switch (code) {
                case DMOLibrary.LoginCode.SUCCESS: {       //Если логин успешен, сохраняем аргументы текущей сессии вместе с настройками и запускаем игру
                        LauncherEnv.Settings.CurrentProfile.Login.LastSessionArgs = result;
                        LauncherEnv.Save();
                        StartGame(result);
                        break;
                    }
                case DMOLibrary.LoginCode.WRONG_PAGE:       //Если получены результаты ошибки на странице, отображаем сообщение с кодом ошибки
                case DMOLibrary.LoginCode.UNKNOWN_URL: {    //И возвращаем в форму ввода
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                            MessageBox.Show(LanguageEnv.Strings.LoginWrongPage + string.Format(" [{0}]", code), LanguageEnv.Strings.LoginLogIn, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }));
                        break;
                    }
            }
        }

        #endregion Game Start/Login Section

        #region Interface Section

        #region Start Button

        private void SetStartEnabled(bool IsEnabled) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                //Убираем задачу обновления
                TaskManager.Tasks.Remove(UpdateTask);
                TaskBar.ProgressState = TaskbarItemProgressState.None;
                LauncherEnv.Settings.OnProfileLocked(false);
                LauncherEnv.Settings.OnClosingLocked(false);
                LauncherEnv.Settings.OnFileSystemLocked(false);
                UpdateRequired = false;
                WrapElement.Content = StartButton;
                StartButton.SetBinding(Button.ContentProperty, StartButtonBinding);
                StartButton.IsEnabled = false;
                //Проверяем наличие необходимых файлов стандартного лаунчера. Если нету - просто показываем неактивную кнопку "Обновить игру" и сообщение об ошибке.
                if (!LauncherEnv.Settings.CurrentProfile.GameEnv.CheckGame()) {
                    MessageBox.Show(LanguageEnv.Strings.PleaseSelectGamePath, LanguageEnv.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                StartButton.IsEnabled = IsEnabled;
            }));
        }

        private void SetUpdateEnabled(bool IsEnabled) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                //Убираем задачу обновления
                try {
                    TaskManager.Tasks.Remove(UpdateTask);
                } catch (ArgumentOutOfRangeException e) {
                    // TODO Wtf this happening here?
                }
                TaskBar.ProgressState = TaskbarItemProgressState.None;
                LauncherEnv.Settings.OnProfileLocked(false);
                LauncherEnv.Settings.OnFileSystemLocked(false);
                LauncherEnv.Settings.OnClosingLocked(false);
                UpdateRequired = true;
                WrapElement.Content = StartButton;
                StartButton.SetBinding(Button.ContentProperty, UpdateButtonBinding);
                StartButton.IsEnabled = false;
                //Проверяем наличие необходимых файлов стандартного лаунчера. Если нету - просто показываем неактивную кнопку "Обновить игру" и сообщение об ошибке.
                if (!LauncherEnv.Settings.CurrentProfile.GameEnv.CheckDefLauncher()) {
                    MessageBox.Show(LanguageEnv.Strings.PleaseSelectLauncherPath, LanguageEnv.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                StartButton.IsEnabled = IsEnabled;
            }));
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e) {
            LauncherEnv.Settings.OnProfileLocked(true);
            LauncherEnv.Settings.OnFileSystemLocked(true);
            //Проверяем, требуется ли логин
            if (LauncherEnv.Settings.CurrentProfile.DMOProfile.IsLoginRequired) {
                LoginManager.Instance.Login();
            } else { //Логин не требуется, запускаем игру как есть
                StartGame(string.Empty);
            }
        }

        #endregion Start Button

        #region ProgressBar

        private void ShowProgressBar() {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                WrapElement.Content = UpdateBlock;
                UpdateText.Text = string.Empty;
                UpdateMainProgressBar(0, 100);
                UpdateSubProgressBar(0, 100);
                TaskBar.ProgressState = TaskbarItemProgressState.Normal;
            }));
        }

        private void OnWriteStatusChanged(object sender, int file_num, int file_count) {
            UpdateInfoText(2, file_num, file_count, null, null);
            UpdateMainProgressBar(file_num, file_count);
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            UpdateSubProgressBar(0, 100);
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            dataReceived = (e.BytesReceived / (1024.0 * 1024.0));
            dataTotal = (e.TotalBytesToReceive / (1024.0 * 1024.0));
            UpdateInfoText(0, verCurrent, verRemote, dataReceived, dataTotal);
            UpdateMainProgressBar(MainPBValue + e.BytesReceived);
            UpdateSubProgressBar(e.ProgressPercentage, 100);
        }

        private double MainPBValue = 0;

        private void UpdateMainProgressBar(double value, double maxvalue) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBar((value_, maxvalue_) => {
                MainProgressBar.Maximum = maxvalue_;
                MainProgressBar.Value = MainPBValue = value_;
                TaskBar.ProgressValue = MainProgressBar.Value / MainProgressBar.Maximum;
            }), value, maxvalue);
        }

        private void UpdateMainProgressBar(double value) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBarVal((value_) => {
                if (MainProgressBar.Maximum > value_) {
                    MainProgressBar.Value = value_;
                    TaskBar.ProgressValue = MainProgressBar.Value / MainProgressBar.Maximum;
                }
            }), value);
        }

        private void UpdateSubProgressBar(double value, double maxvalue) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBar((value_, maxvalue_) => {
                SubProgressBar.Maximum = maxvalue_;
                SubProgressBar.Value = value_;
            }), value, maxvalue);
        }

        private void UpdateInfoText(int code, object arg1, object arg2, object arg3, object arg4) {
            string text = string.Empty;
            switch (code) {
                case 0: {  //downloading
                        text = string.Format(LanguageEnv.Strings.UpdateDownloading, arg1, arg2, arg3, arg4);
                        break;
                    }
                case 1: {  //extracting
                        text = string.Format(LanguageEnv.Strings.UpdateExtracting, arg1, arg2, arg3, arg4);
                        break;
                    }
                case 2: {  //installing
                        text = string.Format(LanguageEnv.Strings.UpdateInstalling, arg1, arg2);
                        break;
                    }
            }
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetInfoText((text_) => {
                UpdateText.Text = text_;
            }), text);
        }

        #endregion ProgressBar

        #endregion Interface Section
    }
}