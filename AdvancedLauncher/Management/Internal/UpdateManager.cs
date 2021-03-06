﻿// ======================================================================
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.Tools;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Ninject;

namespace AdvancedLauncher.Management.Internal {

    internal sealed class UpdateManager {
        private ConcurrentDictionary<GameModel, IFileSystemManager> FileSystems = new ConcurrentDictionary<GameModel, IFileSystemManager>();

        [Inject]
        public IConfigurationManager ConfigurationManager {
            get; set;
        }

        public void Initialize() {
            // nothing to do here
        }

        private IFileSystemManager GetFileSystem(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            IFileSystemManager fileSystem;
            if (FileSystems.TryGetValue(model, out fileSystem)) {
                return fileSystem;
            }
            fileSystem = App.Kernel.Get<IFileSystemManager>();
            FileSystems.TryAdd(model, fileSystem);
            fileSystem.WriteStatusChanged += (s, e) => {
                OnStatusChanged(UpdateStatusEventArgs.Stage.INSTALLING, 1, 1, e.FileNumber, e.FileCount, 0, 1);
            };
            return fileSystem;
        }

        public bool ImportPackages(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            if (Directory.Exists(ConfigurationManager.GetImportPath(model))) {
                IFileSystemManager fs = GetFileSystem(model);
                bool IsOpened = false;
                try {
                    IsOpened = fs.Open(FileAccess.Write, 16, ConfigurationManager.GetHFPath(model), ConfigurationManager.GetPFPath(model));
                } catch {
                    IsOpened = false;
                }

                if (!IsOpened) {
                    OnFileSystemOpenError();
                    return false;
                }
                try {
                    return fs.WriteDirectory(ConfigurationManager.GetImportPath(model), true);
                } finally {
                    fs.Close();
                }
            }
            return true;
        }

        public bool DownloadUpdates(GameModel model, VersionPair versionPair) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            if (versionPair == null) {
                throw new ArgumentException("versionPair argument cannot be null");
            }
            bool updateSuccess = true;
            double downloadedContentLenght = 0;
            double WholeContentLength = 0;

            Dictionary<int, double> contentLenght = new Dictionary<int, double>();
            try {
                for (int i = versionPair.Local + 1; i <= versionPair.Remote; i++) {
                    double patchSize = GetFileLength(new Uri(string.Format(ConfigurationManager.GetConfiguration(model).PatchRemoteURL, i)));
                    WholeContentLength += patchSize;
                    contentLenght.Add(i, patchSize);
                }
            } catch (WebException) {
                return false;
            }

            for (int i = versionPair.Local + 1; i <= versionPair.Remote; i++) {
                Uri patchUri = new Uri(string.Format(ConfigurationManager.GetConfiguration(model).PatchRemoteURL, i));
                string packageFile = Path.Combine(ConfigurationManager.GetGamePath(model), string.Format("UPDATE{0}.zip", i));

                OnStatusChanged(UpdateStatusEventArgs.Stage.DOWNLOADING, i, versionPair.Remote, downloadedContentLenght, WholeContentLength, 0, 100);
                double CurrentContentLength = 0;
                if (!contentLenght.TryGetValue(i, out CurrentContentLength)) {
                    updateSuccess = false;
                    break;
                }

                int downloadAttempts = 5;
                bool patchSuccess = false;
                while (downloadAttempts > 0 && !patchSuccess) {
                    try {
                        if (File.Exists(packageFile)) {
                            File.Delete(packageFile);
                        }
                    } catch {
                        updateSuccess = false;
                        break;
                    }

                    using (WebClientEx webClient = new WebClientEx()) {
                        DownloadProgressChangedEventHandler progressChangedEventHandler = (s, e) => {
                            double dataReceived = (e.BytesReceived / (1024.0 * 1024.0));
                            double dataTotal = (e.TotalBytesToReceive / (1024.0 * 1024.0));
                            OnStatusChanged(UpdateStatusEventArgs.Stage.DOWNLOADING,
                                i, versionPair.Remote,
                                downloadedContentLenght + e.BytesReceived, WholeContentLength,
                                dataReceived, dataTotal);
                        };

                        webClient.DownloadProgressChanged += progressChangedEventHandler;
                        try {
                            webClient.DownloadFileAsync(patchUri, packageFile);
                            while (webClient.IsBusy) {
                                System.Threading.Thread.Sleep(100);
                            }
                            downloadedContentLenght += CurrentContentLength;
                        } catch {
                            downloadAttempts--;
                            continue;
                        } finally {
                            webClient.DownloadProgressChanged -= progressChangedEventHandler;
                        }
                    }
                    if (!ConfigurationManager.CheckUpdateAccess(model)) {
                        updateSuccess = false;
                        break;
                    }
                    if (ExtractUpdate(i, versionPair.Remote,
                        downloadedContentLenght, WholeContentLength,
                        packageFile, ConfigurationManager.GetGamePath(model), true)) {
                        try {
                            string versionFile = ConfigurationManager.GetLocalVersionFile(model);
                            string directory = Path.GetDirectoryName(versionFile);
                            if (!Directory.Exists(directory)) {
                                Directory.CreateDirectory(directory);
                            }
                            File.WriteAllLines(versionFile, new string[] { "[VERSION]", "version=" + i.ToString() });
                        } catch {
                            updateSuccess = false;
                            break;
                        }
                        patchSuccess = true;
                    }
                    downloadAttempts--;
                }
                if (!patchSuccess) {
                    updateSuccess = false;
                    break;
                }
            }
            return updateSuccess;
        }

