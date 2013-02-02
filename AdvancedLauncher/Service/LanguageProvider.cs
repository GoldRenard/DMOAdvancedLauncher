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
using System.Xml.Serialization;
using System.IO;

namespace AdvancedLauncher
{
    public class LanguageProvider
    {
        public delegate void ChangedEventHandler();
        public static event ChangedEventHandler Languagechanged;
        public static void OnChanged()
        {
            if (Languagechanged != null)
                Languagechanged();
        }

        public struct Translation
        {
            //SHARED
            public string LANGUAGE_NAME;
            public string ERROR;
            public string CANT_OPEN_LINK;
            public string BROWSE;
            public string CLOSE;
            public string CANCEL;
            public string APPLY;
            public string PLEASE_WAIT;
            public string NEED_CLOSE_GAME;
            public string NEED_CLOSE_DEF_LAUNCHER;
            public string GAME_FILES_IN_USE;

            //MainWindow
            public string MAIN_NEWS_TAB;
            public string MAIN_GALLERY_TAB;
            public string MAIN_COMMUNITY_TAB;
            public string MAIN_CUSTOMIZATION_TAB;

            //MainPage
            public string MAIN_START_GAME;
            public string MAIN_START_WAITING;
            public string MAIN_UPDATE_GAME;
            public string MAIN_SHOW_TAB;

            //Game Update
            public string UPDATE_CANT_GET_GAME_VERSION;
            public string UPDATE_CANT_CONNECT_TO_JOYMAX;
            public string UPDATE_DOWNLOADING;
            public string UPDATE_EXTRACTING;
            public string UPDATE_INSTALLING;

            //NEWS
            public string NEWS_TYPE_NOTICE;
            public string NEWS_TYPE_EVENT;
            public string NEWS_TYPE_PATCH;
            public string NEWS_CANT_GET_TWITLIST;
            public string NEWS_PUBLICATION_DATE;
            public string NEWS_READ_MORE;

            //DIGIROTATION
            public string ROT_LOADING;
            public string ROT_TAMER;
            public string ROT_LEVEL;
            public string ROT_ERRMSG1;
            public string ROT_ERRMSG2;

            //GALLERY
            public string GAL_HELP;
            public string GAL_NOSCREENSHOTS;
            public string GAL_CANT_OPEN;

            //Community
            public string COMM_SHOWTAB;
            public string COMM_TAMERS_TAB;
            public string COMM_DIGIMONS_TAB;

            public string COMM_SELECT_GUILD;
            public string COMM_CB_DETAILED;
            public string COMM_BTN_SEARCH;

            public string COMM_INFO;
            public string COMM_GMASTER;
            public string COMM_GBEST;
            public string COMM_GRANK;
            public string COMM_GREP;
            public string COMM_TAMER_COUNT;
            public string COMM_DIGIMON_COUNT;

            public string COMM_TB_GUILD_NAME;
            public string COMM_TB_EMPTY_MSG;
            public string COMM_TB_INCORRECT;

            public string COMM_STATUS_GUILD_SEARCHING;
            public string COMM_STATUS_TAMER_GETTING;

            public string COMM_LHEADER_TYPE;
            public string COMM_LHEADER_LEVEL;
            public string COMM_LHEADER_RANKING;
            public string COMM_LHEADER_TAMER;
            public string COMM_LHEADER_PARTNER;
            public string COMM_LHEADER_MERCENARY;
            public string COMM_LHEADER_NAME;
            public string COMM_LHEADER_SIZE;

            public string COMM_CANT_GET_GUILD;
            public string COMM_CANT_CONNECT_TO_DB;
            public string COMM_GUILD_NOT_FOUND;
            public string COMM_GUILD_NO_CONNECTION;

            //PERSONALISATION
            public string CUST_SELECT_PICTURE; // Select
            public string CUST_SELECT_MESSAGE; // SelectMessage
            public string CUST_CURRENT_PICTURE; // Current
            public string CUST_SAVE_PICTURE; // Save
            public string CUST_SELECT_TITLE;
            public string CUST_SAVE_TITLE;
            public string CUST_CANT_OPEN_FS;
            public string CUST_WRONG_TGA;
            public string CUST_CANT_SAVE;
            public string CUST_CANT_WRITE;
            public string CUST_GAME_FILE_NOT_FOUND;
            public string CUST_RES_FILE_NOT_FOUND;

