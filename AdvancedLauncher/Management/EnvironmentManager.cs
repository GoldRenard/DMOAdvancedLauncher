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
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using AdvancedLauncher.Model.Config;
using AdvancedLauncher.Model.Protected;
using AdvancedLauncher.Providers;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using MahApps.Metro;
using Ninject;

namespace AdvancedLauncher.Management {

    public class EnvironmentManager : IEnvironmentManager {
        private const string SETTINGS_FILE = "Settings.xml";
        private const string LOCALE_DIR = "Languages";
        private const string RESOURCE_DIR = "Resources";
        private const string KBLC_SERVICE_EXECUTABLE = "KBLCService.exe";
        private const string NTLEA_EXECUTABLE = "ntleas.exe";

        [Inject]
        public IProfileManager ProfileManager {
            get;
            set;
        }

        [Inject]
        public ILanguageManager LanguageManager {
            get;
            set;
        }

        private ISettings _Settings;

        public ISettings Settings {
            get {
                return _Settings;
            }
        }

        #region Environment Properties

        private string _AppPath = null;

        public string AppPath {
            get {
                if (_AppPath == null) {
                    _AppPath = System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
                }
                return _AppPath;
            }
        }

        public string AppDataPath {
            get {
                return InitFolder(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                    Path.Combine("GoldRenard", "AdvancedLauncher"));
            }
        }

        private string _SettingsFile = null;

        public string SettingsFile {
            get {
                if (_SettingsFile == null) {
                    _SettingsFile = Path.Combine(AppDataPath, SETTINGS_FILE);
                }
                return _SettingsFile;
            }
        }

        private string _KBLCFile = null;

        public string KBLCFile {
            get {
                if (_KBLCFile == null) {
                    _KBLCFile = Path.Combine(AppPath, KBLC_SERVICE_EXECUTABLE);
                }
                return _KBLCFile;
            }
        }

        private string _NTLEAFile = null;

        public string NTLEAFile {
            get {
                if (_NTLEAFile == null) {
                    _NTLEAFile = Path.Combine(AppPath, NTLEA_EXECUTABLE);
                }
                return _NTLEAFile;
            }
        }

        private string _LanguagesPath = null;

        public string LanguagesPath {
            get {
                if (_LanguagesPath == null) {
                    _LanguagesPath = InitFolder(AppPath, LOCALE_DIR);
                }
                return _LanguagesPath;
            }
        }

        private string _Resources3rdPath = null;

        public string Resources3rdPath {
            get {
                if (_Resources3rdPath == null) {
                    _Resources3rdPath = InitFolder(AppDataPath, RESOURCE_DIR);
                }
                return _Resources3rdPath;
            }
        }

        private string _ResourcesPath = null;

        public string ResourcesPath {
            get {
                if (_ResourcesPath == null) {
                    _ResourcesPath = InitFolder(AppPath, RESOURCE_DIR);
                }
                return _ResourcesPath;
            }
        }

        #endregion Environment Properties

        #region Configuration properties

        public string LanguageFile {
            get; set;
        }

        public string AppTheme {
            get; set;
        }

        public string ThemeAccent {
            get; set;
        }

        public int DefaultProfile {
            get; set;
        }

        #endregion Configuration properties

        #region Initialization

        public void Initialize() {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDataPath);

            // Initialize ProtectedSettings entity
            ProtectedSettings ProtectedSettings = null;
            if (File.Exists(SettingsFile)) {
                ProtectedSettings = DeSerializeSettings(SettingsFile);
            }
            if (ProtectedSettings == null) {
                ProtectedSettings = new ProtectedSettings();
            }
            if (ProtectedSettings.Profiles == null || ProtectedSettings.Profiles.Count == 0) {
                ProtectedSettings.Profiles = new List<ProtectedProfile>();
                ProtectedSettings.Profiles.Add(new ProtectedProfile());
            }

            if (!File.Exists(SettingsFile)) {
                Save();
            }

            ApplyAppTheme(ProtectedSettings);
            ApplyProxySettings(ProtectedSettings);
            InitializeSafeSettings(ProtectedSettings);

            // init language
            _Settings.LanguageFile = LanguageManager.Initialize(InitFolder(AppPath, LOCALE_DIR), _Settings.LanguageFile);
        }

