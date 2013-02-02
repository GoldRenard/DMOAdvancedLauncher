// ======================================================================
// GLOBAL DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
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
using Ookii.Dialogs.Wpf;
using DMOLibrary;
using DMOLibrary.DMOWebInfo;

namespace AdvancedLauncher
{
    class SettingsProvider
    {
        static string GAME_PATH_KEY = "Software\\Joymax\\DMO";
        static string GAME_PATH_PARAM = "Path";
        static string SETTINGS_KEY = "Software\\GoldRenard\\GDMO_Launcher";
        static string SETTINGS_TWITTER = "twitter_user";
        static string SETTINGS_ROT_GNAME = "rotation_gname";
        static string SETTINGS_LANG = "lang";
        static string SETTINGS_ROT_GSERV = "rotation_gserv";
        static string SETTINGS_ROT_URATE = "rotation_urate";
        static string SETTINGS_USE_APPLOC = "ALEnabled";
        static string SETTINGS_USE_UPDATE_ENGINE = "UseUpdateEngine";
        static string SETTINGS_1ST_TAB = "first_news_tab";

        public static string APP_PATH = null;
        private static string GAME_PATH_ = null;

        public static string JOYMAX_PATH_INFO_URI = "http://patch.dmo.joymax.com/PatchInfo_GDMO.ini";
        public static string JOYMAX_FILE_PATCH = "http://patch.dmo.joymax.com/GDMO{0}.zip";
        public static string RES_HF_FILE = "\\Resources.hf";
        public static string RES_PF_FILE = "\\Resources.pf";
        public static string LANGS_DIR = "\\Languages\\";
        public static string RES_IMPORT_DIR = "\\Resources\\";
        public static string RES_LIST_FILE = "\\GameResourceList.cfg";

        public static string DEF_LAUNCHER = "\\DMLauncher.exe";
        public static string GAME_APP = "\\GDMO.exe";
        public static string VERSION_FILE = "\\LauncherLib\\vGDMO.ini";
        public static string PACK_HF_FILE = "\\Data\\Pack01.hf";
        public static string PACK_PF_FILE = "\\Data\\Pack01.pf";
        public static string PACK_IMPORT_DIR = "\\Pack01";

        public static string TRANSLATION_FILE;
        public static string TWITTER_USER = "dmo_russian";
        public static string ROTATION_GNAME = "MonolithMesa";
        public static server ROTATION_GSERV = new server() { Id = 1 };
        public static List<server> server_list;
        public static int ROTATION_URATE = 2;
        public static bool USE_APPLOC = false;

        public static bool USE_UPDATE_ENGINE = false;
        public static int FIRST_TAB = 1;

        /// <summary> Loads settings from registry or create with default values </summary>
        public static void LoadSettings()
        {
            APP_PATH = System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);

            RegistryKey read_settings = Registry.CurrentUser.CreateSubKey(SETTINGS_KEY);

            if (read_settings.GetValue(SETTINGS_LANG) == null)
            {
                LanguageProvider.LoadTranslation(CultureInfo.CurrentCulture.EnglishName);
                if (LanguageProvider.LoadTranslation(CultureInfo.CurrentCulture.EnglishName))
                    read_settings.SetValue(SETTINGS_LANG, CultureInfo.CurrentCulture.EnglishName);
                else
                    read_settings.SetValue(SETTINGS_LANG, LanguageProvider.def.LANGUAGE_NAME);
            }
            TRANSLATION_FILE = (string)read_settings.GetValue(SETTINGS_LANG);
            if (!LanguageProvider.LoadTranslation(TRANSLATION_FILE))
                LanguageProvider.LoadTranslation(LanguageProvider.def.LANGUAGE_NAME);

            if (read_settings.GetValue(SETTINGS_TWITTER) == null)
                read_settings.SetValue(SETTINGS_TWITTER, TWITTER_USER);
            TWITTER_USER = (string)read_settings.GetValue(SETTINGS_TWITTER);

            if (read_settings.GetValue(SETTINGS_ROT_GNAME) == null)
                read_settings.SetValue(SETTINGS_ROT_GNAME, ROTATION_GNAME);
            ROTATION_GNAME = (string)read_settings.GetValue(SETTINGS_ROT_GNAME);

            if (read_settings.GetValue(SETTINGS_ROT_GSERV) == null)
                read_settings.SetValue(SETTINGS_ROT_GSERV, ROTATION_GSERV.Id);
            ROTATION_GSERV.Id = (int)read_settings.GetValue(SETTINGS_ROT_GSERV);

            if (read_settings.GetValue(SETTINGS_ROT_URATE) == null)
                read_settings.SetValue(SETTINGS_ROT_URATE, ROTATION_URATE);
            ROTATION_URATE = (int)read_settings.GetValue(SETTINGS_ROT_URATE);
            if (ROTATION_URATE < 1 || ROTATION_URATE > 7)
                ROTATION_URATE = 2;

            if (read_settings.GetValue(SETTINGS_USE_APPLOC) == null)
            {
                bool isALInstalled = File.Exists(Environment.GetEnvironmentVariable("windir") + "\\apppatch\\AppLoc.exe");
                bool isKoreanSupported = false;
                CultureInfo[] cis = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures);
                foreach (CultureInfo ci in cis)
                    if (ci.TwoLetterISOLanguageName == "ko")
                    {
                        isKoreanSupported = true;
                        break;
                    }
                read_settings.SetValue(SETTINGS_USE_APPLOC, isALInstalled && isKoreanSupported, RegistryValueKind.DWord);
            }
            USE_APPLOC = ((int)read_settings.GetValue(SETTINGS_USE_APPLOC) == 1) ? true : false;

