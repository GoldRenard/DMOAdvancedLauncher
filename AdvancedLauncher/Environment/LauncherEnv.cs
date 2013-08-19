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
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Globalization;
using AdvancedLauncher.Service;
using System.Xml.Serialization;
using AdvancedLauncher.Environment.Containers;

namespace AdvancedLauncher.Environment
{
    public static class LauncherEnv
    {
        private static string _AppPath = null;
        public static string AppPath { get { return _AppPath; } }
        const string sFile = "Settings.xml";
        const string CONF_DIR = "Configs";
        const string LANGS_DIR = "Languages";
        const string RES_DIR = "Resources";
        public const string RemotePath = "http://renamon.ru/launcher/";
        public static Settings Settings;
        public static System.Net.WebClient WebClient = new System.Net.WebClient();

        public static void Load()
        {
            _AppPath = System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            if (File.Exists(GetSettingsFile()))
                Settings = DeSerialize(GetSettingsFile());
            if (Settings == null)
                Settings = new Settings();

            if (string.IsNullOrEmpty(Settings.LangFile))
            {
                if (LanguageEnv.Load(CultureInfo.CurrentCulture.EnglishName))
                    LauncherEnv.Settings.LangFile = CultureInfo.CurrentCulture.EnglishName;
                else
                {
                    LanguageEnv.Load(LanguageEnv.DefaultName);
                    LauncherEnv.Settings.LangFile = LanguageEnv.DefaultName;
                }
            }
            else
            {
                if (!LanguageEnv.Load(Settings.LangFile))
                {
                    LanguageEnv.Load(LanguageEnv.DefaultName);
                    LauncherEnv.Settings.LangFile = LanguageEnv.DefaultName;
                }
            }

            if (Settings.pCollection == null || Settings.pCollection.Count == 0)
            {
                Settings.pCollection = new ObservableCollection<Profile>();
                Settings.pCollection.Add(new Profile());
            }

            if (!File.Exists(GetSettingsFile()))
                Save();
        }

        public static Settings DeSerialize(string filepath)
        {
            Settings db = new Settings();
            if (File.Exists(filepath))
            {
                StreamReader file = null;
                try
                {
                    XmlSerializer reader = new XmlSerializer(typeof(Settings));
                    file = new StreamReader(filepath);
                    db = (Settings)reader.Deserialize(file);
                    file.Close();
                }
                catch
                {
                    if (file != null)
                        file.Close();
                    return db;
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
            return db;
        }

        public static void Save()
        {
            XmlSerializer writer = new XmlSerializer(typeof(Settings));
            StreamWriter file = new StreamWriter(GetSettingsFile());
            writer.Serialize(file, Settings);
            file.Close();
        }

        public static string GetSettingsFile()
        {
            return Path.Combine(GetConfigsPath(), sFile);
        }

        public static string GetConfigsPath()
        {
            string dir = Path.Combine(AppPath, CONF_DIR);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        public static string GetLangsPath()
        {
            string dir = Path.Combine(AppPath, LANGS_DIR);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        public static string GetResourcesPath()
        {
            string dir = Path.Combine(AppPath, RES_DIR);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }
    }
}