            //SETTINGS
            public string SELECT_GAME_DIR;
            public string SELECT_GAME_DIR_WRONG;
            public string SETTINGS_CANT_USE_APPLOC;
            public string SETTINGS_CANT_USE_APPLOC_CAPTION;
            public string SETTINGS_APPLOC_ISNT_INSTALLED;
            public string SETTINGS_ALP_ISNT_INSTALLED;
            public string SETTINGS_QUESTION_DOWNLOAD_NOW;
            public string SETTINGS_NEED_RESTART;
            public string SETTINGS_NEED_RESTART_CAPTION;

            public string SETTINGS_LANG;
            public string SETTINGS_ROTATION_GUILD;
            public string SETTINGS_ROT_UPD_FREQ;

            public string SETTINGS_NEWS;
            public string SETTINGS_TWITTER_USER;
            public string SETTINGS_START_NEWS;

            public string SETTINGS_GAME;
            public string SETTINGS_GAME_PATH;
            public string SETTINGS_USE_UPDATER;
            public string SETTINGS_USE_APPLOCALE;
            public string SETTINGS_APPLOCALE_HELP;

            //APPLOCALE
            public string APPLAUNCHER_APPLOCALE_NOT_FOUND_MSG;
            public string APPLAUNCHER_APPLOCALE_NOT_FOUND_TITLE;
            public string APPLAUNCHER_APPLOCALE_CANT_PATCH;
            public string APPLAUNCHER_CANT_EXECURE;

            //ABOUT
            public string ABOUT_VERSION;
            public string ABOUT_DEV;
            public string ABOUT_DES;
            public string ABOUT_PROJECTS;

            //LAUNCHER UPDATER
            public string LAUNCHER_UPDATE_NEW_AVAILABLE;
            public string LAUNCHER_UPDATE_NEW_AVAILABLE_CAPTION;
            public string LAUNCHER_Q_UPDATE_WANT_DOWNLOAD;

