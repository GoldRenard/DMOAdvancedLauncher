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
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Serialization;
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Model {
#pragma warning disable 1591

    /// <summary>
    /// Main globalization string collection
    /// </summary>
    [XmlType("Language")]
    public sealed class LanguageModel : CrossDomainObject {

        #region Shared Strings

        public string CantOpenLink {
            set;
            get;
        } = "Can't open link. Try to change the default browser. Error: ";

        public string Error {
            get;
            set;
        } = "Error";

        public string Yes {
            get;
            set;
        } = "Yes";

        public string No {
            get;
            set;
        } = "No";

        public string Console {
            get;
            set;
        } = "Console";

        public string CreateButton {
            get;
            set;
        } = "Create";

        public string DeleteButton {
            get;
            set;
        } = "Delete";

        public string BrowseButton {
            get;
            set;
        } = "Browse";

        public string ApplyButton {
            get;
            set;
        } = "Apply";

        public string CancelButton {
            get;
            set;
        } = "Cancel";

        public string SendButton {
            get;
            set;
        } = "Send";

        public string LogInButton {
            get;
            set;
        } = "LogIn";

        public string CloseButton {
            get;
            set;
        } = "Close";

        private string _StartButton = "GAME START";

        public string StartButton {
            set {
                _StartButton = value;
            }
            get {
                return _StartButton.ToUpper();
            }
        }

        public string DigiRotation {
            get;
            set;
        } = "DigiRotation";

        public string PleaseWait {
            get;
            set;
        } = "Please wait";

        public string ErrorOccured {
            get;
            set;
        } = "An error has occurred!";

        public string ConnectionError {
            get;
            set;
        } = "Check your Internet connection.";

        public string CantGetError {
            get;
            set;
        } = "Can't get remote information";

        public string GuildNotFoundError {
            get;
            set;
        } = "Guild not found. Check your settings.";

        public string GameFilesInUse {
            get;
            set;
        } = "Unable to get access to the required game files. Please close all programs that use them.";

        public string PleaseCloseGame {
            get;
            set;
        } = "Please close the game.";

        public string PleaseSelectGamePath {
            get;
            set;
        } = "Please select correct path to directory of game in settings of launcher.";

        public string PleaseSelectLauncherPath {
            get;
            set;
        } = "Please select correct path to directory of default launcher in settings.";

        public string SearchOnWiki {
            get;
            set;
        } = "Search on dmowiki.com";

        #endregion Shared Strings

        #region Main Window

        private string _MainWindow_NewsTab = "NEWS";

        public string MainWindow_NewsTab {
            set {
                _MainWindow_NewsTab = value;
            }
            get {
                return _MainWindow_NewsTab.ToUpper();
            }
        }

        private string _MainWindow_GalleryTab = "GALLERY";

        public string MainWindow_GalleryTab {
            set {
                _MainWindow_GalleryTab = value;
            }
            get {
                return _MainWindow_GalleryTab.ToUpper();
            }
        }

        private string _MainWindow_CommunityTab = "COMMUNITY";

        public string MainWindow_CommunityTab {
            set {
                _MainWindow_CommunityTab = value;
            }
            get {
                return _MainWindow_CommunityTab.ToUpper();
            }
        }

        private string _MainWindow_PersonalizationTab = "PERSONALIZATION";

        public string MainWindow_PersonalizationTab {
            set {
                _MainWindow_PersonalizationTab = value;
            }
            get {
                return _MainWindow_PersonalizationTab.ToUpper();
            }
        }

        #endregion Main Window

        #region About Window

        public string About {
            get;
            set;
        } = "About";

        public string About_Version {
            get;
            set;
        } = "Version";

        public string About_Developer {
            get;
            set;
        } = "Developer";

        public string About_DesignHelp {
            get;
            set;
        } = "Design help";

        public string About_Acknowledgments {
            get;
            set;
        } = "Acknowledgments";

        public string About_Translators {
            get;
            set;
        } = "Translators";

        #endregion About Window

        #region Settings Window

        public string ManageProfiles {
            get;
            set;
        } = "Manage profiles";

        public string Settings {
            get;
            set;
        } = "Settings";

        public string Settings_Language {
            get;
            set;
        } = "Language";

        public string Settings_LastProfile {
            get;
            set;
        } = "It is impossible to delete the latest profile. Launcher must has at least one profile.";

        #region Group Names

        public string Settings_MainGroup {
            get;
            set;
        } = "Profile";

        public string Settings_RotationGroup {
            get;
            set;
        } = "DigiRotation";

        public string Settings_NewsGroup {
            get;
            set;
        } = "News";

        public string Settings_GameGroup {
            get;
            set;
        } = "Game";

        public string Settings_AccountGroup {
            get;
            set;
        } = "Account";

        public string Settings_ThemeGroup {
            get;
            set;
        } = "Theme";

        public string Settings_ColorSchemeGroup {
            get;
            set;
        } = "Color scheme";

        public string Settings_ConnectionGroup {
            get;
            set;
        } = "Connection";

        public string Settings_ProxyGroup {
            get;
            set;
        } = "Proxy";

        #endregion Group Names

        #region Profile

        public string Settings_ProfileNameHint {
            get;
            set;
        } = "Write profile name";

        public string Settings_SetAsDefaultButton {
            get;
            set;
        } = "Set as default profile";

        #endregion Profile

        #region DigiRotation

        public string Settings_RotationGuildHint {
            get;
            set;
        } = "Guild for Rotation";

        public string Settings_RotationTamerHint {
            get;
            set;
        } = "Tamer for Rotation (optional)";

        public string Settings_RotationUpdateFREQ {
            get;
            set;
        } = "Update frequency (days)";

        #endregion DigiRotation

        #region News

        public string Settings_TwitterHint {
            get;
            set;
        } = "Twitter URL (JSON-source)";

        public string Settings_FirstNewsTab {
            get;
            set;
        } = "First news tab";

        #endregion News

        #region Game

        public string Settings_ClientType {
            get;
            set;
        } = "Type of client";

        public string Settings_GamePath {
            get;
            set;
        } = "Path to game";

        public string Settings_LauncherPath {
            get;
            set;
        } = "Path to default launcher";

        public string Settings_UpdateEngine {
            get;
            set;
        } = "Integrated Update Engine";

        public string Settings_AppLocale_Help {
            get;
            set;
        } = "Why inactive?";

        public string Settings_KBLCService {
            get;
            set;
        } = "Fix keyboard layout changing";

        public string Settings_LocaleFix {
            get;
            set;
        } = "Fix game locale";

        public string Settings_Account_User {
            get;
            set;
        } = "Username";

        public string Settings_Account_Password {
            get;
            set;
        } = "Password";

        #endregion Game

        #region Browse Messages

        public string Settings_SelectGameDir {
            get;
            set;
        } = "Please specify the directory where the game is located";

        public string Settings_SelectGameDirError {
            get;
            set;
        } = "You must specify the correct folder of the game.";

        public string Settings_SelectLauncherDir {
            get;
            set;
        } = "Please specify the directory where the default launcher is located";

        public string Settings_SelectLauncherDirError {
            get;
            set;
        } = "You must specify the correct folder of the default launcher.";

        #endregion Browse Messages

        #region Proxy Settings

        public string Settings_Proxy_Default {
            get;
            set;
        } = "System default";

        public string Settings_Proxy_Authentication {
            get;
            set;
        } = "Authentication";

        public string Settings_Proxy_HostWatermark {
            get;
            set;
        } = "Host and port";

        #endregion Proxy Settings

        #endregion Settings Window

        #region AppLocale

        public string AppLocale_FailReasons {
            get;
            set;
        } = "Can't run the game through AppLocale for the following reasons:";

        public string AppLocale_Error {
            get;
            set;
        } = "Microsoft AppLocale Error";

        public string AppLocale_NotInstalled {
            get;
            set;
        } = " - Microsoft AppLocale Utility is not installed";

        public string AppLocale_EALNotInstalled {
            get;
            set;
        } = " - Support of East Asian languages is not installed";

        public string AppLocale_FixQuestion {
            get;
            set;
        } = "Want to open the download page and/or instructions to fix these errors right now?";

        #endregion AppLocale

        #region DigiRotation

        public string RotationLevelText {
            set;
            get;
        } = "level";

        public string RotationDownloading {
            set;
            get;
        } = "Downloading data...";

        public string RotationTamer {
            set;
            get;
        } = "Tamer";

        #endregion DigiRotation

        #region NewsBlock

        public string News {
            set;
            get;
        } = "News";

        public string NewsPubDate {
            set;
            get;
        } = "Publication date";

        public string NewsReadMore {
            set;
            get;
        } = "Read more...";

        public string NewsTwitterError {
            set;
            get;
        } = "Unable to get a list of tweets";

        public string NewsType_Notice {
            set;
            get;
        } = "notice";

        public string NewsType_Event {
            set;
            get;
        } = "event";

        public string NewsType_Patch {
            set;
            get;
        } = "patch";

        #endregion NewsBlock

        #region Game control

        #region Update Section

        private string _GameButton_Waiting = "Waiting";

        public string GameButton_Waiting {
            set {
                _GameButton_Waiting = value;
            }
            get {
                return _GameButton_Waiting.ToUpper();
            }
        }

        private string _GameButton_UpdateGame = "Update game";

        public string GameButton_UpdateGame {
            set {
                _GameButton_UpdateGame = value;
            }
            get {
                return _GameButton_UpdateGame.ToUpper();
            }
        }

        public string UpdateCantGetVersion {
            get;
            set;
        } = "Unable to get the version of the game.";

        public string UpdateCantConnect {
            get;
            set;
        } = "Can't connect to the update servers.";

        public string UpdateDownloading {
            get;
            set;
        } = "Downloading update {0} of {1}... [{2:0.00}MB / {3:0.00}MB]";

        public string UpdateExtracting {
            get;
            set;
        } = "Unpacking update {0} of {1}... [{2} / {3}]";

        public string UpdateInstalling {
            get;
            set;
        } = "Installing updates... [{0} / {1}]";

        public string UseLastSession {
            get;
            set;
        } = "Do you want to use last successful session?";

        #endregion Update Section

        #region Login Section

        public string LoginTry {
            set;
            get;
        } = "Login attempt {0}...";

        public string LoginGettingData {
            set;
            get;
        } = "Obtaining data...";

        public string LoginLogIn {
            set;
            get;
        } = "Log in...";

        public string LoginWasError {
            set;
            get;
        } = "was error";

        public string LoginBadAccount {
            set;
            get;
        } = "Can't login. Your username or password may be incorrect.";

        public string LoginEmptyUsername {
            set;
            get;
        } = "Please type your username!";

        public string LoginEmptyPassword {
            set;
            get;
        } = "Please type your password!";

        public string LoginWrongPage {
            set;
            get;
        } = "Login page is incorrect or has been changed. Please contact the developer of launcher.";

        #endregion Login Section

        #endregion Game control

        #region Gallery

        private string _GalleryNoScreenshots = "THERE IS NO SCREENSHOTS";

        public string GalleryNoScreenshots {
            set {
                _GalleryNoScreenshots = value;
            }
            get {
                return _GalleryNoScreenshots.ToUpper();
            }
        }

        public string GalleryCantOpenImage {
            get;
            set;
        } = "Can't open the image. Error: ";

        #endregion Gallery

        #region Persinalization

        public string PersonalizationWrongTGA {
            get;
            set;
        } = "It is not a TGA-image!";

        public string PersonalizationCantSave {
            get;
            set;
        } = "Unable to save the Image.";

        public string PersonalizationCantWrite {
            get;
            set;
        } = "Unable to write the Image into the game!";

        public string PersonalizationSelectTitle {
            get;
            set;
        } = "Select Image:";

        public string PersonalizationSelectMessage {
            get;
            set;
        } = "Click here to select the Image";

        public string PersonalizationCurrentTitle {
            get;
            set;
        } = "Current Image:";

        public string PersonalizationSaveButton {
            get;
            set;
        } = "Save Image";

        #endregion Persinalization

        #region Community

        public string CommGuildName {
            get;
            set;
        } = "Guild name";

        public string CommGuildNameEmpty {
            get;
            set;
        } = "Please enter the name of the guild!";

        public string CommWrongGuildName {
            get;
            set;
        } = "Guild's name contains invalid characters!";

        public string CommSearchingGuild {
            get;
            set;
        } = "Searching the guild";

        public string CommGettingTamer {
            get;
            set;
        } = "Obtaining Tamer: {0}";

        #region Main DC

        private string _CommTamersTab = "TAMERS";

        public string CommTamersTab {
            set {
                _CommTamersTab = value;
            }
            get {
                return _CommTamersTab.ToUpper();
            }
        }

        private string _CommDigimonsTab = "DIGIMONS";

        public string CommDigimonsTab {
            set {
                _CommDigimonsTab = value;
            }
            get {
                return _CommDigimonsTab.ToUpper();
            }
        }

        public string CommSelect_Guild {
            get;
            set;
        } = "What guild you are looking for";

        public string CommCheckBox_Detailed {
            get;
            set;
        } = "Receive detailed data (long)";

        public string CommButton_Search {
            get;
            set;
        } = "Search a guild";

        public string CommGMaster {
            get;
            set;
        } = "Master";

        public string CommGBest {
            get;
            set;
        } = "Best";

        public string CommGRank {
            get;
            set;
        } = "Ranking";

        public string CommGRep {
            get;
            set;
        } = "Reputation";

        public string CommGTCnt {
            get;
            set;
        } = "Quantity of tamers";

        public string CommGDCnt {
            get;
            set;
        } = "Quantity of digimons";

        #endregion Main DC

        #region Headers

        public string CommHeader_Type {
            get;
            set;
        } = "Type";

        public string CommHeader_Level {
            get;
            set;
        } = "Level";

        public string CommHeader_Ranking {
            get;
            set;
        } = "Ranking";

        public string CommHeader_Tamer {
            get;
            set;
        } = "Tamer";

        public string CommHeader_Partner {
            get;
            set;
        } = "Partner";

        public string CommHeader_Mercenary {
            get;
            set;
        } = "Mercenary";

        public string CommHeader_Name {
            get;
            set;
        } = "Name";

        public string CommHeader_Size {
            get;
            set;
        } = "Size";

        #endregion Headers

        #endregion Community

        #region Update Checker

        public string CheckForUpdates {
            set;
            get;
        } = "Check for updates";

        public string UpdateAvailableText {
            set;
            get;
        } = "New version {0} is available. Changelog:";

        public string UpdateAvailableCaption {
            set;
            get;
        } = "New version {0} is available";

        public string UpdateDownloadQuestion {
            set;
            get;
        } = "Do you want to download it now?";

        #endregion Update Checker

        /// <summary>
        /// Returns property value by its string representation as key
        /// </summary>
        /// <param name="key">Property name</param>
        /// <returns>Property value</returns>
        public string this[string key] {
            get {
                PropertyInfo property = GetType().GetProperty(key);
                if (property == null) {
                    return null;
                }
                return property.GetValue(this, null) as string;
            }
        }

        /// <summary>
        /// Returns string representation of property by lambda expression
        /// </summary>
        /// <param name="expression">Property lambda expression</param>
        /// <returns></returns>
        public string this[Expression<Func<LanguageModel, object>> expression] {
            get {
                var body = expression.Body as MemberExpression;
                if (body == null) {
                    body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
                }
                return body.Member.Name;
            }
        }
    }
}