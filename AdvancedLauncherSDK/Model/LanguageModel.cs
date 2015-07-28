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

namespace AdvancedLauncher.SDK.Model {

    [XmlType("Language")]
    public sealed class LanguageModel {
        private static string sep = ": ";

        #region Shared Strings

        private string _CantOpenLink = "Can't open link. Try to change the default browser. Error: ";

        public string CantOpenLink {
            set {
                _CantOpenLink = value;
            }
            get {
                return _CantOpenLink;
            }
        }

        private string _Error = "Error";

        public string Error {
            set {
                _Error = value;
            }
            get {
                return _Error;
            }
        }

        private string _Yes = "Yes";

        public string Yes {
            set {
                _Yes = value;
            }
            get {
                return _Yes;
            }
        }

        private string _No = "No";

        public string No {
            set {
                _No = value;
            }
            get {
                return _No;
            }
        }

        private string _Console = "Console";

        public string Console {
            set {
                _Console = value;
            }
            get {
                return _Console;
            }
        }

        private string _CreateButton = "Create";

        public string CreateButton {
            set {
                _CreateButton = value;
            }
            get {
                return _CreateButton;
            }
        }

        private string _DeleteButton = "Delete";

        public string DeleteButton {
            set {
                _DeleteButton = value;
            }
            get {
                return _DeleteButton;
            }
        }

        private string _BrowseButton = "Browse";

        public string BrowseButton {
            set {
                _BrowseButton = value;
            }
            get {
                return _BrowseButton;
            }
        }

        private string _ApplyButton = "Apply";

        public string ApplyButton {
            set {
                _ApplyButton = value;
            }
            get {
                return _ApplyButton;
            }
        }

        private string _CancelButton = "Cancel";

        public string CancelButton {
            set {
                _CancelButton = value;
            }
            get {
                return _CancelButton;
            }
        }

        private string _SendButton = "Send";

        public string SendButton {
            set {
                _SendButton = value;
            }
            get {
                return _SendButton;
            }
        }

        private string _LogInButton = "LogIn";

        public string LogInButton {
            set {
                _LogInButton = value;
            }
            get {
                return _LogInButton;
            }
        }

        private string _CloseButton = "Close";

        public string CloseButton {
            set {
                _CloseButton = value;
            }
            get {
                return _CloseButton;
            }
        }

        private string _StartButton = "GAME START";

        public string StartButton {
            set {
                _StartButton = value;
            }
            get {
                return _StartButton.ToUpper();
            }
        }

        private string _DigiRotation = "DigiRotation";

        public string DigiRotation {
            set {
                _DigiRotation = value;
            }
            get {
                return _DigiRotation;
            }
        }

        private string _PleaseWait = "Please wait";

        public string PleaseWait {
            set {
                _PleaseWait = value;
            }
            get {
                return _PleaseWait;
            }
        }

        private string _ErrorOccured = "An error has occurred!";

        public string ErrorOccured {
            set {
                _ErrorOccured = value;
            }
            get {
                return _ErrorOccured;
            }
        }

        private string _ConnectionError = "Check your Internet connection.";

        public string ConnectionError {
            set {
                _ConnectionError = value;
            }
            get {
                return _ConnectionError;
            }
        }

        private string _CantGetError = "Can't get remote information";

        public string CantGetError {
            set {
                _CantGetError = value;
            }
            get {
                return _CantGetError;
            }
        }

        private string _GuildNotFoundError = "Guild not found. Check your settings.";

        public string GuildNotFoundError {
            set {
                _GuildNotFoundError = value;
            }
            get {
                return _GuildNotFoundError;
            }
        }

        private string _GameFilesInUse = "Unable to get access to the required game files. Please close all programs that use them.";

        public string GameFilesInUse {
            set {
                _GameFilesInUse = value;
            }
            get {
                return _GameFilesInUse;
            }
        }

        private string _PleaseCloseGame = "Please close the game.";

        public string PleaseCloseGame {
            set {
                _PleaseCloseGame = value;
            }
            get {
                return _PleaseCloseGame;
            }
        }

