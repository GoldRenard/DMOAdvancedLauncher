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
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Windows.Threading;
using System.Windows.Shell;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using DMOLibrary.DMOFileSystem;
using AdvancedLauncher.Service;
using AdvancedLauncher.Environment;

namespace AdvancedLauncher.Controls
{
    public partial class GameControl : UserControl
    {
        TaskManager.Task UpdateTask;
        private bool UpdateRequired = false;
        Storyboard ShowLoginBlockFirst, ShowLoginBlock;
        Storyboard ShowWaitingBlockFirst, ShowWaitingBlock;
        Storyboard HideLogin;

        TaskbarItemInfo taskbar = new TaskbarItemInfo();
        WebClient web_Downloader = new WebClient();
        DMOFileSystem GameFS = null;
        BackgroundWorker CheckWorker = new BackgroundWorker();

        Binding StartButtonBinding = new Binding("StartButton");
        Binding WaitingButtonBinding = new Binding("GameButton_Waiting");
        Binding UpdateButtonBinding = new Binding("GameButton_UpdateGame");

        double MB_R, MB_T;
        int p_current = -1;
        int p_remote = -1;

        class CheckResult
        {
            public int LocalVer;
            public int RemoteVer;
            public bool IsUpdateRequired { get { return RemoteVer > LocalVer; } }
        };

        public delegate void SetProgressBar(double value, double maxvalue);
        public delegate void SetProgressBarVal(double value);
        public delegate void SetInfoText(string text);

        public GameControl()
        {
            InitializeComponent();
            UpdateTask = new TaskManager.Task() { Owner = this };
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                HideLogin = ((Storyboard)this.FindResource("HideLogin"));
                ShowLoginBlockFirst = ((Storyboard)this.FindResource("ShowLoginBlockFirst"));
                ShowLoginBlock = ((Storyboard)this.FindResource("ShowLoginBlock"));
                ShowWaitingBlockFirst = ((Storyboard)this.FindResource("ShowWaitingBlockFirst"));
                ShowWaitingBlock = ((Storyboard)this.FindResource("ShowWaitingBlock"));

                Application.Current.MainWindow.TaskbarItemInfo = taskbar;
                LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
                LauncherEnv.Settings.ProfileChanged += ProfileChanged;
                web_Downloader.DownloadProgressChanged += web_Downloader_DownloadProgressChanged;
                web_Downloader.DownloadFileCompleted += web_Downloader_DownloadFileCompleted;
                CheckWorker.DoWork += CheckWorker_DoWork;
                ProfileChanged();
            }
        }

        void ProfileChanged()
        {
            InitLoginBlock();
            StartButton.IsEnabled = false;
            StartButton.SetBinding(Button.ContentProperty, WaitingButtonBinding);
            LastSession.IsChecked = false;
            LastSession.Visibility = Visibility.Collapsed;
            if (LauncherEnv.Settings.pCurrent.GameEnv.IsLastSessionAvailable() && !string.IsNullOrEmpty(LauncherEnv.Settings.pCurrent.Login.pLastSessionArgs))
                LastSession.Visibility = Visibility.Visible;
            CheckWorker.RunWorkerAsync();
        }