        private bool ExtractUpdate(int updateNumber, int updateMaxNumber,
            double progress, double maxProgress,
            string archiveFilenameIn, string outFolder, bool DeleteAfterExtract) {
            try {
                using (var zf = new ZipFile(archiveFilenameIn)) {
                    int zEntryNumber = 1;
                    foreach (ZipEntry zipEntry in zf) {
                        OnStatusChanged(UpdateStatusEventArgs.Stage.EXTRACTING, updateNumber, updateMaxNumber, progress, maxProgress, zEntryNumber, zf.Count);
                        if (!zipEntry.IsFile) {
                            continue;
                        }
                        byte[] buffer = new byte[4096];
                        Stream zipStream = zf.GetInputStream(zipEntry);
                        string fullZipToPath = Path.Combine(outFolder, zipEntry.Name);
                        string directoryName = Path.GetDirectoryName(fullZipToPath);
                        if (directoryName.Length > 0) {
                            Directory.CreateDirectory(directoryName);
                        }
                        using (FileStream streamWriter = File.Create(fullZipToPath)) {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }
                        zEntryNumber++;
                    }
                }
            } catch (ZipException) {
                return false;
            }

            if (DeleteAfterExtract) {
                try {
                    File.Delete(archiveFilenameIn);
                } catch {
                }
            }
            return true;
        }

        public VersionPair CheckUpdates(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            string versionFile = ConfigurationManager.GetLocalVersionFile(model);
            if (File.Exists(ConfigurationManager.GetLocalVersionFile(model))) {
                int verCurrent = -1;
                int verRemote = -1;

                using (StreamReader streamReader = new StreamReader(versionFile)) {
                    verCurrent = GetVersion(streamReader.ReadToEnd());
                }

                using (WebClientEx webClient = new WebClientEx()) {
                    string result;
                    try {
                        result = webClient.DownloadString(ConfigurationManager.GetConfiguration(model).VersionRemoteURL);
                        verRemote = GetVersion(result);
                    } catch {
                        return null;
                    }

                    if (verRemote < 0 || verCurrent < 0) {
                        return null;
                    }
                    return new VersionPair(verCurrent, verRemote);
                }
            }
            return null;
        }

        /// <summary> Parse version file (like vGDMO.ini) </summary>
        /// <param name="text">Version file content</param>
        /// <returns> Version (integer) or -1 if version not found </returns>
        private static int GetVersion(string text) {
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
        private static double GetFileLength(Uri url) {
            System.Net.WebRequest req = WebClientEx.CreateHTTPRequest(url);
            req.Method = "HEAD";
            double ContentLength = 0;
            using (System.Net.WebResponse resp = req.GetResponse()) {
                double.TryParse(resp.Headers.Get("Content-Length"), out ContentLength);
            }
            return ContentLength;
        }

        #region Event handlers

        public event BaseEventHandler FileSystemOpenError;

        public event UpdateStatusEventHandler StatusChanged;

        private void OnFileSystemOpenError() {
            if (FileSystemOpenError != null) {
                FileSystemOpenError(this, BaseEventArgs.Empty);
            }
        }

        private void OnStatusChanged(UpdateStatusEventArgs.Stage stage,
            int currentPatch, int maxPatch,
            double progress, double maxProgress,
            double summaryProgress, double summaryMaxProgress) {
            if (StatusChanged != null) {
                StatusChanged(this, new UpdateStatusEventArgs(stage, currentPatch, maxPatch, progress, maxProgress, summaryProgress, summaryMaxProgress));
            }
        }

        #endregion Event handlers
    }
}