        private string _PleaseSelectGamePath = "Please select correct path to directory of game in settings of launcher.";

        public string PleaseSelectGamePath {
            set {
                _PleaseSelectGamePath = value;
            }
            get {
                return _PleaseSelectGamePath;
            }
        }

        private string _PleaseSelectLauncherPath = "Please select correct path to directory of default launcher in settings.";

        public string PleaseSelectLauncherPath {
            set {
                _PleaseSelectLauncherPath = value;
            }
            get {
                return _PleaseSelectLauncherPath;
            }
        }

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

        private string _About = "About";

        public string About {
            set {
                _About = value;
            }
            get {
                return _About;
            }
        }

        private string _About_Version = "Version";

        public string About_Version {
            set {
                _About_Version = value;
            }
            get {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0}: {1}.{2} (build {3})", _About_Version, version.Major, version.Minor, version.Build);
            }
        }

        private string _About_Developer = "Developer";

        public string About_Developer {
            set {
                _About_Developer = value;
            }
            get {
                return _About_Developer + sep;
            }
        }

        private string _About_DesignHelp = "Design help";

        public string About_DesignHelp {
            set {
                _About_DesignHelp = value;
            }
            get {
                return _About_DesignHelp + sep;
            }
        }

        private string _About_Acknowledgments = "Acknowledgments";

        public string About_Acknowledgments {
            set {
                _About_Acknowledgments = value;
            }
            get {
                return _About_Acknowledgments + sep;
            }
        }

        private string _About_Translators = "Translators";

        public string About_Translators {
            set {
                _About_Translators = value;
            }
            get {
                return _About_Translators + sep;
            }
        }

        #endregion About Window

        #region Settings Window

        private string _ManageProfiles = "Manage profiles";

        public string ManageProfiles {
            set {
                _ManageProfiles = value;
            }
            get {
                return _ManageProfiles;
            }
        }

        private string _Settings = "Settings";

        public string Settings {
            set {
                _Settings = value;
            }
            get {
                return _Settings;
            }
        }

        private string _Settings_Language = "Language";

        public string Settings_Language {
            set {
                _Settings_Language = value;
            }
            get {
                return _Settings_Language + sep;
            }
        }

        private string _Settings_LastProfile = "It is impossible to delete the latest profile. Launcher must has at least one profile.";

        public string Settings_LastProfile {
            set {
                _Settings_LastProfile = value;
            }
            get {
                return _Settings_LastProfile;
            }
        }

        #region Group Names

        private string _Settings_MainGroup = "Profile";

        public string Settings_MainGroup {
            set {
                _Settings_MainGroup = value;
            }
            get {
                return _Settings_MainGroup;
            }
        }

        private string _Settings_RotationGroup = "DigiRotation";

        public string Settings_RotationGroup {
            set {
                _Settings_RotationGroup = value;
            }
            get {
                return _Settings_RotationGroup;
            }
        }

        private string _Settings_NewsGroup = "News";

        public string Settings_NewsGroup {
            set {
                _Settings_NewsGroup = value;
            }
            get {
                return _Settings_NewsGroup;
            }
        }

        private string _Settings_GameGroup = "Game";

        public string Settings_GameGroup {
            set {
                _Settings_GameGroup = value;
            }
            get {
                return _Settings_GameGroup;
            }
        }

        private string _Settings_AccountGroup = "Account";

        public string Settings_AccountGroup {
            set {
                _Settings_AccountGroup = value;
            }
            get {
                return _Settings_AccountGroup;
            }
        }

        private string _Settings_ThemeGroup = "Theme";

        public string Settings_ThemeGroup {
            set {
                _Settings_ThemeGroup = value;
            }
            get {
                return _Settings_ThemeGroup + sep;
            }
        }

        private string _Settings_ColorSchemeGroup = "Color scheme";

        public string Settings_ColorSchemeGroup {
            set {
                _Settings_ColorSchemeGroup = value;
            }
            get {
                return _Settings_ColorSchemeGroup + sep;
            }
        }