        #region Update Section
        private void CheckWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
            {
                //Добавляем задачу обновления
                TaskManager.Tasks.Add(UpdateTask);
                LauncherEnv.Settings.OnProfileLocked(true);
                LauncherEnv.Settings.OnFileSystemLocked(true);
                LauncherEnv.Settings.OnClosingLocked(true);
                UpdateRequired = false;
                StartButton.IsEnabled = false;
                StartButton.SetBinding(Button.ContentProperty, WaitingButtonBinding);
                UpdateBlock.Visibility = Visibility.Collapsed;
            }));
            GameFS = LauncherEnv.Settings.pCurrent.GameEnv.GetFS();

            //Проверяем наличие необходимых файлов игры
            if (!LauncherEnv.Settings.pCurrent.GameEnv.CheckGame())
            {
                StartButton_SetStart(false); //Если необходимых файлов нет, просто вызываю этот метод. он при вышеуказанном условии покажет неактивную кнопку и сообщение о неправильном пути
                return;                      //Далее идти нет смысла
            }

            //Проверяем наличие обновления Pack01 файлами. Возвражающее значение говорит, можно ли проходить далее по алгоритму
            if (!ImportPackage())
            {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
                {
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
            if (cRes != null)
            {
                //Если обновление требуется
                if (cRes.IsUpdateRequired)
                {
                    //Если включен интегрированных движок обновления, пытаемся обновиться
                    if (LauncherEnv.Settings.pCurrent.UpdateEngine)
                    {
                        bool UpdateRes = BeginUpdate(cRes.LocalVer, cRes.RemoteVer);
                        StartButton_SetStart(UpdateRes);
                    }
                    else //Если интегрированный движок отключен - показываем кнопку "Обновить игру"
                        StartButton_SetUpdate(true);
                }
                else //Если обновление не требуется, показываем кнопку "Начать игру".
                    StartButton_SetStart(true);
            }
        }

        private bool ImportPackage()
        {
            //Необходимо импортировать директорию (Pack01), если имеется. Проверяем наличие этой папки
            if (Directory.Exists(LauncherEnv.Settings.pCurrent.GameEnv.GetImportPath()))
            {
                //Если включен интегрированных движок обновления, пытаемся импортировать
                if (LauncherEnv.Settings.pCurrent.UpdateEngine)
                {
                    //Проверяем наличие доступа к игре
                    while (!LauncherEnv.Settings.pCurrent.GameEnv.CheckUpdateAccess())
                    {
                        MessageBoxResult mRes = MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                        if (mRes == MessageBoxResult.Cancel)
                            return false;
                    }

                    //Если есть, показываем, вызываем ивент начала и показываем блок с прогрессбарами
                    //Пытаемся открыть файлы игры на запись
                    bool IsOpened = false;
                    try { IsOpened = GameFS.Open(FileAccess.Write, 16, LauncherEnv.Settings.pCurrent.GameEnv.GetHFPath(), LauncherEnv.Settings.pCurrent.GameEnv.GetPFPath()); }
                    catch { IsOpened = false; }

                    //Если успешно открыли - применяем обновления
                    if (IsOpened)
                    {
                        ShowProgressBar();  //Показываем прогрессбар и начинаем запись.
                        GameFS.WriteStatusChanged += GameFS_WriteStatusChanged;
                        bool IsSuccess = GameFS.WriteDirectory(LauncherEnv.Settings.pCurrent.GameEnv.GetImportPath(), true);
                        GameFS.WriteStatusChanged -= GameFS_WriteStatusChanged;
                        //Если сфейлилось, отправляем соответствующий ивент и сообщение
                        if (!IsSuccess)
                        {
                            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
                            {
                                Utils.MSG_ERROR(LanguageEnv.Strings.GameFilesInUse);
                            }));
                        }
                        GameFS.Close();
                        return IsSuccess;
                    }
                    else   //Файл не открылся, false
                    {
                        MessageBoxResult mRes = MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        StartButton_SetUpdate(false);
                        return false;
                    }
                }
                else
                {
                    //Интегрированный движок отключен, поэтому мы активируем кнопку обновления.
                    StartButton_SetUpdate(true);
                    return false; //Далее по алгоритму идти не нужно, поэтому false
                }
            }
            return true;
        }

        private CheckResult CheckUpdates()
        {
            //Если локальный файл с версией существует
            if (File.Exists(LauncherEnv.Settings.pCurrent.GameEnv.GetLocalVerFile()))
            {
                p_current = -1;
                p_remote = -1;
                //Открываем и парсим его
                StreamReader streamReader = new StreamReader(LauncherEnv.Settings.pCurrent.GameEnv.GetLocalVerFile());
                p_current = GetVersion(streamReader.ReadToEnd());
                streamReader.Close();

                try
                {
                    //Получаем и парсим удаленную версию
                    string result = LauncherEnv.WebClient.DownloadString(LauncherEnv.Settings.pCurrent.GameEnv.GetRemoteVerURL());
                    p_remote = GetVersion(result);

                    //Если хоть одна не спарсилась, возвращаем нулл
                    if (p_remote < 0 || p_current < 0)
                        return null;

                    //Возвращаем нормальное значение
                    return new CheckResult() { LocalVer = p_current, RemoteVer = p_remote };
                }
                catch { return null; }
            }
            //локального файла нет - возвращаем нулл
            return null;
        }

        private bool BeginUpdate(int local, int remote)
        {
            ShowProgressBar();
            bool p_success = true;
            string p_file;

            //Проверяем наличие доступа к игре
            while (!LauncherEnv.Settings.pCurrent.GameEnv.CheckUpdateAccess())
            {
                MessageBoxResult mRes = MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                if (mRes == MessageBoxResult.Cancel)
                    return false;
            }

            double WholeContentLength = 0;
            for (int i = local + 1; i <= remote; i++)
                WholeContentLength += GetFileLength(new Uri(string.Format(LauncherEnv.Settings.pCurrent.GameEnv.GetPatchURL(), i)));
            UpdateMainProgressBar(0, WholeContentLength);

            for (int i = local + 1; i <= remote; i++)
            {
                p_current = i;
                p_success = true;
                p_file = LauncherEnv.Settings.pCurrent.GameEnv.GamePath + string.Format("\\UPDATE{0}.zip", i);
                UpdateSubProgressBar(0, 100);

                //downloading
                double CurrentContentLength = GetFileLength(new Uri(string.Format(LauncherEnv.Settings.pCurrent.GameEnv.GetPatchURL(), i)));
                try
                {
                    web_Downloader.DownloadFileAsync(new Uri(string.Format(LauncherEnv.Settings.pCurrent.GameEnv.GetPatchURL(), i)), p_file);
                    while (web_Downloader.IsBusy)
                        System.Threading.Thread.Sleep(100);
                }
                catch { p_success = false; }

                if (p_success)
                    ExtractUpdate(p_current, p_remote, p_file, LauncherEnv.Settings.pCurrent.GameEnv.GamePath, true);
                MainPBValue += CurrentContentLength;

                File.WriteAllLines(LauncherEnv.Settings.pCurrent.GameEnv.GetLocalVerFile(), new string[] { "[VERSION]", "version=" + p_current.ToString() });
            }

            //Проверяем наличие доступа к игре еще раз
            while (!LauncherEnv.Settings.pCurrent.GameEnv.CheckUpdateAccess())
            {
                MessageBoxResult mRes = MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                if (mRes == MessageBoxResult.Cancel)
                    return false;
            }

            if (Directory.Exists(LauncherEnv.Settings.pCurrent.GameEnv.GetImportPath()))
            {
                //Открываем файловую систему игры
                bool IsOpened = false;
                try { IsOpened = GameFS.Open(FileAccess.Write, 16, LauncherEnv.Settings.pCurrent.GameEnv.GetHFPath(), LauncherEnv.Settings.pCurrent.GameEnv.GetPFPath()); }
                catch { IsOpened = false; }

                //Если успешно открыли - применяем обновления
                if (IsOpened)
                {
                    GameFS.WriteStatusChanged += GameFS_WriteStatusChanged;
                    bool IsSuccess = GameFS.WriteDirectory(LauncherEnv.Settings.pCurrent.GameEnv.GetImportPath(), true);
                    GameFS.WriteStatusChanged -= GameFS_WriteStatusChanged;
                    //Если сфейлилось, отправляем соответствующее сообщение
                    if (!IsSuccess)
                    {
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
                        {
                            Utils.MSG_ERROR(LanguageEnv.Strings.GameFilesInUse);
                        }));
                    }
                    GameFS.Close();
                }
                else
                {
                    MessageBoxResult mRes = MessageBox.Show(LanguageEnv.Strings.GameFilesInUse, LanguageEnv.Strings.PleaseCloseGame, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    StartButton_SetUpdate(false);
                    return false;
                }
            }
            return true;
        }

        private void ExtractUpdate(int upd_num, int upd_num_of, string archiveFilenameIn, string outFolder, bool DeleteAfterExtract)
        {
            ZipFile zf = null;
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);

                UpdateSubProgressBar(0, (int)zf.Count);
                int z_num = 1;
                foreach (ZipEntry zipEntry in zf)
                {
                    UpdateInfoText(1, upd_num, upd_num_of, z_num, zf.Count);
                    if (!zipEntry.IsFile)
                        continue;
                    byte[] buffer = new byte[4096];
                    Stream zipStream = zf.GetInputStream(zipEntry);
                    string fullZipToPath = Path.Combine(outFolder, zipEntry.Name);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                    UpdateSubProgressBar(z_num, (int)zf.Count);
                    z_num++;
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true;
                    zf.Close();
                }
                if (fs != null)
                    fs.Close();
            }

            if (DeleteAfterExtract)
            {
                try { File.Delete(archiveFilenameIn); }
                catch { }
            }
        }

        /// <summary> Parse version file (like vGDMO.ini) </summary>
        /// <param name="text">Version file content</param>
        /// <returns> Version (integer) or -1 if version not found </returns>
        public static int GetVersion(string text)
        {
            string expr = "(version)(=)(\\d+)";
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(expr, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            System.Text.RegularExpressions.Match m = r.Match(text);
            if (m.Success)
                return Convert.ToInt32(m.Groups[3].ToString());
            return -1;
        }

        /// <summary> Returns Length of remote file </summary>
        /// <param name="url">Remote file Uri</param>
        /// <returns> ength of remote file </returns>
        public static double GetFileLength(Uri url)
        {
            System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
            req.Method = "HEAD";
            double ContentLength = 0;
            using (System.Net.WebResponse resp = req.GetResponse())
            {
                double.TryParse(resp.Headers.Get("Content-Length"), out ContentLength);
            }
            return ContentLength;
        }

        #endregion

        #region Game Start/Login Section

        private void StartGame(string args)
        {
            //Применить все ссылки
            LauncherEnv.Settings.pCurrent.GameEnv.SetRegistryPaths();

            if (ApplicationLauncher.Execute(
                UpdateRequired ? LauncherEnv.Settings.pCurrent.GameEnv.GetDefLauncherEXE() : LauncherEnv.Settings.pCurrent.GameEnv.GetGameEXE(),
                UpdateRequired ? LauncherEnv.Settings.pCurrent.DMOProfile.GetLauncherStartArgs(args) : LauncherEnv.Settings.pCurrent.DMOProfile.GetGameStartArgs(args),
                LauncherEnv.Settings.pCurrent.AppLocale))
            {
                StartButton.SetBinding(Button.ContentProperty, WaitingButtonBinding);
                TaskManager.CloseApp(); //Если удалось, закрываем приложение.
            }
            else                        //Если не удалось, разрешаем повторный запуск и смену профиля.
            {
                LauncherEnv.Settings.OnProfileLocked(false);
                LauncherEnv.Settings.OnFileSystemLocked(false);
                StartButton.IsEnabled = true;
            }
        }

        private void StartLogin()
        {
            StartButton.SetBinding(Button.ContentProperty, WaitingButtonBinding);
            LauncherEnv.Settings.pCurrent.DMOProfile.LoginStateChanged += DMOProfile_LoginStateChanged;
            LauncherEnv.Settings.pCurrent.DMOProfile.LoginCompleted += DMOProfile_GameStartCompleted;
            LauncherEnv.Settings.pCurrent.DMOProfile.TryLogin(Login.Text, Password.SecurePassword);
        }

        void DMOProfile_LoginStateChanged(object sender, DMOLibrary.LoginState state, int try_num, int last_error)
        {
            ShowWaitingFunc();
            if (state == DMOLibrary.LoginState.LOGINNING)
                LoginStatus2.Text = LanguageEnv.Strings.LoginLogIn;
            else if (state == DMOLibrary.LoginState.GETTING_DATA)
                LoginStatus2.Text = LanguageEnv.Strings.LoginGettingData;
            LoginStatus1.Text = string.Format(LanguageEnv.Strings.LoginTry, try_num);
            if (last_error != -1)
                LoginStatus1.Text += string.Format(" ({0} {1})", LanguageEnv.Strings.LoginWasError, last_error);
        }

        void DMOProfile_GameStartCompleted(object sender, DMOLibrary.LoginCode code, string result)
        {
            LauncherEnv.Settings.pCurrent.DMOProfile.LoginStateChanged -= DMOProfile_LoginStateChanged;
            LauncherEnv.Settings.pCurrent.DMOProfile.LoginCompleted -= DMOProfile_GameStartCompleted;
            //Если результат НЕУСПЕШЕН, возвращаем кнопку старта и возможность смены профиля
            if (code != DMOLibrary.LoginCode.SUCCESS)
            {
                StartButton.IsEnabled = true;
                if (UpdateRequired)
                    StartButton.SetBinding(Button.ContentProperty, UpdateButtonBinding);
                else
                    StartButton.SetBinding(Button.ContentProperty, StartButtonBinding);
                LauncherEnv.Settings.OnProfileLocked(false);
            }

            //Получаем результат логина
            switch (code)
            {
                case DMOLibrary.LoginCode.SUCCESS:      //Если логин успешен, сохраняем аргументы текущей сессии вместе с настройками и запускаем игру
                    {
                        LauncherEnv.Settings.pCurrent.Login.pLastSessionArgs = result;
                        LauncherEnv.Save();
                        InitLoginBlock();
                        StartGame(result);
                        break;
                    }
                case DMOLibrary.LoginCode.WRONG_USER:   //Если получен результат неправильного пользователя, отображаем сообщение и форму ввода
                    {
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
                        {
                            //Показываем сообщение только, когда блок открыт
                            if (LoginBlock.Height != 0)
                                MessageBox.Show(LanguageEnv.Strings.LoginBadAccount, LanguageEnv.Strings.LoginLogIn, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }));
                        ShowLoginFunc();
                        break;
                    }
                case DMOLibrary.LoginCode.WRONG_PAGE:     //Если получены результаты ошибки на странице, отображаем сообщение с кодом ошибки
                case DMOLibrary.LoginCode.UNKNOWN_URL:    //И возвращаем в форму ввода
                    {
                        ShowLoginFunc();
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
                        {
                            MessageBox.Show(LanguageEnv.Strings.LoginWrongPage + string.Format(" [{0}]", code), LanguageEnv.Strings.LoginLogIn, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }));
                        break;
                    }
            }
        }

        #endregion

        #region Interface Section

        #region Start Button
        private void StartButton_SetStart(bool IsEnabled)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
            {
                //Убираем задачу обновления
                TaskManager.Tasks.Remove(UpdateTask);
                taskbar.ProgressState = TaskbarItemProgressState.None;
                LauncherEnv.Settings.OnProfileLocked(false);
                LauncherEnv.Settings.OnClosingLocked(false);
                LauncherEnv.Settings.OnFileSystemLocked(false);
                UpdateRequired = false;
                UpdateBlock.Visibility = System.Windows.Visibility.Collapsed;
                StartBlock.Visibility = System.Windows.Visibility.Visible;
                StartButton.SetBinding(Button.ContentProperty, StartButtonBinding);
                StartButton.IsEnabled = false;
                //Проверяем наличие необходимых файлов стандартного лаунчера. Если нету - просто показываем неактивную кнопку "Обновить игру" и сообщение об ошибке.
                if (!LauncherEnv.Settings.pCurrent.GameEnv.CheckGame())
                {
                    MessageBox.Show(LanguageEnv.Strings.PleaseSelectGamePath, LanguageEnv.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                StartButton.IsEnabled = IsEnabled;
            }));
        }

        private void StartButton_SetUpdate(bool IsEnabled)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
            {
                //Убираем задачу обновления
                TaskManager.Tasks.Remove(UpdateTask);
                taskbar.ProgressState = TaskbarItemProgressState.None;
                LauncherEnv.Settings.OnProfileLocked(false);
                LauncherEnv.Settings.OnFileSystemLocked(false);
                LauncherEnv.Settings.OnClosingLocked(false);
                UpdateRequired = true;
                UpdateBlock.Visibility = System.Windows.Visibility.Collapsed;
                StartBlock.Visibility = System.Windows.Visibility.Visible;
                StartButton.SetBinding(Button.ContentProperty, UpdateButtonBinding);
                StartButton.IsEnabled = false;
                //Проверяем наличие необходимых файлов стандартного лаунчера. Если нету - просто показываем неактивную кнопку "Обновить игру" и сообщение об ошибке.
                if (!LauncherEnv.Settings.pCurrent.GameEnv.CheckDefLauncher())
                {
                    MessageBox.Show(LanguageEnv.Strings.PleaseSelectLauncherPath, LanguageEnv.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                StartButton.IsEnabled = IsEnabled;
            }));
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            LauncherEnv.Settings.OnProfileLocked(true);
            LauncherEnv.Settings.OnFileSystemLocked(true);
            StartButton.IsEnabled = false;
            //Проверяем, требуется ли логин
            if (LauncherEnv.Settings.pCurrent.DMOProfile.IsLoginRequired)
            {
                //Проверяем, отмечена ли галка последней сессии. Если отмечена - запускаем посл. сессию, иначе логинимся
                if ((bool)LastSession.IsChecked)
                    StartGame(LauncherEnv.Settings.pCurrent.Login.pLastSessionArgs);
                else
                {
                    //Иначе - заполняем поля нужными нам данными (только в первый раз) и логинимся
                    if (!IsLoginDataLoaded)
                    {
                        Login.Text = LauncherEnv.Settings.pCurrent.Login.User;
                        Password.Password = PassEncrypt.ConvertToUnsecureString(LauncherEnv.Settings.pCurrent.Login.SecurePassword);
                        IsLoginDataLoaded = true;
                    }
                    StartLogin();
                }
            }
            else //Логин не требуется, запускаем игру как есть
                StartGame(string.Empty);
        }
        #endregion

        #region ProgressBar

        void ShowProgressBar()
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
            {
                StartBlock.Visibility = System.Windows.Visibility.Collapsed;
                UpdateBlock.Visibility = System.Windows.Visibility.Visible;
                UpdateText.Text = string.Empty;
                UpdateMainProgressBar(0, 100);
                UpdateSubProgressBar(0, 100);
                taskbar.ProgressState = TaskbarItemProgressState.Normal;
            }));
        }

        void GameFS_WriteStatusChanged(object sender, int file_num, int file_count)
        {
            UpdateInfoText(2, file_num, file_count, null, null);
            UpdateMainProgressBar(file_num, file_count);
        }

        void web_Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UpdateSubProgressBar(0, 100);
        }

        void web_Downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            MB_R = (e.BytesReceived / (1024.0 * 1024.0));
            MB_T = (e.TotalBytesToReceive / (1024.0 * 1024.0));
            UpdateInfoText(0, p_current, p_remote, MB_R, MB_T);
            int t = e.ProgressPercentage;

            UpdateMainProgressBar(MainPBValue + e.BytesReceived);
            UpdateSubProgressBar(e.ProgressPercentage, 100);
        }

        private double MainPBValue = 0;
        private void UpdateMainProgressBar(double value, double maxvalue)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBar((value_, maxvalue_) =>
            {
                MainProgressBar.Maximum = maxvalue_;
                MainProgressBar.Value = MainPBValue = value_;
                taskbar.ProgressValue = MainProgressBar.Value / MainProgressBar.Maximum;
            }), value, maxvalue);
        }

        private void UpdateMainProgressBar(double value)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBarVal((value_) =>
            {
                if (MainProgressBar.Maximum > value_)
                {
                    MainProgressBar.Value = value_;
                    taskbar.ProgressValue = MainProgressBar.Value / MainProgressBar.Maximum;
                }
            }), value);
        }

        private void UpdateSubProgressBar(double value, double maxvalue)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBar((value_, maxvalue_) =>
            {
                SubProgressBar.Maximum = maxvalue_;
                SubProgressBar.Value = value_;
            }), value, maxvalue);
        }

        private void UpdateInfoText(int code, object arg1, object arg2, object arg3, object arg4)
        {
            string text = string.Empty;
            switch (code)
            {
                case 0: //downloading
                    {
                        text = string.Format(LanguageEnv.Strings.UpdateDownloading, arg1, arg2, arg3, arg4);
                        break;
                    }
                case 1: //extracting
                    {
                        text = string.Format(LanguageEnv.Strings.UpdateExtracting, arg1, arg2, arg3, arg4);
                        break;
                    }
                case 2: //installing
                    {
                        text = string.Format(LanguageEnv.Strings.UpdateInstalling, arg1, arg2);
                        break;
                    }
            }
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetInfoText((text_) =>
            {
                UpdateText.Text = text_;
            }), text);
        }
        #endregion

        #region LoginBlock
        private bool IsLoginDataLoaded = false;

        private void InitLoginBlock()
        {
            IsLoginDataLoaded = false;
            Login.Text = Password.Password = string.Empty;
            HideLogin.Stop();
            ShowLoginBlockFirst.Stop();
            ShowLoginBlock.Stop();
            ShowWaitingBlockFirst.Stop();
            ShowWaitingBlock.Stop();
            HideLogin.Begin();
        }

        private void ShowLoginFunc()
        {
            if (LoginBlock.Height == 0)
                ShowLoginBlockFirst.Begin();
            else
                ShowLoginBlock.Begin();
        }

        private void ShowWaitingFunc()
        {
            if (LoginBlock.Height == 0)
                ShowWaitingBlockFirst.Begin();
            else
                ShowWaitingBlock.Begin();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Password.Password.Length == 0)
                PW_Watermark.Visibility = System.Windows.Visibility.Visible;
            else
                PW_Watermark.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void LoginForm_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Return && e.Key != System.Windows.Input.Key.Enter)
                return;
            e.Handled = true;
            StartButton_Click(this, null);
        }

        #endregion

        #endregion
    }
}
