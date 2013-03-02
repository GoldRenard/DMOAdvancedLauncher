// ======================================================================
// DMOLibrary
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
using System.IO;
using System.Globalization;
using Ookii.Dialogs.Wpf;
using Microsoft.Win32;
using HtmlAgilityPack;

namespace DMOLibrary.Profiles
{
    public abstract class DMOProfile
    {
        protected System.Windows.Threading.Dispatcher owner_dispatcher = null;

        #region LANGUAGE SECTION
        public static string SELECT_GAME_DIR = "Please select directory of game for ";
        public static string SELECT_LAUNCHER_DIR = "Please select directory of default launcher (D-Player, for example) for ";
        #endregion LANGUAGE SECTION

        protected string TYPE_NAME;
        protected string PROFILE_NAME = "Default";
        private string PROFILES_FOLDER = "{0}\\Profiles";
        protected string DATABASE_NAME = string.Empty;
        private string DATABASES_FOLDER = "{0}\\Databases";
        public bool IsSeparateLauncher = false;
        public bool IsWebSupported = false;
        public bool IsUpdateSupported = false;
        public bool IsLoginRequired = false;
        public bool IsNewsSupported = false;
        public string GetProfileName() { return PROFILE_NAME; }
        public string GetTypeName() { return TYPE_NAME; }

        public DMOProfile()
        {
            string dir = string.Format(PROFILES_FOLDER, System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory));
            if (!Directory.Exists(dir))
            {
                try { Directory.CreateDirectory(dir); }
                catch { }
            }