        private string _Settings_ConnectionGroup = "Connection";

        public string Settings_ConnectionGroup {
            set {
                _Settings_ConnectionGroup = value;
            }
            get {
                return _Settings_ConnectionGroup + sep;
            }
        }

        private string _Settings_ProxyGroup = "Proxy";

        public string Settings_ProxyGroup {
            set {
                _Settings_ProxyGroup = value;
            }
            get {
                return _Settings_ProxyGroup + sep;
            }
        }

        #endregion Group Names

        #region Profile

        private string _Settings_ProfileNameHint = "Write profile name";

        public string Settings_ProfileNameHint {
            set {
                _Settings_ProfileNameHint = value;
            }
            get {
                return _Settings_ProfileNameHint;
            }
        }

        private string _Settings_SetAsDefaultButton = "Set as default profile";

        public string Settings_SetAsDefaultButton {
            set {
                _Settings_SetAsDefaultButton = value;
            }
            get {
                return _Settings_SetAsDefaultButton;
            }
        }

        #endregion Profile

        #region DigiRotation

        private string _Settings_RotationGuildHint = "Guild for Rotation";

        public string Settings_RotationGuildHint {
            set {
                _Settings_RotationGuildHint = value;
            }
            get {
                return _Settings_RotationGuildHint;
            }
        }

        private string _Settings_RotationTamerHint = "Tamer for Rotation (optional)";

        public string Settings_RotationTamerHint {
            set {
                _Settings_RotationTamerHint = value;
            }
            get {
                return _Settings_RotationTamerHint;
            }
        }

        private string _Settings_RotationUpdateFREQ = "Update frequency (days)";

        public string Settings_RotationUpdateFREQ {
            set {
                _Settings_RotationUpdateFREQ = value;
            }
            get {
                return _Settings_RotationUpdateFREQ + sep;
            }
        }

        #endregion DigiRotation

        #region News

        private string _Settings_TwitterHint = "Twitter URL (JSON-source)";

        public string Settings_TwitterHint {
            set {
                _Settings_TwitterHint = value;
            }
            get {
                return _Settings_TwitterHint;
            }
        }

        private string _Settings_FirstNewsTab = "First news tab";

        public string Settings_FirstNewsTab {
            set {
                _Settings_FirstNewsTab = value;
            }
            get {
                return _Settings_FirstNewsTab + sep;
            }
        }

        #endregion News

        #region Game

        private string _Settings_ClientType = "Type of client";

        public string Settings_ClientType {
            set {
                _Settings_ClientType = value;
            }
            get {
                return _Settings_ClientType + sep;
            }
        }

        private string _Settings_GamePath = "Path to game";

        public string Settings_GamePath {
            set {
                _Settings_GamePath = value;
            }
            get {
                return _Settings_GamePath + sep;
            }
        }

        private string _Settings_LauncherPath = "Path to default launcher";

        public string Settings_LauncherPath {
            set {
                _Settings_LauncherPath = value;
            }
            get {
                return _Settings_LauncherPath + sep;
            }
        }

        private string _Settings_UpdateEngine = "Integrated Update Engine";

        public string Settings_UpdateEngine {
            set {
                _Settings_UpdateEngine = value;
            }
            get {
                return _Settings_UpdateEngine;
            }
        }

        private string _Settings_AppLocale_Help = "Why inactive?";

        public string Settings_AppLocale_Help {
            set {
                _Settings_AppLocale_Help = value;
            }
            get {
                return _Settings_AppLocale_Help;
            }
        }

        private string _Settings_KBLCService = "Fix keyboard layout changing";

        public string Settings_KBLCService {
            set {
                _Settings_KBLCService = value;
            }
            get {
                return _Settings_KBLCService;
            }
        }

        private string _Settings_LocaleFix = "Fix game locale";

        public string Settings_LocaleFix {
            set {
                _Settings_LocaleFix = value;
            }
            get {
                return _Settings_LocaleFix;
            }
        }