            public bool IsEmpty()
            {
                if (LANGUAGE_NAME == null & ERROR == null & ABOUT_VERSION == null)
                { return true; }
                else
                { return false; }
            }
        }
        public static Translation def = new Translation()
        {
            //SHARED
            LANGUAGE_NAME = "English (Default)",
            ERROR = "Error",
            CANT_OPEN_LINK = "Can't open link. Try change default browser. Error: ",
            BROWSE = "Browse",
            CLOSE = "Close",
            CANCEL = "Cancel",
            APPLY = "Apply",
            PLEASE_WAIT = "Please wait...",
            NEED_CLOSE_GAME = "Please close the game.",
            NEED_CLOSE_DEF_LAUNCHER = "Please close the default launcher.",
            GAME_FILES_IN_USE = "Don't have access to the required game files. Please close all programs that use them.",

            //MainPage
            MAIN_SHOW_TAB = "NEWS:",
            MAIN_START_GAME = "GAME START",
            MAIN_START_WAITING = "WAITING...",
            MAIN_UPDATE_GAME = "UPDATE GAME",

            //MainWindow
            MAIN_NEWS_TAB = "NEWS",
            MAIN_GALLERY_TAB = "GALLERY",
            MAIN_COMMUNITY_TAB = "COMMUNITY",
            MAIN_CUSTOMIZATION_TAB = "CUSTOMISATION",

            //Game Update
            UPDATE_CANT_GET_GAME_VERSION = "Unable to get the version of the game",
            UPDATE_CANT_CONNECT_TO_JOYMAX = "Can't connect to the JOYMAX servers.",
            UPDATE_DOWNLOADING = "Downloading update {0}... [{1:0.00}MB / {2:0.00}MB]",
            UPDATE_EXTRACTING = "Unpacking update {0}... [{1} / {2}]",
            UPDATE_INSTALLING = "Installing updates... [{0} / {1}]",

            //NEWS
            NEWS_TYPE_EVENT = "event",
            NEWS_TYPE_NOTICE = "notice",
            NEWS_TYPE_PATCH = "patch",
            NEWS_CANT_GET_TWITLIST = "Unable to get a list of tweets",
            NEWS_PUBLICATION_DATE = "Publication date",
            NEWS_READ_MORE = "Read more...",

            //DIGIROTATION
            ROT_LOADING = "Loading the list of Digimons",
            ROT_TAMER = "Tamer",
            ROT_LEVEL = "level",
            ROT_ERRMSG1 = "An error has occurred!",
            ROT_ERRMSG2 = "Check your Internet connection.",

            //GALLERY
            GAL_HELP = "CLICK TWICE ON THE SCREENSHOT FOR OPENING",
            GAL_NOSCREENSHOTS = "THERE IS NO SCREENSHOTS",
            GAL_CANT_OPEN = "Can't open the image. Error: ",

            //COMMUNITY
            COMM_SHOWTAB = "SHOW:",
            COMM_TAMERS_TAB = "TAMERS",
            COMM_DIGIMONS_TAB = "DIGIMONS",

            COMM_SELECT_GUILD = "What guild you are looking for:",
            COMM_CB_DETAILED = "Receive detailed data (long)",
            COMM_BTN_SEARCH = "Search a guild",

            COMM_INFO = "Information:",
            COMM_GMASTER = "Master:",
            COMM_GBEST = "Best:",
            COMM_GRANK = "Ranking:",
            COMM_GREP = "Reputation:",
            COMM_TAMER_COUNT = "Quantity of tamers:",
            COMM_DIGIMON_COUNT = "Quantity of digimons:",

            COMM_TB_GUILD_NAME = "Guild name",
            COMM_TB_EMPTY_MSG = "Please enter the name of the guild!",
            COMM_TB_INCORRECT = "Guild name contains invalid characters!",
            COMM_STATUS_GUILD_SEARCHING = "Searching the guild",
            COMM_STATUS_TAMER_GETTING = "Getting tamer: {0}",

            COMM_LHEADER_LEVEL = "Level",
            COMM_LHEADER_MERCENARY = "Mercenary",
            COMM_LHEADER_NAME = "Name",
            COMM_LHEADER_PARTNER = "Partner",
            COMM_LHEADER_RANKING = "Ranking",
            COMM_LHEADER_SIZE = "Size",
            COMM_LHEADER_TAMER = "Tamer",
            COMM_LHEADER_TYPE = "Type",

            COMM_CANT_GET_GUILD = "Can't get information about the guild",
            COMM_CANT_CONNECT_TO_DB = "Can't connect to local database",
            COMM_GUILD_NOT_FOUND = "This guild is not found",
            COMM_GUILD_NO_CONNECTION = "Unable to get information, check your internet connection",

            //PERSONALISATION
            CUST_SELECT_PICTURE = "Select Image:",
            CUST_SELECT_MESSAGE = "Click here to select the Image",
            CUST_CURRENT_PICTURE = "Current Image:",
            CUST_SAVE_PICTURE = "Save the current Image",
            CUST_SELECT_TITLE = "Select TGA-Image",
            CUST_SAVE_TITLE = "Save Image",
            CUST_CANT_OPEN_FS = "Failed to get access to the files of game :(",
            CUST_WRONG_TGA = "It is not a TGA-image!",
            CUST_CANT_SAVE = "Unable to save the Image.",
            CUST_CANT_WRITE = "Unable to write the Image to the game!",
            CUST_GAME_FILE_NOT_FOUND = "The file \"{0}\" was not found or launcher don't have access to the game resources.",
            CUST_RES_FILE_NOT_FOUND = "File of the list of resources \"{0}\" was not found or is empty.",

            //APPLOCALE
            APPLAUNCHER_APPLOCALE_NOT_FOUND_MSG = "Microsoft AppLocale was not found on this computer. Do you want to download it now?",
            APPLAUNCHER_APPLOCALE_NOT_FOUND_TITLE = "Microsoft AppLocale was not found",
            APPLAUNCHER_APPLOCALE_CANT_PATCH = "Microsoft AppLocale patching failed.",
            APPLAUNCHER_CANT_EXECURE = "Can't execute program",

            //SETTINGS
            SELECT_GAME_DIR = "Please specify the directory where the game is installed",
            SELECT_GAME_DIR_WRONG = "You must specify the correct folder with the game.",
            SETTINGS_CANT_USE_APPLOC = "Can't run the game through AppLocale for the following reasons:",
            SETTINGS_CANT_USE_APPLOC_CAPTION = "Run through AppLocale is impossible now",
            SETTINGS_APPLOC_ISNT_INSTALLED = " - Not installed Microsoft AppLocale Utility",
            SETTINGS_ALP_ISNT_INSTALLED = " - Support of East Asian languages is not installed",
            SETTINGS_QUESTION_DOWNLOAD_NOW = "Open the download page and instructions to correct these errors right now?",
            SETTINGS_NEED_RESTART = "Some changes will take effect when you restart the launcher.",
            SETTINGS_NEED_RESTART_CAPTION = "Need restart",

            SETTINGS_LANG = "Language:",
            SETTINGS_ROTATION_GUILD = "Guild for Digimon Rotation:",
            SETTINGS_ROT_UPD_FREQ = "Update frequency (days):",

            SETTINGS_NEWS = "Settings of news page",
            SETTINGS_TWITTER_USER = "Get news from Twitter user:",
            SETTINGS_START_NEWS = "First news:",

            SETTINGS_GAME = "Settings of game",
            SETTINGS_GAME_PATH = "Path to game:",
            SETTINGS_USE_UPDATER = "Use game updater",
            SETTINGS_USE_APPLOCALE = "Use AppLocale",
            SETTINGS_APPLOCALE_HELP = "Help",

            //ABOUT
            ABOUT_VERSION = "Version",
            ABOUT_DEV = "Developer",
            ABOUT_DES = "Design help",
            ABOUT_PROJECTS = "Acknowledgments",

            //LAUNCHER UPDATER
            LAUNCHER_UPDATE_NEW_AVAILABLE = "New version {0} available. Changelog:",
            LAUNCHER_UPDATE_NEW_AVAILABLE_CAPTION = "New version {0} available",
            LAUNCHER_Q_UPDATE_WANT_DOWNLOAD = "Do you want to download it now?"
        };

