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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Globalization;
using DMOLibrary;

namespace AdvancedLauncher
{
    class SettingsProvider
    {
        public static string APP_PATH = null;

        static string SETTINGS_KEY = "Software\\GoldRenard\\GDMO_Launcher";
        static string SETTINGS_LANG = "Language";

        public static string RES_HF_FILE = "\\Resources.hf";
        public static string RES_PF_FILE = "\\Resources.pf";
        public static string LANGS_DIR = "\\Languages\\";
        public static string RES_IMPORT_DIR = "\\Resources\\";
        public static string RES_LIST_FILE = "\\ResourceList_{0}.cfg";

        public static string TRANSLATION_FILE;

        /// <summary> Loads settings from registry or create with default values </summary>
        public static void LoadSettings()
        {
            APP_PATH = System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);

            RegistryKey read_settings = Registry.CurrentUser.CreateSubKey(SETTINGS_KEY);

            if (read_settings.GetValue(SETTINGS_LANG) == null)
            {
                if (LanguageProvider.LoadTranslation(CultureInfo.CurrentCulture.EnglishName))
                    read_settings.SetValue(SETTINGS_LANG, CultureInfo.CurrentCulture.EnglishName);
                else
                  read_settings.SetValue(SETTINGS_LANG, LanguageProvider.Translation.DEF_LANG_NAME);
            }
            TRANSLATION_FILE = (string)read_settings.GetValue(SETTINGS_LANG);
            if (!LanguageProvider.LoadTranslation(TRANSLATION_FILE))
              LanguageProvider.LoadTranslation(LanguageProvider.Translation.DEF_LANG_NAME);

            read_settings.Close();
        }

        /// <summary> Saves settings to registry </summary>
        public static void SaveSettings()
        {
            RegistryKey write_settings = Registry.CurrentUser.CreateSubKey(SETTINGS_KEY);
            write_settings.SetValue(SETTINGS_LANG, TRANSLATION_FILE);
            write_settings.Close();
        }
    }
}
