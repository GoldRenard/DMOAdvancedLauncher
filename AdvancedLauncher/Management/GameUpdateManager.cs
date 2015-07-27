using System;
using System.Collections.Concurrent;
using System.IO;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Model.Config;
using DMOLibrary;
using DMOLibrary.DMOFileSystem;
using DMOLibrary.Events;
using Ninject;

namespace AdvancedLauncher.Management {
    public class GameUpdateManager : IGameUpdateManager {

        private ConcurrentDictionary<GameModel, DMOFileSystem> FileSystems = new ConcurrentDictionary<GameModel, DMOFileSystem>();

        [Inject]
        public IGameManager GameManager {
            get; set;
        }

        public void Initialize() {
            // nothing to do here
        }

        public DMOFileSystem GetFileSystem(GameModel model) {
            DMOFileSystem fileSystem;
            if (FileSystems.TryGetValue(model, out fileSystem)) {
                return fileSystem;
            }
            fileSystem = new DMOFileSystem();
            FileSystems.TryAdd(model, fileSystem);
            return fileSystem;
        }

        public bool ImportPackages(GameModel model) {
            if (Directory.Exists(GameManager.GetImportPath(model))) {
                DMOFileSystem fs = GetFileSystem(model);
                bool IsOpened = false;
                try {
                    IsOpened = fs.Open(FileAccess.Write, 16, GameManager.GetHFPath(model), GameManager.GetPFPath(model));
                } catch {
                    IsOpened = false;
                }

                if (!IsOpened) {
                    OnFileSystemOpenError();
                    return false;
                }
                fs.WriteStatusChanged += OnWriteStatusChanged;
                bool IsSuccess = false;
                try {
                    IsSuccess = fs.WriteDirectory(GameManager.GetImportPath(model), true);
                } finally {
                    fs.WriteStatusChanged -= OnWriteStatusChanged;
                    fs.Close();
                }
                return IsSuccess;
            }
            return true;
        }

        public VersionPair CheckUpdates(GameModel model) {
            string versionFile = GameManager.GetLocalVersionFile(model);
            if (File.Exists(GameManager.GetLocalVersionFile(model))) {
                int verCurrent = -1;
                int verRemote = -1;

                using (StreamReader streamReader = new StreamReader(versionFile)) {
                    verCurrent = GetVersion(streamReader.ReadToEnd());
                }

                using (WebClientEx webClient = new WebClientEx()) {
                    string result;
                    try {
                        result = webClient.DownloadString(GameManager.GetConfiguration(model).VersionRemoteURL);
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
        public static int GetVersion(string text) {
            string expr = "(version)(=)(\\d+)";
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(expr, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            System.Text.RegularExpressions.Match m = r.Match(text);
            if (m.Success) {
                return Convert.ToInt32(m.Groups[3].ToString());
            }
            return -1;
        }

        #region Event handlers

        public event EventHandler ImportStarted;

        public event EventHandler FileSystemOpenError;

        public event WriteStatusChangedEventHandler WriteStatusChanged;

        private void OnFileSystemOpenError() {
            if (FileSystemOpenError != null) {
                FileSystemOpenError(this, new EventArgs());
            }
        }
        private void OnImportStarted() {
            if (ImportStarted != null) {
                ImportStarted(this, new EventArgs());
            }
        }

        private void OnWriteStatusChanged(object sender, WriteDirectoryEventArgs e) {
            if (WriteStatusChanged != null) {
                WriteStatusChanged(sender, e);
            }
        }

        #endregion
    }
}