        private string _Settings_Account_User = "Username";

        public string Settings_Account_User {
            set {
                _Settings_Account_User = value;
            }
            get {
                return _Settings_Account_User;
            }
        }

        private string _Settings_Account_Password = "Password";

        public string Settings_Account_Password {
            set {
                _Settings_Account_Password = value;
            }
            get {
                return _Settings_Account_Password;
            }
        }

        #endregion Game

        #region Browse Messages

        private string _Settings_SelectGameDir = "Please specify the directory where the game is located";

        public string Settings_SelectGameDir {
            set {
                _Settings_SelectGameDir = value;
            }
            get {
                return _Settings_SelectGameDir;
            }
        }

        private string _Settings_SelectGameDirError = "You must specify the correct folder of the game.";

        public string Settings_SelectGameDirError {
            set {
                _Settings_SelectGameDirError = value;
            }
            get {
                return _Settings_SelectGameDirError;
            }
        }

        private string _Settings_SelectLauncherDir = "Please specify the directory where the default launcher is located";

        public string Settings_SelectLauncherDir {
            set {
                _Settings_SelectLauncherDir = value;
            }
            get {
                return _Settings_SelectLauncherDir;
            }
        }

        private string _Settings_SelectLauncherDirError = "You must specify the correct folder of the default launcher.";

        public string Settings_SelectLauncherDirError {
            set {
                _Settings_SelectLauncherDirError = value;
            }
            get {
                return _Settings_SelectLauncherDirError;
            }
        }

        #endregion Browse Messages

        #region Proxy Settings

        private string _Settings_Proxy_Default = "System default";

        public string Settings_Proxy_Default {
            set {
                _Settings_Proxy_Default = value;
            }
            get {
                return _Settings_Proxy_Default;
            }
        }

        private string _Settings_Proxy_Authentication = "Authentication";

        public string Settings_Proxy_Authentication {
            set {
                _Settings_Proxy_Authentication = value;
            }
            get {
                return _Settings_Proxy_Authentication + sep;
            }
        }

        private string _Settings_Proxy_HostWatermark = "Host and port";

        public string Settings_Proxy_HostWatermark {
            set {
                _Settings_Proxy_HostWatermark = value;
            }
            get {
                return _Settings_Proxy_HostWatermark;
            }
        }

        #endregion Proxy Settings

        #endregion Settings Window

        #region AppLocale

        private string _AppLocale_FailReasons = "Can't run the game through AppLocale for the following reasons:";

        public string AppLocale_FailReasons {
            set {
                _AppLocale_FailReasons = value;
            }
            get {
                return _AppLocale_FailReasons;
            }
        }

        private string _AppLocale_Error = "Microsoft AppLocale Error";

        public string AppLocale_Error {
            set {
                _AppLocale_Error = value;
            }
            get {
                return _AppLocale_Error;
            }
        }

        private string _AppLocale_NotInstalled = " - Microsoft AppLocale Utility is not installed";

        public string AppLocale_NotInstalled {
            set {
                _AppLocale_NotInstalled = value;
            }
            get {
                return _AppLocale_NotInstalled;
            }
        }

        private string _AppLocale_EALNotInstalled = " - Support of East Asian languages is not installed";

        public string AppLocale_EALNotInstalled {
            set {
                _AppLocale_EALNotInstalled = value;
            }
            get {
                return _AppLocale_EALNotInstalled;
            }
        }

        private string _AppLocale_FixQuestion = "Want to open the download page and/or instructions to fix these errors right now?";

        public string AppLocale_FixQuestion {
            set {
                _AppLocale_FixQuestion = value;
            }
            get {
                return _AppLocale_FixQuestion;
            }
        }

        #endregion AppLocale

        #region DigiRotation

        private string _RotationLevelText = "level";

        public string RotationLevelText {
            set {
                _RotationLevelText = value;
            }
            get {
                return _RotationLevelText;
            }
        }

        private string _RotationDownloading = "Downloading data...";

