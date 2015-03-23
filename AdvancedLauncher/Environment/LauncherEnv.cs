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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using AdvancedLauncher.Environment.Containers;
using MahApps.Metro;

namespace AdvancedLauncher.Environment {

    public static class LauncherEnv {
        private static string _AppPath = null;
        private const string SETTINGS_FILE = "Settings.xml";
        private const string CONFIG_DIR = "Configs";
        private const string LOCALE_DIR = "Languages";
        private const string RESOURCE_DIR = "Resources";
        public const string KBLC_SERVICE_EXECUTABLE = "KBLCService.exe";
        public const string REMOTE_PATH = "http://renamon.ru/launcher/";
        public static Settings Settings;
        public static System.Net.WebClient WebClient = new System.Net.WebClient();

        public static string AppPath {
            get {
                return _AppPath;
            }
        }

        public static void Load() {
            _AppPath = System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            if (File.Exists(GetSettingsFile())) {
                Settings = DeSerialize(GetSettingsFile());
            }
            if (Settings == null) {
                Settings = new Settings();
            }

            if (string.IsNullOrEmpty(Settings.LanguageFile)) {
                if (LanguageEnv.Load(CultureInfo.CurrentCulture.EnglishName)) {
                    LauncherEnv.Settings.LanguageFile = CultureInfo.CurrentCulture.EnglishName;
                } else {
                    LanguageEnv.Load(LanguageEnv.DefaultName);
                    LauncherEnv.Settings.LanguageFile = LanguageEnv.DefaultName;
                }
            } else {
                if (!LanguageEnv.Load(Settings.LanguageFile)) {
                    LanguageEnv.Load(LanguageEnv.DefaultName);
                    LauncherEnv.Settings.LanguageFile = LanguageEnv.DefaultName;
                }
            }

            if (Settings.Profiles == null || Settings.Profiles.Count == 0) {
                Settings.Profiles = new ObservableCollection<Profile>();
                Settings.Profiles.Add(new Profile());
            }

            if (!File.Exists(GetSettingsFile())) {
                Save();
            }
        }

        public static void LoadTheme() {
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

        public static Settings DeSerialize(string filepath) {
            Settings db = new Settings();
            if (File.Exists(filepath)) {
                StreamReader file = null;
                try {
                    XmlSerializer reader = new XmlSerializer(typeof(Settings));
                    file = new StreamReader(filepath);
                    db = (Settings)reader.Deserialize(file);
                    file.Close();
                } catch {
                    if (file != null) {
                        file.Close();
                    }
                    return db;
                } finally {
                    if (file != null) {
                        file.Close();
                    }
                }
            }
            return db;
        }

        public static void Save() {
            XmlSerializer writer = new XmlSerializer(typeof(Settings));
            StreamWriter file = new StreamWriter(GetSettingsFile());
            writer.Serialize(file, Settings);
            file.Close();
        }

        public static string GetSettingsFile() {
            return Path.Combine(GetConfigsPath(), SETTINGS_FILE);
        }

        public static string GetKBLCFile() {
            return Path.Combine(AppPath, KBLC_SERVICE_EXECUTABLE);
        }

        public static string GetConfigsPath() {
            return InitDir(CONFIG_DIR);
        }

        public static string GetLangsPath() {
            return InitDir(LOCALE_DIR);
        }

        public static string GetResourcesPath() {
            return InitDir(RESOURCE_DIR);
        }

        private static string InitDir(string source) {
            string dir = Path.Combine(AppPath, source);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }
    }
}