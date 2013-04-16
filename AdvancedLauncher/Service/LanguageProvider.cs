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

        public class Translation
        {
            //SHARED
            public static string DEF_LANG_NAME = "English (Default)";
            public string LANGUAGE_NAME = "English (Default)";
            public string ERROR = "Error";
            public string CANT_OPEN_LINK = "Can't open link. Try to change the default browser. Error: ";
            public string BROWSE = "Browse";
            public string CLOSE = "Close";
            public string CANCEL = "Cancel";
            public string APPLY = "Apply";
            public string PLEASE_WAIT = "Please wait...";
            public string NEED_CLOSE_GAME = "Please close the game.";
            public string NEED_CLOSE_DEF_LAUNCHER = "Please close the default launcher.";
            public string GAME_FILES_IN_USE = "Unable to get access to the required game files. Please close all programs that use them.";

            //MainWindow
            public string MAIN_NEWS_TAB = "NEWS";
            public string MAIN_GALLERY_TAB = "GALLERY";
            public string MAIN_COMMUNITY_TAB = "COMMUNITY";
            public string MAIN_CUSTOMIZATION_TAB = "CUSTOMIZATION";

            //MainPage
            public string MAIN_START_GAME = "GAME START";
            public string MAIN_START_WAITING = "WAITING...";
            public string MAIN_UPDATE_GAME = "UPDATE GAME";
            public string MAIN_SHOW_TAB = "NEWS:";

            //LoginWindow
            public string LOGIN_START_LAST_SESSION = "Play with last session";
            public string LOGIN_LOGIN = "user";
            public string LOGIN_PASSWORD = "password";
            public string LOGIN_SAVEPASS = "Remember password";

            public string LOGIN_TRY_TEXT = "Try {0}";
            public string LOGIN_GETTING_DATA = "Obtaining data...";
            public string LOGIN_LOGINNING = "Log in...";
            public string LOGIN_WAS_ERROR = "(was error {0})";
            public string LOGIN_BAD_LOGIN_PW = "Can't login. Your username or password may be incorrect.";
            public string LOGIN_EMPTY_LOGIN = "Please input your username!";
            public string LOGIN_EMPTY_PASSWORD = "Please input your password!";
            public string LOGIN_CANT_LOGIN = "Login page is incorrect or has been changed. Please contact the developer of launcher.";

            //Game Update
            public string UPDATE_CANT_GET_GAME_VERSION = "Unable to get the version of the game.";
            public string UPDATE_CANT_CONNECT_TO_JOYMAX = "Can't connect to the update servers.";
            public string UPDATE_DOWNLOADING = "Downloading update {0} of {1}... [{2:0.00}MB / {3:0.00}MB]";
            public string UPDATE_EXTRACTING = "Unpacking update {0} of {1}... [{2} / {3}]";
            public string UPDATE_INSTALLING = "Installing updates... [{0} / {1}]";

            //NEWS
            public string NEWS_TYPE_NOTICE = "event";
            public string NEWS_TYPE_EVENT = "notice";
            public string NEWS_TYPE_PATCH = "patch";
            public string NEWS_CANT_GET_TWITLIST = "Unable to get a list of tweets";
            public string NEWS_PUBLICATION_DATE = "Publication date";
            public string NEWS_READ_MORE = "Read more...";

            //DIGIROTATION
            public string ROT_LOADING = "Loading the list of Digimons";
            public string ROT_TAMER = "Tamer";
            public string ROT_LEVEL = "level";
            public string ROT_ERRMSG1 = "An error has occurred!";
            public string ROT_ERRMSG2 = "Check your Internet connection.";

            //GALLERY
            public string GAL_HELP = "CLICK TWICE ON THE SCREENSHOT FOR OPENING";
            public string GAL_NOSCREENSHOTS = "THERE IS NO SCREENSHOTS";
            public string GAL_CANT_OPEN = "Can't open the image. Error: ";

            //Community
            public string COMM_SHOWTAB = "SHOW:";
            public string COMM_TAMERS_TAB = "TAMERS";
            public string COMM_DIGIMONS_TAB = "DIGIMONS";

            public string COMM_SELECT_GUILD = "What guild you are looking for:";
            public string COMM_CB_DETAILED = "Receive detailed data (long)";
            public string COMM_BTN_SEARCH = "Search a guild";

            public string COMM_INFO = "Information:";
            public string COMM_GMASTER = "Master:";
            public string COMM_GBEST = "Best:";
            public string COMM_GRANK = "Ranking:";
            public string COMM_GREP = "Reputation:";
            public string COMM_TAMER_COUNT = "Quantity of tamers:";
            public string COMM_DIGIMON_COUNT = "Quantity of digimons:";

            public string COMM_TB_GUILD_NAME = "Guild name";
            public string COMM_TB_EMPTY_MSG = "Please enter the name of the guild!";
            public string COMM_TB_INCORRECT = "Guild's name contains invalid characters!";
            public string COMM_STATUS_GUILD_SEARCHING = "Searching the guild";
            public string COMM_STATUS_TAMER_GETTING = "Getting tamer: {0}";

            public string COMM_LHEADER_TYPE = "Type";
            public string COMM_LHEADER_LEVEL = "Level";
            public string COMM_LHEADER_RANKING = "Ranking";
            public string COMM_LHEADER_TAMER = "Tamer";
            public string COMM_LHEADER_PARTNER = "Partner";
            public string COMM_LHEADER_MERCENARY = "Mercenary";
            public string COMM_LHEADER_NAME = "Name";
            public string COMM_LHEADER_SIZE = "Size";

            public string COMM_CANT_GET_GUILD = "Can't get information about the guild";
            public string COMM_CANT_CONNECT_TO_DB = "Can't connect to local database";
            public string COMM_GUILD_NOT_FOUND = "This guild is not found";
            public string COMM_GUILD_NO_CONNECTION = "Unable to get information, check your internet connection";

            //PERSONALISATION
            public string CUST_SELECT_PICTURE = "Select Image:";
            public string CUST_SELECT_MESSAGE = "Click here to select the Image";
            public string CUST_CURRENT_PICTURE = "Current Image:";
            public string CUST_SAVE_PICTURE = "Save the current Image";
            public string CUST_SELECT_TITLE = "Select TGA-Image";
            public string CUST_SAVE_TITLE = "Save Image";
            public string CUST_CANT_OPEN_FS = "Failed to get access to the files of game :(";
            public string CUST_WRONG_TGA = "It is not a TGA-image!";
            public string CUST_CANT_SAVE = "Unable to save the Image.";
            public string CUST_CANT_WRITE = "Unable to write the Image into the game!";
            public string CUST_GAME_FILE_NOT_FOUND = "The file \"{0}\" was not found or launcher don't have access to the game resources.";
            public string CUST_RES_FILE_NOT_FOUND = "File of the list of resources \"{0}\" was not found or is empty.";

            //SETTINGS
            public string SELECT_GAME_DIR = "Please specify the directory where the game is installed";
            public string SELECT_GAME_DIR_WRONG = "You must specify the correct folder with the game.";
            public string SETTINGS_CANT_USE_APPLOC = "Can't run the game through AppLocale for the following reasons:";
            public string SETTINGS_CANT_USE_APPLOC_CAPTION = "Run through AppLocale is impossible now";
            public string SETTINGS_APPLOC_ISNT_INSTALLED = " - Microsoft AppLocale Utility is not installed";
            public string SETTINGS_ALP_ISNT_INSTALLED = " - Support of East Asian languages is not installed";
            public string SETTINGS_QUESTION_DOWNLOAD_NOW = "Want to open the download page and instructions to fix these errors right now?";
            public string SETTINGS_NEED_RESTART = "Some changes will take effect when you restart the launcher";
            public string SETTINGS_NEED_RESTART_CAPTION = "Need restart";

            public string SETTINGS_LANG = "Language:";
            public string SETTINGS_ROTATION_GUILD = "Guild for Digimon Rotation:";
            public string SETTINGS_ROT_UPD_FREQ = "Update frequency (days):";

            public string SETTINGS_NEWS = "Settings of news page";
            public string SETTINGS_TWITTER_USER = "Get news from Twitter user:";
            public string SETTINGS_START_NEWS = "First news:";

            public string SETTINGS_GAME = "Settings of game";
            public string SETTINGS_GAME_PATH = "Path to game:";
            public string SETTINGS_LAUNCHER_PATH = "Path to launcher (D-Player):";
            public string SETTINGS_USE_UPDATER = "Use game updater";
            public string SETTINGS_USE_APPLOCALE = "Use AppLocale";
            public string SETTINGS_APPLOCALE_HELP = "Help";

            //APPLOCALE
            public string APPLAUNCHER_APPLOCALE_NOT_FOUND_MSG = "Microsoft AppLocale was not found on this computer. Do you want to download it now?";
            public string APPLAUNCHER_APPLOCALE_NOT_FOUND_TITLE = "Microsoft AppLocale was not found";
            public string APPLAUNCHER_APPLOCALE_CANT_PATCH = "Microsoft AppLocale patching failed.";
            public string APPLAUNCHER_CANT_EXECURE = "Can't execute program";

            //ABOUT
            public string ABOUT_VERSION = "Version";
            public string ABOUT_DEV = "Developer";
            public string ABOUT_DES = "Design help";
            public string ABOUT_PROJECTS = "Acknowledgments";

            //LAUNCHER UPDATER
            public string LAUNCHER_UPDATE_NEW_AVAILABLE = "New version {0} available. Changelog:";
            public string LAUNCHER_UPDATE_NEW_AVAILABLE_CAPTION = "New version {0} available";
            public string LAUNCHER_Q_UPDATE_WANT_DOWNLOAD = "Do you want to download it now?";

            public bool IsDefault()
            {
                return LANGUAGE_NAME == Translation.DEF_LANG_NAME;
            }
        }

        public static Translation strings = new Translation();

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
          if (name == Translation.DEF_LANG_NAME)
            {
              SettingsProvider.TRANSLATION_FILE = Translation.DEF_LANG_NAME;
                strings = new Translation();
                OnChanged();
                return true;
            }

            string file_path = SettingsProvider.APP_PATH + SettingsProvider.LANGS_DIR + name + ".lng";
            if (File.Exists(file_path))
            {
                Translation transl = DeSerializeTranslation(file_path);
                if (transl.IsDefault())
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
