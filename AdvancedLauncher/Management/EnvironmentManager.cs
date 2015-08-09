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
using System.Security.Permissions;
using System.Windows;
using System.Xml.Serialization;
using AdvancedLauncher.Management.Internal;
using AdvancedLauncher.Model.Protected;
using AdvancedLauncher.Providers;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Events.Proxy;
using MahApps.Metro;
using Ninject;

namespace AdvancedLauncher.Management {

    public class EnvironmentManager : CrossDomainObject, IEnvironmentManager {
        private const string SETTINGS_FILE = "Settings.xml";
        private const string LOCALE_DIR = "Languages";
        private const string RESOURCE_DIR = "Resources";
        public const string PLUGINS_DIR = "Plugins";
        private const string KBLC_SERVICE_EXECUTABLE = "KBLCService.exe";
        private const string NTLEA_EXECUTABLE = "ntleas.exe";

        [Inject]
        public ILanguageManager LanguageManager {
            get;
            set;
        }

        private Settings _Settings;

        public Settings Settings {
            get {
                return _Settings;
            }
        }

        #region Environment Properties

        public string AppPath {
            get;
            private set;
        }

        public string AppDataPath {
            get;
            private set;
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

        public string LanguagesPath {
            get;
            private set;
        }

        public string Resources3rdPath {
            get;
            private set;
        }

        public string ResourcesPath {
            get;
            private set;
        }

        public string PluginsPath {
            get;
            private set;
        }

        public string DatabaseFile {
            get {
                return Path.Combine(AppDataPath, "MainDatabase.sdf");
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
            AppPath = System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            AppDataPath = InitFolder(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), Path.Combine("GoldRenard", "AdvancedLauncher"));
            LanguagesPath = InitFolder(AppPath, LOCALE_DIR);
            Resources3rdPath = InitFolder(AppDataPath, RESOURCE_DIR);
            ResourcesPath = InitFolder(AppPath, RESOURCE_DIR);
            PluginsPath = InitFolder(AppPath, PLUGINS_DIR);

            AppDomain.CurrentDomain.SetData("DataDirectory", AppDataPath);

            // Initialize ProtectedSettings entity
            ProtectedSettings ProtectedSettings = null;

            if (File.Exists(SettingsFile)) {
                try {
                    ProtectedSettings = DeSerializeSettings(SettingsFile);
                } catch (Exception) {
                    // fall down and recreate settings file
                }
            }
            bool createSettingsFile = ProtectedSettings == null;
            if (createSettingsFile) {
                ProtectedSettings = new ProtectedSettings();
            }

            ApplyAppTheme(ProtectedSettings);
            ApplyProxySettings(ProtectedSettings);
            InitializeSafeSettings(ProtectedSettings);

            LanguageManager LM = LanguageManager as LanguageManager;
            if (LM != null) {
                Settings.LanguageFile = LM.Initialize(InitFolder(AppPath, LOCALE_DIR), _Settings.LanguageFile);
            }

            if (createSettingsFile) {
                Save();
            }
        }

        private void InitializeSafeSettings(ProtectedSettings settings) {
            _Settings = new Settings();
            _Settings.AppTheme = settings.AppTheme;
            _Settings.LanguageFile = settings.Language;
            _Settings.ThemeAccent = settings.ThemeAccent;
            _Settings.CheckForUpdates = settings.CheckForUpdates;

            _Settings.Profiles = new List<Profile>();
            LoginManager loginManager = App.Kernel.Get<LoginManager>();
            if (settings.Profiles != null) {
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
                    _Settings.Profiles.Add(safeProfile);
                    if (safeProfile.Id == settings.DefaultProfile) {
                        _Settings.DefaultProfile = safeProfile;
                    }
                    loginManager.UpdateCredentials(safeProfile, new LoginData(protectedProfile.Login));
                }
            }
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
            ProxyManager pManager = App.Kernel.Get<ProxyManager>();
            if (settings.Proxy == null) {
                settings.Proxy = new ProxySetting();
            }
            pManager.Initialize(settings.Proxy);
        }

        #endregion Initialization

        public void Save() {
            ProtectedSettings toSave = new ProtectedSettings(Settings);
            toSave.Proxy = new ProxySetting(App.Kernel.Get<ProxyManager>().Settings);
            IProfileManager ProfileManager = App.Kernel.Get<IProfileManager>();
            if (ProfileManager.DefaultProfile != null) {
                toSave.DefaultProfile = ProfileManager.DefaultProfile.Id;
            }
            if (ProfileManager.Profiles != null) {
                LoginManager loginManager = App.Kernel.Get<LoginManager>();
                foreach (Profile profile in ProfileManager.Profiles) {
                    ProtectedProfile protectedProfile = new ProtectedProfile(profile);
                    LoginData login = loginManager.GetCredentials(profile);
                    if (login != null) {
                        protectedProfile.Login = new LoginData(login);
                    }
                    toSave.Profiles.Add(protectedProfile);
                }
            }
            new FileIOPermission(FileIOPermissionAccess.Write, SettingsFile).Assert();
            XmlSerializer writer = new XmlSerializer(typeof(ProtectedSettings));
            using (var file = new StreamWriter(SettingsFile)) {
                writer.Serialize(file, toSave);
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
            string resource = Path.Combine(ResourcesPath, folder, file);
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

        public void FileSystemLockedProxy(EventProxy<LockedEventArgs> proxy, bool subscribe = true) {
            if (subscribe) {
                FileSystemLocked += proxy.Handler;
            } else {
                FileSystemLocked -= proxy.Handler;
            }
        }

        public void OnClosingLocked(bool IsLocked) {
            if (ClosingLocked != null) {
                ClosingLocked(null, new LockedEventArgs(IsLocked));
            }
        }

        public void ClosingLockedProxy(EventProxy<LockedEventArgs> proxy, bool subscribe = true) {
            if (subscribe) {
                ClosingLocked += proxy.Handler;
            } else {
                ClosingLocked -= proxy.Handler;
            }
        }

        #endregion Event handlers
    }
}