        public string RotationDownloading {
            set {
                _RotationDownloading = value;
            }
            get {
                return _RotationDownloading;
            }
        }

        private string _RotationTamer = "Tamer";

        public string RotationTamer {
            set {
                _RotationTamer = value;
            }
            get {
                return _RotationTamer;
            }
        }

        #endregion DigiRotation

        #region NewsBlock

        private string _News = "News";

        public string News {
            set {
                _News = value;
            }
            get {
                return _News;
            }
        }

        private string _NewsPubDate = "Publication date";

        public string NewsPubDate {
            set {
                _NewsPubDate = value;
            }
            get {
                return _NewsPubDate + sep;
            }
        }

        private string _NewsReadMore = "Read more...";

        public string NewsReadMore {
            set {
                _NewsReadMore = value;
            }
            get {
                return _NewsReadMore;
            }
        }

        private string _NewsTwitterError = "Unable to get a list of tweets";

        public string NewsTwitterError {
            set {
                _NewsTwitterError = value;
            }
            get {
                return _NewsTwitterError;
            }
        }

        private string _NewsType_Notice = "notice";

        public string NewsType_Notice {
            set {
                _NewsType_Notice = value;
            }
            get {
                return _NewsType_Notice;
            }
        }

        private string _NewsType_Event = "event";

        public string NewsType_Event {
            set {
                _NewsType_Event = value;
            }
            get {
                return _NewsType_Event;
            }
        }

        private string _NewsType_Patch = "patch";

        public string NewsType_Patch {
            set {
                _NewsType_Patch = value;
            }
            get {
                return _NewsType_Patch;
            }
        }

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

        private string _UpdateCantGetVersion = "Unable to get the version of the game.";

        public string UpdateCantGetVersion {
            set {
                _UpdateCantGetVersion = value;
            }
            get {
                return _UpdateCantGetVersion;
            }
        }

        private string _UpdateCantConnect = "Can't connect to the update servers.";

        public string UpdateCantConnect {
            set {
                _UpdateCantConnect = value;
            }
            get {
                return _UpdateCantConnect;
            }
        }

        private string _UpdateDownloading = "Downloading update {0} of {1}... [{2:0.00}MB / {3:0.00}MB]";

        public string UpdateDownloading {
            set {
                _UpdateDownloading = value;
            }
            get {
                return _UpdateDownloading;
            }
        }

        private string _UpdateExtracting = "Unpacking update {0} of {1}... [{2} / {3}]";

        public string UpdateExtracting {
            set {
                _UpdateExtracting = value;
            }
            get {
                return _UpdateExtracting;
            }
        }

        private string _UpdateInstalling = "Installing updates... [{0} / {1}]";

        public string UpdateInstalling {
            set {
                _UpdateInstalling = value;
            }
            get {
                return _UpdateInstalling;
            }
        }

        private string _UseLastSession = "Do you want to use last successful session?";

        public string UseLastSession {
            set {
                _UseLastSession = value;
            }
            get {
                return _UseLastSession;
            }
        }

        #endregion Update Section

        #region Login Section

        private string _LoginTry = "Try {0}...";

        public string LoginTry {
            set {
                _LoginTry = value;
            }
            get {
                return _LoginTry;
            }
        }

        private string _LoginGettingData = "Obtaining data...";

        public string LoginGettingData {
            set {
                _LoginGettingData = value;
            }
            get {
                return _LoginGettingData;
            }
        }

        private string _LoginLogIn = "Log in...";

        public string LoginLogIn {
            set {
                _LoginLogIn = value;
            }
            get {
                return _LoginLogIn;
            }
        }

        private string _LoginWasError = "was error";

        public string LoginWasError {
            set {
                _LoginWasError = value;
            }
            get {
                return _LoginWasError;
            }
        }

        private string _LoginBadAccount = "Can't login. Your username or password may be incorrect.";

        public string LoginBadAccount {
            set {
                _LoginBadAccount = value;
            }
            get {
                return _LoginBadAccount;
            }
        }