        private void InitializeSafeSettings(ProtectedSettings settings) {
            _Settings = new Settings();
            _Settings.AppTheme = settings.AppTheme;
            _Settings.ThemeAccent = settings.ThemeAccent;

            ProfileManager.PendingProfiles.Clear();
            LoginManager loginManager = App.Kernel.Get<LoginManager>();
            foreach (ProtectedProfile protectedProfile in settings.Profiles) {
                Profile safeProfile = new Profile();
                safeProfile.Id = protectedProfile.Id;
                safeProfile.Name = protectedProfile.Name;
                safeProfile.ImagePath = protectedProfile.ImagePath;
                safeProfile.KBLCServiceEnabled = protectedProfile.KBLCServiceEnabled;
                safeProfile.UpdateEngineEnabled = protectedProfile.UpdateEngineEnabled;
                safeProfile.LaunchMode = protectedProfile.LaunchMode;
                safeProfile.GameModel = new GameModel(protectedProfile.GameModel);
                safeProfile.News = new NewsData(protectedProfile.News);
                safeProfile.Rotation = new RotationData(protectedProfile.Rotation);
                ProfileManager.PendingProfiles.Add(safeProfile);
                if (safeProfile.Id == settings.DefaultProfile) {
                    ProfileManager.PendingDefaultProfile = safeProfile;
                }
                loginManager.UpdateCredentials(safeProfile, new LoginData(protectedProfile.Login));
            }
            ProfileManager.ApplyChanges();
        }

        private void ApplyAppTheme(ProtectedSettings ProtectedSettings) {
            Tuple<AppTheme, Accent> currentTheme = ThemeManager.DetectAppStyle(Application.Current);
            if (currentTheme == null) {
                return;
            }
            AppTheme appTheme = null;
            Accent themeAccent = null;
            if (ProtectedSettings.AppTheme != null) {
                appTheme = ThemeManager.GetAppTheme(ProtectedSettings.AppTheme);
            }
            if (appTheme == null) {
                appTheme = currentTheme.Item1;
            }
            if (ProtectedSettings.ThemeAccent != null) {
                themeAccent = ThemeManager.GetAccent(ProtectedSettings.ThemeAccent);
            }
            if (themeAccent == null) {
                themeAccent = currentTheme.Item2;
            }
            ProtectedSettings.AppTheme = appTheme.Name;
            ProtectedSettings.ThemeAccent = themeAccent.Name;
            ThemeManager.ChangeAppStyle(Application.Current, themeAccent, appTheme);
        }

        private void ApplyProxySettings(ProtectedSettings settings) {
            ProxySetting proxy = settings.Proxy;
            if (!proxy.IsEnabled) {
                WebClientEx.ProxyConfig = null;
                return;
            }
            ProxyConfiguration.ProxyMode mode = (ProxyConfiguration.ProxyMode)proxy.Mode;
            if (proxy.Authentication && proxy.Credentials.IsCorrect) {
                WebClientEx.ProxyConfig = new ProxyConfiguration(mode, proxy.Host, proxy.Port,
                    proxy.Credentials.User, proxy.Credentials.SecurePassword);
            } else {
                WebClientEx.ProxyConfig = new ProxyConfiguration(mode, proxy.Host, proxy.Port);
            }
        }

        #endregion Initialization

        public void Save() {
            XmlSerializer writer = new XmlSerializer(typeof(ProtectedSettings));
            using (var file = new StreamWriter(SettingsFile)) {
                writer.Serialize(file, Settings);
            }
        }

        private static ProtectedSettings DeSerializeSettings(string filepath) {
            ProtectedSettings settings = new ProtectedSettings();
            if (File.Exists(filepath)) {
                XmlSerializer reader = new XmlSerializer(typeof(ProtectedSettings));
                using (var file = new StreamReader(filepath)) {
                    settings = (ProtectedSettings)reader.Deserialize(file);
                }
            }
            return settings;
        }

        public string ResolveResource(string folder, string file, string downloadUrl = null) {
            string resource = Path.Combine(InitFolder(ResourcesPath, folder), file);
            string resource3rd = Path.Combine(InitFolder(Resources3rdPath, folder), file);
            if (File.Exists(resource)) {
                return resource;
            }
            if (File.Exists(resource3rd)) {
                return resource3rd;
            }

            if (downloadUrl != null) {
                using (WebClientEx webClient = new WebClientEx()) {
                    try {
                        webClient.DownloadFile(downloadUrl, resource3rd);
                    } catch {
                        // fall down
                    }
                }
            }
            return File.Exists(resource3rd) ? resource3rd : null;
        }

        private static string InitFolder(string root, string name) {
            string folder = Path.Combine(root, name);
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }

        #region Event handlers

        public event LockedChangedHandler FileSystemLocked;

        public event LockedChangedHandler ClosingLocked;

        public void OnFileSystemLocked(bool IsLocked) {
            if (FileSystemLocked != null) {
                FileSystemLocked(null, new LockedEventArgs(IsLocked));
            }
        }

        public void OnClosingLocked(bool IsLocked) {
            if (ClosingLocked != null) {
                ClosingLocked(null, new LockedEventArgs(IsLocked));
            }
        }

        #endregion Event handlers
    }
}