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
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Windows.Threading;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using DMOLibrary.DMOFileSystem;
using System.Runtime.InteropServices;
using System.Windows.Shell;

namespace AdvancedLauncher
{
    public partial class GameUpdater : UserControl
    {
        private const int SC_CLOSE = 0xF060;
        private const int MF_GRAYED = 0x1;
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, int wEnable);

        TaskbarItemInfo taskbar = new TaskbarItemInfo();
        IntPtr Main_hWnd;
        Dispatcher owner_dispatcher;
        WebClient web_Downloader = new WebClient();
        DMOFileSystem dmo_fs;

        double MB_R, MB_T;
        int p_current = -1;
        int p_remote = -1;

        public delegate void SetProgressBar(int value, int maxvalue);
        public delegate void SetProgressBarVal(int value);
        public delegate void SetInfoText(string text);

        #region EVENTS
        public delegate void StatusEventHandler(object sender);
        public event StatusEventHandler UpdateStarted;
        public event StatusEventHandler UpdateCompleted;
        public event StatusEventHandler UpdateFailed;
        public event StatusEventHandler DefaultUpdateRequired;
        protected virtual void OnBegin()
        {
            BlockX(true);
            if (UpdateStarted != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new StatusEventHandler((sender) =>
                    {
                        taskbar.ProgressState = TaskbarItemProgressState.Normal;
                        taskbar.ProgressValue = 0.0 / 100.0;
                        UpdateStarted(sender);
                    }), this);
                }
                else
                {
                    taskbar.ProgressState = TaskbarItemProgressState.Normal;
                    taskbar.ProgressValue = 0.0 / 100.0;
                    UpdateStarted(this);
                }
            }
        }

        protected virtual void OnComplete()
        {
            BlockX(false);
            if (UpdateCompleted != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new StatusEventHandler((sender) =>
                    {
                        taskbar.ProgressState = TaskbarItemProgressState.None;
                        UpdateCompleted(sender);
                    }), this);
                }
                else
                {
                    taskbar.ProgressState = TaskbarItemProgressState.None;
                    UpdateCompleted(this);
                }
            }
        }

        protected virtual void OnFail()
        {
            
            BlockX(false);
            if (UpdateFailed != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new StatusEventHandler((sender) =>
                    {
                        taskbar.ProgressState = TaskbarItemProgressState.None;
                        UpdateFailed(sender);
                    }), this);
                }
                else
                {
                    taskbar.ProgressState = TaskbarItemProgressState.None;
                    UpdateFailed(this);
                }
            }
        }

        protected virtual void OnDefaultUpdateRequired()
        {
            BlockX(false);
            if (DefaultUpdateRequired != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new StatusEventHandler((sender) =>
                    {
                        taskbar.ProgressState = TaskbarItemProgressState.None;
                        DefaultUpdateRequired(sender);
                    }), this);
                }
                else
                {
                    taskbar.ProgressState = TaskbarItemProgressState.None;
                    DefaultUpdateRequired(this);
                }
            }
        }
        #endregion

        public GameUpdater()
        {
            dmo_fs = new DMOFileSystem(this.Dispatcher, 16, App.DMOProfile.GetPackHFPath(), App.DMOProfile.GetPackPFPath());
            dmo_fs.WriteStatusChanged += dmo_fs_WriteStatusChanged;
            web_Downloader.DownloadProgressChanged += web_Downloader_DownloadProgressChanged;
            web_Downloader.DownloadFileCompleted += web_Downloader_DownloadFileCompleted;
            InitializeComponent();
            Application.Current.MainWindow.TaskbarItemInfo = taskbar;
        }

        public void CheckUpdates(Dispatcher owner_dispatcher)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;
            Main_hWnd = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle;
            this.owner_dispatcher = owner_dispatcher;
            BackgroundWorker bw_checkupdates = new BackgroundWorker();
            bw_checkupdates.DoWork += (s1, e2) =>
            {
                if (File.Exists(App.DMOProfile.GetVersionFile()))
                {
                    StreamReader streamReader = new StreamReader(App.DMOProfile.GetVersionFile());
                    p_current = Utils.GetVersion(streamReader.ReadToEnd());
                    streamReader.Close();
                }

                if (App.DMOProfile.IsUpdateSupported && App.DMOProfile.S_USE_UPDATE_ENGINE && Directory.Exists(App.DMOProfile.GetPackImportDir()))
                {
                    OnBegin();
                    while (!App.DMOProfile.CheckUpdateAccess()) ;
                    dmo_fs.WriteDirectory(App.DMOProfile.GetPackImportDir(), true);
                }

                WebClient client = new WebClient();
                client.Proxy = (IWebProxy)null;
                try
                {
                    string result = client.DownloadString(App.DMOProfile.GetRemoteVersionURL());
                    p_remote = Utils.GetVersion(result);

                    if (p_remote < 0 || p_current < 0)
                    {
                        OnFail();
                        Utils.MSG_ERROR(LanguageProvider.strings.UPDATE_CANT_GET_GAME_VERSION);
                        return;
                    }

                    if (p_remote > p_current)
                    {
                        if (App.DMOProfile.IsUpdateSupported && App.DMOProfile.S_USE_UPDATE_ENGINE)
                            BeginUpdate(p_current, p_remote);
                        else
                            OnDefaultUpdateRequired();
                    }
                    else
                        OnComplete();
                }
                catch (Exception ex) { Utils.MSG_ERROR(LanguageProvider.strings.UPDATE_CANT_CONNECT_TO_JOYMAX + " " + ex.Message); }
            };
            bw_checkupdates.RunWorkerAsync();
        }

        private void BeginUpdate(int local, int remote)
        {
            OnBegin();
            bool p_success = true;
            string p_file;
            string GAME_PATH = App.DMOProfile.GetGamePath();
            while (!App.DMOProfile.CheckUpdateAccess())
                MessageBox.Show(LanguageProvider.strings.GAME_FILES_IN_USE, LanguageProvider.strings.NEED_CLOSE_GAME, MessageBoxButton.OK, MessageBoxImage.Warning);
            UpdateMainProgressBar(0, remote - local);
            for (int i = local + 1; i <= remote; i++)
            {
                p_current = i;
                p_success = true;
                p_file = GAME_PATH + string.Format("\\UPDATE{0}.zip", i);
                UpdateSubProgressBar(0, 100);

                //downloading
                try
                {
                    web_Downloader.DownloadFileAsync(new Uri(string.Format(App.DMOProfile.GetPatchURL(), i)), p_file);
                    while (web_Downloader.IsBusy)
                        System.Threading.Thread.Sleep(100);
                }
                catch (Exception ex) {
                    Utils.MSG_ERROR(LanguageProvider.strings.UPDATE_CANT_CONNECT_TO_JOYMAX + " " + ex.Message);
                    p_success = false;
                }

                if (p_success)
                    ExtractUpdate(p_current, p_remote, p_file, GAME_PATH, true);
                UpdateMainProgressBar(i - local);
            }

            while (!App.DMOProfile.CheckUpdateAccess()) ; 
            //install updates to game
            dmo_fs.WriteDirectory(App.DMOProfile.GetPackImportDir(), true);
            File.WriteAllLines(App.DMOProfile.GetVersionFile(), new string[] { "[VERSION]", "version=" + p_current.ToString() });
            OnComplete();
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

        #region Interface operations

        void dmo_fs_WriteStatusChanged(object sender, int file_num, int file_count)
        {
            UpdateInfoText(2, file_num, file_count, null, null);
            UpdateSubProgressBar(file_num, file_count);
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
            UpdateSubProgressBar(e.ProgressPercentage, 100);
        }

        private void BlockX(bool isEnable)
        {
            EnableMenuItem(GetSystemMenu(Main_hWnd, !isEnable), SC_CLOSE, MF_GRAYED);
        }

        private void UpdateMainProgressBar(int value, int maxvalue)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBar((value_, maxvalue_) =>
            {
                MainProgressBar.Maximum = maxvalue_;
                MainProgressBar.Value = value_;
            }), value, maxvalue);
        }

        private void UpdateMainProgressBar(int value)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBarVal((value_) =>
            {
                MainProgressBar.Value = value_;
            }), value);
        }

        private void UpdateSubProgressBar(int value, int maxvalue)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetProgressBar((value_, maxvalue_) =>
            {
                SubProgressBar.Maximum = maxvalue_;
                SubProgressBar.Value = value_;
                taskbar.ProgressValue = (double)value_ / (double)maxvalue_;
            }), value, maxvalue);
        }

        private void UpdateInfoText(int code, object arg1, object arg2, object arg3, object arg4)
        {
            string text = string.Empty;
            switch (code)
            {
                case 0: //downloading
                    {
                        text = string.Format(LanguageProvider.strings.UPDATE_DOWNLOADING, arg1, arg2, arg3, arg4);
                        break;
                    }
                case 1: //extracting
                    {
                        text = string.Format(LanguageProvider.strings.UPDATE_EXTRACTING, arg1, arg2, arg3, arg4);
                        break;
                    }
                case 2: //installing
                    {
                        text = string.Format(LanguageProvider.strings.UPDATE_INSTALLING, arg1, arg2);
                        break;
                    }
            }
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new SetInfoText((text_) =>
            {
                UpdateText.Text = text_;
            }), text);
        }
        #endregion
    }
}