        private string _LoginEmptyUsername = "Please type your username!";

        public string LoginEmptyUsername {
            set {
                _LoginEmptyUsername = value;
            }
            get {
                return _LoginEmptyUsername;
            }
        }

        private string _LoginEmptyPassword = "Please type your password!";

        public string LoginEmptyPassword {
            set {
                _LoginEmptyPassword = value;
            }
            get {
                return _LoginEmptyPassword;
            }
        }

        private string _LoginWrongPage = "Login page is incorrect or has been changed. Please contact the developer of launcher.";

        public string LoginWrongPage {
            set {
                _LoginWrongPage = value;
            }
            get {
                return _LoginWrongPage;
            }
        }

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

        private string _GalleryCantOpenImage = "Can't open the image. Error: ";

        public string GalleryCantOpenImage {
            set {
                _GalleryCantOpenImage = value;
            }
            get {
                return _GalleryCantOpenImage;
            }
        }

        #endregion Gallery

        #region Persinalization

        private string _PersonalizationWrongTGA = "It is not a TGA-image!";

        public string PersonalizationWrongTGA {
            set {
                _PersonalizationWrongTGA = value;
            }
            get {
                return _PersonalizationWrongTGA;
            }
        }

        private string _PersonalizationCantSave = "Unable to save the Image.";

        public string PersonalizationCantSave {
            set {
                _PersonalizationCantSave = value;
            }
            get {
                return _PersonalizationCantSave;
            }
        }

        private string _PersonalizationCantWrite = "Unable to write the Image into the game!";

        public string PersonalizationCantWrite {
            set {
                _PersonalizationCantWrite = value;
            }
            get {
                return _PersonalizationCantWrite;
            }
        }

        private string _PersonalizationSelectTitle = "Select Image:";

        public string PersonalizationSelectTitle {
            set {
                _PersonalizationSelectTitle = value;
            }
            get {
                return _PersonalizationSelectTitle;
            }
        }

        private string _PersonalizationSelectMessage = "Click here to select the Image";

        public string PersonalizationSelectMessage {
            set {
                _PersonalizationSelectMessage = value;
            }
            get {
                return _PersonalizationSelectMessage;
            }
        }

        private string _PersonalizationCurrentTitle = "Current Image:";

        public string PersonalizationCurrentTitle {
            set {
                _PersonalizationCurrentTitle = value;
            }
            get {
                return _PersonalizationCurrentTitle;
            }
        }

        private string _PersonalizationSaveButton = "Save Image";

        public string PersonalizationSaveButton {
            set {
                _PersonalizationSaveButton = value;
            }
            get {
                return _PersonalizationSaveButton;
            }
        }

        #endregion Persinalization

        #region Community

        private string _CommGuildName = "Guild name";

        public string CommGuildName {
            set {
                _CommGuildName = value;
            }
            get {
                return _CommGuildName;
            }
        }

        private string _CommGuildNameEmpty = "Please enter the name of the guild!";

        public string CommGuildNameEmpty {
            set {
                _CommGuildNameEmpty = value;
            }
            get {
                return _CommGuildNameEmpty;
            }
        }

        private string _CommWrongGuildName = "Guild's name contains invalid characters!";

        public string CommWrongGuildName {
            set {
                _CommWrongGuildName = value;
            }
            get {
                return _CommWrongGuildName;
            }
        }

        private string _CommSearchingGuild = "Searching the guild";

        public string CommSearchingGuild {
            set {
                _CommSearchingGuild = value;
            }
            get {
                return _CommSearchingGuild;
            }
        }

        private string _CommGettingTamer = "Obtaining Tamer: {0}";

        public string CommGettingTamer {
            set {
                _CommGettingTamer = value;
            }
            get {
                return _CommGettingTamer;
            }
        }

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

        private string _CommSelect_Guild = "What guild you are looking for";

        public string CommSelect_Guild {
            set {
                _CommSelect_Guild = value;
            }
            get {
                return _CommSelect_Guild + sep;
            }
        }

