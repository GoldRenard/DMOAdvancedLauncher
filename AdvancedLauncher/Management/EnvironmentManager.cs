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
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Model.Config;
using AdvancedLauncher.Model.Events;
using DMOLibrary;
using MahApps.Metro;

namespace AdvancedLauncher.Management {

    internal class EnvironmentManager : IEnvironmentManager {
        private const string SETTINGS_FILE = "Settings.xml";
        private const string LOCALE_DIR = "Languages";
        private const string RESOURCE_DIR = "Resources";
        private const string KBLC_SERVICE_EXECUTABLE = "KBLCService.exe";
        private const string NTLEA_EXECUTABLE = "ntleas.exe";

        #region Properties

        public Settings _Settings = null;

        public Settings Settings {
            get {
                return _Settings;
            }
        }

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

        #endregion Properties

        public void Initialize() {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDataPath);
            if (File.Exists(SettingsFile)) {
                _Settings = DeSerializeSettings(SettingsFile);
            }
            if (Settings == null) {
                _Settings = new Settings();
            }

            ApplyProxySettings(Settings);
            Settings.ConfigurationChanged += (s, e) => {
                ApplyProxySettings((Settings)s);
            };

            if (Settings.Profiles == null || Settings.Profiles.Count == 0) {
                Settings.Profiles = new List<Profile>();
                Settings.Profiles.Add(new Profile());
            }

            if (!File.Exists(SettingsFile)) {
                Save();
            }

            Tuple<AppTheme, Accent> currentTheme = ThemeManager.DetectAppStyle(Application.Current);
            if (currentTheme == null) {
                return;
            }
            AppTheme appTheme = null;
            Accent themeAccent = null;
            if (Settings.AppTheme != null) {
                appTheme = ThemeManager.GetAppTheme(Settings.AppTheme);
            }
            if (appTheme == null) {
                appTheme = currentTheme.Item1;
            }
            if (Settings.ThemeAccent != null) {
                themeAccent = ThemeManager.GetAccent(Settings.ThemeAccent);
            }
            if (themeAccent == null) {
                themeAccent = currentTheme.Item2;
            }
            Settings.AppTheme = appTheme.Name;
            Settings.ThemeAccent = themeAccent.Name;
            ThemeManager.ChangeAppStyle(Application.Current, themeAccent, appTheme);
        }

        public void Save() {
            XmlSerializer writer = new XmlSerializer(typeof(Settings));
            using (var file = new StreamWriter(SettingsFile)) {
                writer.Serialize(file, Settings);
            }
        }

        private void ApplyProxySettings(Settings settings) {
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

        private static Settings DeSerializeSettings(string filepath) {
            Settings settings = new Settings();
            if (File.Exists(filepath)) {
                XmlSerializer reader = new XmlSerializer(typeof(Settings));
                using (var file = new StreamReader(filepath)) {
                    settings = (Settings)reader.Deserialize(file);
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