            dir = string.Format(DATABASES_FOLDER, System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory));
            if (!Directory.Exists(dir))
            {
                try { Directory.CreateDirectory(dir); }
                catch { }
            }
        }

        #region Game start
        protected int login_try, start_try = 0, last_error = -1;
        protected System.Windows.Forms.WebBrowser wb = new System.Windows.Forms.WebBrowser() { ScriptErrorsSuppressed = true };

        public delegate void GameStartHandler(object sender, LoginCode code, string result);
        public event GameStartHandler GameStartCompleted;
        protected virtual void OnCompleted(LoginCode code, string result)
        {
            if (GameStartCompleted != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new GameStartHandler((sender, code_, result_) =>
                    {
                        GameStartCompleted(sender, code_, result_);
                    }), this, code, result);
                }
                else
                    GameStartCompleted(this, code, result);
            }
        }

        public delegate void LoginStateHandler(object sender, LoginState state, int try_num, int last_error);
        public event LoginStateHandler LoginStateChanged;
        protected virtual void OnChanged(LoginState state)
        {
            if (LoginStateChanged != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new LoginStateHandler((sender_, state_, try_num_, last_error_) =>
                    {
                        LoginStateChanged(sender_, state_, try_num_, last_error_);
                    }), this, state, start_try + 1, last_error);
                }
                else
                    LoginStateChanged(this, state, start_try + 1, last_error);
            }
        }

        public abstract void GameStart();
        public abstract void LastSessionStart();

        protected void TryParseInfo(string content)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            string result_text = doc.DocumentNode.SelectSingleNode("//body").InnerText;

            result_text = result_text.Replace("\r\n-\r\n", "");
            result_text = result_text.Replace("\r\n", "");
            result_text = System.Net.WebUtility.HtmlDecode(result_text);

            HtmlDocument result = new HtmlDocument();
            result.LoadHtml(result_text);

            int res_code = Convert.ToInt32(result.DocumentNode.SelectSingleNode("//result").Attributes["value"].Value);
            LastSessionArgs = string.Empty;
            if (res_code == 0)
            {
                foreach (HtmlNode node in result.DocumentNode.SelectNodes("//param"))
                {
                    try { LastSessionArgs += node.Attributes["value"].Value + " "; }
                    catch { };
                }
                LastSessionStart();
            }
            else
            {
                last_error = res_code;
                start_try++;
                GameStart();
            }
        }
        #endregion Game start

        #region Game environment
        public bool IsUpdateNeeded = false;
        protected string PATH_FORMAT = "{0}\\{1}";
        protected string GAME_EXE, GAME_PATH;
        protected string LAUNCHER_EXE, LAUNCHER_PATH;
        protected string LOCAL_VERSION_FILE, REMOTE_VERSION_FILE, PATCHES_URL;
        protected string PACK_PF_FILE = @"\Data\Pack01.pf", PACK_HF_FILE = @"\Data\Pack01.hf", PACK_IMPORT_DIR = @"\Pack01";
        protected string REGEDIT_GAME_KEY, REGEDIT_GAME_PARAMETER = "Path";
        protected string REGEDIT_LAUNCHER_KEY, REGEDIT_LAUNCHER_PARAMETER = "Launcher";

        /// <summary> Checks access to file </summary>
        /// <param name="file">Full path to file</param>
        /// <returns> <see langword="True"/> if file is locked </returns>
        public static bool IsFileLocked(string file)
        {
            FileStream stream = null;

            try { stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None); }
            catch (IOException) { return true; }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

        /// <summary> Checks access to game resources. Checks for working game, default launcher and locked game resources </summary>
        /// <returns> <see langword="True"/> if game resources are accessible </returns>
        public bool CheckUpdateAccess()
        {
            if (IsFileLocked(GetPackHFPath()) || IsFileLocked(GetPackPFPath()))
                return false;
            return true;
        }

        public string GetPackPFPath() { return GAME_PATH + PACK_PF_FILE; }
        public string GetPackHFPath() { return GAME_PATH + PACK_HF_FILE; }
        public string GetPackImportDir() { return GAME_PATH + PACK_IMPORT_DIR; }

        public string GetVersionFile() { return GAME_PATH + LOCAL_VERSION_FILE; }
        public string GetRemoteVersionURL() { return REMOTE_VERSION_FILE; }
        public string GetPatchURL() { return PATCHES_URL; }

        public bool CheckGameDirectory(string PATH)
        {
            if (!File.Exists(PATH + LOCAL_VERSION_FILE) || !File.Exists(string.Format(PATH_FORMAT, PATH, GAME_EXE)))
                return false;
            return true;
        }
        public string GetGamePath()
        {
            return GetGamePath(false);
        }
        public string GetGamePath(bool AllowCancel)
        {
            if (CheckGameDirectory(GAME_PATH))
                return GAME_PATH;

            RegistryKey read_settings = Registry.CurrentUser.CreateSubKey(REGEDIT_GAME_KEY);
            GAME_PATH = (string)read_settings.GetValue(REGEDIT_GAME_PARAMETER);
            read_settings.Close();

            if (!CheckGameDirectory(GAME_PATH))
                GAME_PATH = SelectGameDir(AllowCancel);
            return GAME_PATH;
        }
        public string SelectGameDir(bool AllowCancel)
        {
            string PATH;
            if (Environment.OSVersion.Version.Major <= 5)
            {
                System.Windows.Forms.FolderBrowserDialog FBrowser = new System.Windows.Forms.FolderBrowserDialog();
                FBrowser.Description = SELECT_GAME_DIR + TYPE_NAME;
                FBrowser.ShowNewFolderButton = false;
                while (true)
                {
                    PATH = string.Empty;
                    if (FBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        PATH = FBrowser.SelectedPath;
                    else if (AllowCancel)
                        break;
                    if (CheckGameDirectory(PATH))
                    {
                        RegistryKey reg = Registry.CurrentUser.CreateSubKey(REGEDIT_GAME_KEY);
                        reg.SetValue(REGEDIT_GAME_PARAMETER, PATH, RegistryValueKind.String);
                        reg.Close();
                        break;
                    }
                }
            }
            else
            {
                VistaFolderBrowserDialog FBrowser = new VistaFolderBrowserDialog();
                FBrowser.Description = SELECT_GAME_DIR + TYPE_NAME;
                FBrowser.ShowNewFolderButton = false;
                while (true)
                {
                    PATH = string.Empty;
                    if (FBrowser.ShowDialog() == true)
                        PATH = FBrowser.SelectedPath;
                    else if (AllowCancel)
                        break;
                    if (CheckGameDirectory(PATH))
                    {
                        RegistryKey reg = Registry.CurrentUser.CreateSubKey(REGEDIT_GAME_KEY);
                        reg.SetValue(REGEDIT_GAME_PARAMETER, PATH, RegistryValueKind.String);
                        reg.Close();
                        break;
                    }
                }
            }
            return PATH;
        }

        public bool CheckLauncherDirectory(string PATH) { return File.Exists(string.Format(PATH_FORMAT, PATH, LAUNCHER_EXE)); }
        public string GetLauncherPath()
        {
            return GetLauncherPath(false);
        }
        public string GetLauncherPath(bool AllowCancel)
        {
            if (CheckLauncherDirectory(LAUNCHER_PATH))
                return LAUNCHER_PATH;

            RegistryKey read_settings = Registry.CurrentUser.CreateSubKey(REGEDIT_LAUNCHER_KEY);
            LAUNCHER_PATH = (string)read_settings.GetValue(REGEDIT_LAUNCHER_PARAMETER);
            read_settings.Close();

            if (!CheckLauncherDirectory(LAUNCHER_PATH))
                LAUNCHER_PATH = SelectLauncherDir(AllowCancel);
            return LAUNCHER_PATH;
        }
        public string SelectLauncherDir(bool AllowCancel)
        {
            string PATH;
            if (Environment.OSVersion.Version.Major <= 5)
            {
                System.Windows.Forms.FolderBrowserDialog FBrowser = new System.Windows.Forms.FolderBrowserDialog();
                FBrowser.Description = SELECT_LAUNCHER_DIR + TYPE_NAME;
                FBrowser.ShowNewFolderButton = false;
                while (true)
                {
                    PATH = string.Empty;
                    if (FBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        PATH = FBrowser.SelectedPath;
                    else if (AllowCancel)
                        break;
                    if (CheckLauncherDirectory(PATH))
                    {
                        RegistryKey reg = Registry.CurrentUser.CreateSubKey(REGEDIT_LAUNCHER_KEY);
                        reg.SetValue(REGEDIT_LAUNCHER_PARAMETER, PATH, RegistryValueKind.String);
                        reg.Close();
                        break;
                    }
                }
            }
            else
            {
                VistaFolderBrowserDialog FBrowser = new VistaFolderBrowserDialog();
                FBrowser.Description = SELECT_LAUNCHER_DIR + TYPE_NAME;
                FBrowser.ShowNewFolderButton = false;
                while (true)
                {
                    PATH = string.Empty;
                    if (FBrowser.ShowDialog() == true)
                        PATH = FBrowser.SelectedPath;
                    else if (AllowCancel)
                        break;
                    if (CheckLauncherDirectory(PATH))
                    {
                        RegistryKey reg = Registry.CurrentUser.CreateSubKey(REGEDIT_LAUNCHER_KEY);
                        reg.SetValue(REGEDIT_LAUNCHER_PARAMETER, PATH, RegistryValueKind.String);
                        reg.Close();
                        break;
                    }
                }
            }
            return PATH;
        }

        #endregion Game environment

        #region Settings Section
        public string USER_ID, USER_PASSWORD;
        public bool RememberPassword = false;
        public string LastSessionArgs = string.Empty;
        public bool S_USE_APPLOC = true;
        public bool S_USE_UPDATE_ENGINE = false;
        public string S_ROTATION_GNAME = "MonolithMesa";
        public server S_ROTATION_GSERV = new server() { Id = 1 };
        public int S_ROTATION_URATE = 2;
        public int S_FIRST_TAB = 1;
        public string S_TWITTER_USER = "dmo_russian";

        protected string GetSettingsPath()
        {
            return string.Format(PROFILES_FOLDER + "\\{1}.{2}.ini", System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory), TYPE_NAME, PROFILE_NAME);
        }

        public void ReadSettings()
        {
            if (!File.Exists(GetSettingsPath()))
                WriteSettings();

            string tmp = string.Empty;
            int tmp_int;

            IniFile ini = new IniFile(GetSettingsPath());

            USER_ID = ini.ReadString("User", "UserId");
            try { USER_PASSWORD = PassEncrypt.Decrypt(ini.ReadString("User", "PasswordHash"), "ddFG4!34sdgxBh"); }
            catch { USER_PASSWORD = string.Empty; }
            RememberPassword = ini.ReadString("User", "RememberPassword").ToLower() == "true";

            LastSessionArgs = ini.ReadString("Environment", "LastSessionArgs");
            S_USE_UPDATE_ENGINE = ini.ReadString("Environment", "UpdateAllowed").ToLower() == "true";
            S_USE_APPLOC = ini.ReadString("Environment", "AppLocale").ToLower() == "true";
            if (!IsALSupported())
                S_USE_APPLOC = false;

            GAME_PATH = GetGamePath(true);
            if (GAME_PATH == string.Empty)
                System.Diagnostics.Process.GetCurrentProcess().Kill();

            if (!IsSeparateLauncher)
                LAUNCHER_PATH = GAME_PATH;
            else
            {
                LAUNCHER_PATH = GetLauncherPath(true);
                if (LAUNCHER_PATH == string.Empty)
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            S_ROTATION_GNAME = ini.ReadString("DigiRotation", "Guild");
            tmp = ini.ReadString("DigiRotation", "ServerId");
            if ((bool)Int32.TryParse(tmp, out tmp_int))
                S_ROTATION_GSERV.Id = tmp_int;
            tmp = ini.ReadString("DigiRotation", "UpdateInDays");
            if ((bool)Int32.TryParse(tmp, out tmp_int))
                S_ROTATION_URATE = tmp_int;

            tmp = ini.ReadString("News", "FirstTab");
            if ((bool)Int32.TryParse(tmp, out tmp_int))
                S_FIRST_TAB = tmp_int;
            S_TWITTER_USER = ini.ReadString("News", "TwitterUser");
        }

        public void WriteSettings()
        {
            try { File.Delete(GetSettingsPath()); }
            catch { }

            string pWrd;
            if (!RememberPassword)
                pWrd = string.Empty;
            else if (USER_PASSWORD == null)
                pWrd = string.Empty;
            else if (USER_PASSWORD.Length == 0)
                pWrd = string.Empty;
            else
                pWrd = PassEncrypt.Encrypt(USER_PASSWORD, "ddFG4!34sdgxBh");

            List<string> settings = new List<string>();
            if (IsLoginRequired)
            {
                settings.Add("[User]");
                settings.Add("UserId=" + USER_ID);
                settings.Add("PasswordHash=" + pWrd);
                settings.Add("RememberPassword=" + RememberPassword.ToString());
                settings.Add(string.Empty);
            }
            settings.Add("[Environment]");
            settings.Add("AppLocale=" + (S_USE_APPLOC && IsALSupported()).ToString());
            settings.Add("UpdateAllowed=" + S_USE_UPDATE_ENGINE.ToString());
            if (IsLoginRequired)
                settings.Add("LastSessionArgs=" + LastSessionArgs);
            settings.Add(string.Empty);

            if (IsWebSupported)
            {
                settings.Add("[DigiRotation]");
                settings.Add("Guild=" + S_ROTATION_GNAME);
                settings.Add("ServerId=" + S_ROTATION_GSERV.Id.ToString());
                settings.Add("UpdateInDays=" + S_ROTATION_URATE.ToString());
                settings.Add(string.Empty);
            }

            settings.Add("[News]");
            if (IsNewsSupported)
                settings.Add("FirstTab=" + S_FIRST_TAB.ToString());
            settings.Add("TwitterUser=" + S_TWITTER_USER);

            try { File.WriteAllLines(GetSettingsPath(), settings.ToArray()); }
            catch { }
        }

        private bool IsALSupported()
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

            return isALInstalled && isKoreanSupported;
        }
        #endregion

        #region Database Section
        public string GetDatabasePath()
        {
            if (DATABASE_NAME != string.Empty)
                return string.Format(DATABASES_FOLDER + "\\{1}.sqlite", System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory), DATABASE_NAME);
            return string.Format(DATABASES_FOLDER + "\\{1}.sqlite", System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory), TYPE_NAME);
        }

        public abstract DMOWebProfile GetWebProfile();
        public abstract DMONewsProfile GetNewsProfile();
        public DMODatabase Database;
        public List<server> ServerList;
        #endregion
    }
}