        private string _CommCheckBox_Detailed = "Receive detailed data (long)";

        public string CommCheckBox_Detailed {
            set {
                _CommCheckBox_Detailed = value;
            }
            get {
                return _CommCheckBox_Detailed;
            }
        }

        private string _CommButton_Search = "Search a guild";

        public string CommButton_Search {
            set {
                _CommButton_Search = value;
            }
            get {
                return _CommButton_Search;
            }
        }

        private string _CommGMaster = "Master";

        public string CommGMaster {
            set {
                _CommGMaster = value;
            }
            get {
                return _CommGMaster + sep;
            }
        }

        private string _CommGBest = "Best";

        public string CommGBest {
            set {
                _CommGBest = value;
            }
            get {
                return _CommGBest + sep;
            }
        }

        private string _CommGRank = "Ranking";

        public string CommGRank {
            set {
                _CommGRank = value;
            }
            get {
                return _CommGRank + sep;
            }
        }

        private string _CommGRep = "Reputation";

        public string CommGRep {
            set {
                _CommGRep = value;
            }
            get {
                return _CommGRep + sep;
            }
        }

        private string _CommGTCnt = "Quantity of tamers";

        public string CommGTCnt {
            set {
                _CommGTCnt = value;
            }
            get {
                return _CommGTCnt + sep;
            }
        }

        private string _CommGDCnt = "Quantity of digimons";

        public string CommGDCnt {
            set {
                _CommGDCnt = value;
            }
            get {
                return _CommGDCnt + sep;
            }
        }

        #endregion Main DC

        #region Headers

        private string _CommHeader_Type = "Type";

        public string CommHeader_Type {
            set {
                _CommHeader_Type = value;
            }
            get {
                return _CommHeader_Type;
            }
        }

        private string _CommHeader_Level = "Level";

        public string CommHeader_Level {
            set {
                _CommHeader_Level = value;
            }
            get {
                return _CommHeader_Level;
            }
        }

        private string _CommHeader_Ranking = "Ranking";

        public string CommHeader_Ranking {
            set {
                _CommHeader_Ranking = value;
            }
            get {
                return _CommHeader_Ranking;
            }
        }

        private string _CommHeader_Tamer = "Tamer";

        public string CommHeader_Tamer {
            set {
                _CommHeader_Tamer = value;
            }
            get {
                return _CommHeader_Tamer;
            }
        }

        private string _CommHeader_Partner = "Partner";

        public string CommHeader_Partner {
            set {
                _CommHeader_Partner = value;
            }
            get {
                return _CommHeader_Partner;
            }
        }

        private string _CommHeader_Mercenary = "Mercenary";

        public string CommHeader_Mercenary {
            set {
                _CommHeader_Mercenary = value;
            }
            get {
                return _CommHeader_Mercenary;
            }
        }

        private string _CommHeader_Name = "Name";

        public string CommHeader_Name {
            set {
                _CommHeader_Name = value;
            }
            get {
                return _CommHeader_Name;
            }
        }

        private string _CommHeader_Size = "Size";

        public string CommHeader_Size {
            set {
                _CommHeader_Size = value;
            }
            get {
                return _CommHeader_Size;
            }
        }

        #endregion Headers

        #endregion Community

        #region Update Checker

        private string _UpdateAvailableText = "New version {0} available. Changelog:";

        public string UpdateAvailableText {
            set {
                _UpdateAvailableText = value;
            }
            get {
                return _UpdateAvailableText;
            }
        }

        private string _UpdateAvailableCaption = "New version {0} available";

        public string UpdateAvailableCaption {
            set {
                _UpdateAvailableCaption = value;
            }
            get {
                return _UpdateAvailableCaption;
            }
        }

        private string _UpdateDownloadQuestion = "Do you want to download it now?";

        public string UpdateDownloadQuestion {
            set {
                _UpdateDownloadQuestion = value;
            }
            get {
                return _UpdateDownloadQuestion;
            }
        }

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