            if (read_settings.GetValue(SETTINGS_USE_UPDATE_ENGINE) == null)
                read_settings.SetValue(SETTINGS_USE_UPDATE_ENGINE, USE_UPDATE_ENGINE, RegistryValueKind.DWord);
            USE_UPDATE_ENGINE = ((int)read_settings.GetValue(SETTINGS_USE_UPDATE_ENGINE) == 1) ? true : false;

            if (read_settings.GetValue(SETTINGS_1ST_TAB) == null)
                read_settings.SetValue(SETTINGS_1ST_TAB, 1);
            FIRST_TAB = (int)read_settings.GetValue(SETTINGS_1ST_TAB);

            read_settings.Close();

            DMODatabase db = new DMODatabase(DMO_DB_PATH());
            if (db.OpenConnection())
            {
                server_list = db.GetServers();
                db.CloseConnection();
            }
            GAME_PATH_ = GAME_PATH();
        }

        /// <summary> Saves settings to registry </summary>
        public static void SaveSettings()
        {
            RegistryKey write_settings = Registry.CurrentUser.CreateSubKey(SETTINGS_KEY);
            write_settings.SetValue(SETTINGS_TWITTER, TWITTER_USER);
            write_settings.SetValue(SETTINGS_ROT_GNAME, ROTATION_GNAME);
            write_settings.SetValue(SETTINGS_ROT_GSERV, ROTATION_GSERV.Id);
            write_settings.SetValue(SETTINGS_ROT_URATE, ROTATION_URATE);
            write_settings.SetValue(SETTINGS_LANG, TRANSLATION_FILE);
            write_settings.SetValue(SETTINGS_USE_APPLOC, USE_APPLOC, RegistryValueKind.DWord);
            write_settings.SetValue(SETTINGS_USE_UPDATE_ENGINE, USE_UPDATE_ENGINE, RegistryValueKind.DWord);
            write_settings.SetValue(SETTINGS_1ST_TAB, FIRST_TAB);
            write_settings.Close();
        }

        /// <summary> Provides full path to game directory </summary>
        public static string GAME_PATH()
        {
            if (CheckGameDir(GAME_PATH_, true))
                return GAME_PATH_;

            RegistryKey read_settings = Registry.CurrentUser.CreateSubKey(GAME_PATH_KEY);
            GAME_PATH_ = (string)read_settings.GetValue(GAME_PATH_PARAM);
            read_settings.Close();

            if (!CheckGameDir(GAME_PATH_, true))
                GAME_PATH_ = SelectGameDir(false);
            return GAME_PATH_;
        }

        /// <summary> Provides full path DMO SQLite database </summary>
        public static string DMO_DB_PATH()
        {
            return SettingsProvider.APP_PATH + "\\DMODatabase.sqlite";
        }

        /// <summary> Selects path to game directory </summary>
        /// <param name="AllowCancel">Allow to cancel that operation. If <see langword="false"/> - user MUST select directory without permission to cancel.</param>
        /// <returns> New path to game directory or old if was pressed Cancel </returns>
        public static string SelectGameDir(bool AllowCancel)
        {
            if (Environment.OSVersion.Version.Major == 5)
            {
                System.Windows.Forms.FolderBrowserDialog FBrowser = new System.Windows.Forms.FolderBrowserDialog();
                FBrowser.Description = LanguageProvider.strings.SELECT_GAME_DIR;
                FBrowser.ShowNewFolderButton = false;
                while (true)
                {
                    if (FBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        GAME_PATH_ = FBrowser.SelectedPath;
                        RegistryKey read_settings = Registry.CurrentUser.CreateSubKey(GAME_PATH_KEY);
                        read_settings.SetValue(GAME_PATH_PARAM, GAME_PATH_, RegistryValueKind.String);
                        read_settings.Close();
                    }
                    else if (AllowCancel)
                        break;
                    if (CheckGameDir(GAME_PATH_, false))
                        break;
                }
            }
            else
            {
                VistaFolderBrowserDialog FBrowser = new VistaFolderBrowserDialog();
                FBrowser.Description = LanguageProvider.strings.SELECT_GAME_DIR;
                FBrowser.ShowNewFolderButton = false;
                while (true)
                {
                    if (FBrowser.ShowDialog() == true)
                    {
                        GAME_PATH_ = FBrowser.SelectedPath;
                        RegistryKey read_settings = Registry.CurrentUser.CreateSubKey(GAME_PATH_KEY);
                        read_settings.SetValue(GAME_PATH_PARAM, GAME_PATH_, RegistryValueKind.String);
                        read_settings.Close();
                    }
                    else if (AllowCancel)
                        break;
                    if (CheckGameDir(GAME_PATH_, false))
                        break;
                }
            }
            return GAME_PATH_;
        }

        /// <summary> Checks and selects game directory if game not exists </summary>
        /// <param name="dir">Directory for checking.</param>
        /// <param name="isSilent">If <see langword="false"/> then will show error if wrong directory</param>
        /// <returns> New path to game directory or old if was pressed Cancel </returns>
        private static bool CheckGameDir(string dir, bool isSilent)
        {
            if (dir == null)
                return false;
            if (!File.Exists(dir + VERSION_FILE) || !File.Exists(dir + DEF_LAUNCHER) || !File.Exists(dir + GAME_APP))
            {
                if (!isSilent)
                    Utils.MSG_ERROR(LanguageProvider.strings.SELECT_GAME_DIR_WRONG);
                return false;
            }
            return true;
        }
    }
}