        public static Translation strings = def;

        public static void SerializeDB(string filename, Translation t_object)
        {
            XmlSerializer writer = new XmlSerializer(typeof(Translation));
            StreamWriter file = new StreamWriter(filename);
            writer.Serialize(file, t_object);
            file.Close();
        }

        public static Translation DeSerializeTranslation(string filepath)
        {
            Translation db = new Translation();
            if (File.Exists(filepath))
            {
                StreamReader file = null;
                try
                {
                    XmlSerializer reader = new XmlSerializer(typeof(Translation));
                    file = new StreamReader(filepath);
                    db = (Translation)reader.Deserialize(file);
                    file.Close();
                }
                catch
                {
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

        public static bool LoadTranslation(string name)
        {
            if (name == def.LANGUAGE_NAME)
            {
                SettingsProvider.TRANSLATION_FILE = def.LANGUAGE_NAME;
                strings = def;
                OnChanged();
                return true;
            }

            string file_path = SettingsProvider.APP_PATH + SettingsProvider.LANGS_DIR + name + ".lng";
            if (File.Exists(file_path))
            {
                Translation transl = DeSerializeTranslation(file_path);
                if (transl.IsEmpty())
                {
                    Utils.MSG_ERROR(string.Format("Language file \"{0}\" is broken!", name));
                    return false;
                }
                strings = transl;
                SettingsProvider.TRANSLATION_FILE = name;
                OnChanged();
                return true;
            }

            return false;
        }

        public static string[] GetTranslations()
        {
            string[] translations = null;
            string lang_dir_path = SettingsProvider.APP_PATH + SettingsProvider.LANGS_DIR;
            if (Directory.Exists(lang_dir_path))
                translations = Directory.GetFiles(lang_dir_path, "*.lng");
            return translations;
        }
    }
}
