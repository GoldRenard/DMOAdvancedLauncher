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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using AdvancedLauncher.Model.Config;
using DMOLibrary.DMOFileSystem;
using DMOLibrary.Profiles;
using Microsoft.Win32;

namespace AdvancedLauncher.Management {

    public class GameManager : INotifyPropertyChanged, IDisposable {

        public enum GameType {
            ADMO = 0,
            GDMO = 1,
            KDMO_IMBC = 2,
            KDMO_DM = 3,
        }

        private const string puPF = @"Data\Pack01.pf";
        private const string puHF = @"Data\Pack01.hf";
        private const string puImportDir = @"Pack01";

        private static ConcurrentDictionary<GameModel, GameManager> Collection = new ConcurrentDictionary<GameModel, GameManager>();
        private static ConcurrentDictionary<GameType, DMOProfile> DMOProfiles = new ConcurrentDictionary<GameType, DMOProfile>();

        private bool pLastSessionAvailable;
        private string pGamePathRegKey;
        private string pGamePathRegVal;
        private string pDefLauncherPathRegKey;
        private string pDefLauncherPathRegVal;
        private string pGameEXE;
        private string pDefLauncherEXE;
        private string puRemoteVer;
        private string puRemotePatch;
        private string puLocalVer;

        #region Properties

        private GameModel _Model;

        public GameModel Model {
            get {
                return _Model;
            }
        }

        public string GamePath {
            set {
                Model.GamePath = value; NotifyPropertyChanged("GamePath");
            }
            get {
                return Model.GamePath;
            }
        }

        public string DefLauncherPath {
            set {
                Model.DefLauncherPath = value; NotifyPropertyChanged("DefLauncherPath");
            }
            get {
                return Model.DefLauncherPath;
            }
        }

        private static GameManager _Current;

        public static GameManager Current {
            get {
                if (_Current == null) {
                    _Current = Get(ProfileManager.Instance.CurrentProfile.GameModel);
                }
                return _Current;
            }
        }

        public static DMOProfile CurrentProfile {
            get {
                return GetProfile(ProfileManager.Instance.CurrentProfile.GameModel.Type);
            }
        }

        #endregion Properties

        #region Load Section

        private GameManager() {
        }

        private GameManager(GameModel model) {
            this._Model = model;
            LoadType(Model.Type);
            if (string.IsNullOrEmpty(Model.GamePath)) {
                Model.GamePath = GetGamePathFromRegistry();
            }
            if (string.IsNullOrEmpty(Model.DefLauncherPath)) {
                Model.DefLauncherPath = GetDefLauncherPathFromRegistry();
            }
        }

        public static DMOProfile GetProfile(GameType type) {
            DMOProfile profile;
            if (DMOProfiles.ContainsKey(type)) {
                DMOProfiles.TryGetValue(type, out profile);
            } else {
                switch (type) {
                    case GameType.ADMO:
                        profile = new DMOLibrary.Profiles.Aeria.DMOAeria();
                        break;

                    case GameType.GDMO:
                        profile = new DMOLibrary.Profiles.Joymax.DMOJoymax();
                        break;

                    case GameType.KDMO_DM:
                        profile = new DMOLibrary.Profiles.Korea.DMOKorea();
                        break;

                    case GameType.KDMO_IMBC:
                        profile = new DMOLibrary.Profiles.Korea.DMOKoreaIMBC();
                        break;

                    default:
                        return null;
                }
                DMOProfiles.TryAdd(type, profile);
            }
            return profile;
        }

        public static GameManager Get(GameModel model) {
            GameManager result = null;
            Collection.TryGetValue(model, out result);
            if (result == null) {
                result = new GameManager(model);
                Collection.TryAdd(new GameModel(model), result);
            }
            return result;
        }

        static GameManager() {
            ProfileManager.Instance.ProfileChanged += (s, e) => {
                _Current = null;
            };
        }

        private void LoadType(GameType type) {
            pLastSessionAvailable = false;
            switch (type) {
                case GameType.ADMO:
                    {
                        pGamePathRegKey = pDefLauncherPathRegKey = "Software\\Aeria Games\\DMO";
                        pGamePathRegVal = pDefLauncherPathRegVal = "Path";
                        pGameEXE = "GDMO.exe";
                        pDefLauncherEXE = "DMLauncher.exe";
                        puRemoteVer = "http://patch.dmo.joymax.com/Aeria/PatchInfo_GDMO.ini";
                        puRemotePatch = "http://patch.dmo.joymax.com/Aeria/GDMO{0}.zip";
                        puLocalVer = @"LauncherLib\vGDMO.ini";
                        break;
                    }
                case GameType.GDMO:
                    {
                        pGamePathRegKey = pDefLauncherPathRegKey = "Software\\Joymax\\DMO";
                        pGamePathRegVal = pDefLauncherPathRegVal = "Path";
                        pGameEXE = "GDMO.exe";
                        pDefLauncherEXE = "DMLauncher.exe";
                        puRemoteVer = "http://patch.dmo.joymax.com/PatchInfo_GDMO.ini";
                        puRemotePatch = "http://patch.dmo.joymax.com/GDMO{0}.zip";
                        puLocalVer = @"LauncherLib\vGDMO.ini";
                        break;
                    }
                case GameType.KDMO_DM:
                case GameType.KDMO_IMBC:
                    {
                        pGamePathRegKey = "Software\\Digitalic\\DigimonMasters";
                        pGamePathRegVal = "Path";
                        pDefLauncherPathRegKey = "Software\\Digitalic\\Launcher";
                        pDefLauncherPathRegVal = "Launcher";
                        pGameEXE = "DigimonMasters.exe";
                        pDefLauncherEXE = "D-Player.exe";
                        puRemoteVer = "http://digimonmasters.nowcdn.co.kr/s1/PatchInfo.ini";
                        puRemotePatch = "http://digimonmasters.nowcdn.co.kr/s1/{0}.zip";
                        puLocalVer = @"LauncherLib\vDMO.ini";
                        pLastSessionAvailable = true;
                        break;
                    }
            }
        }

        #endregion Load Section

        #region Check Section

        /// <summary> Checks access to file </summary>
        /// <param name="file">Full path to file</param>
        /// <returns> <see langword="True"/> if file is locked </returns>
        public static bool IsFileLocked(string file) {
            FileStream stream = null;

            try {
                stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            } catch (IOException) {
                return true;
            } finally {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

        public bool CheckGame() {
            return CheckGame(Model.GamePath);
        }

        public bool CheckGame(string pGamePath) {
            if (string.IsNullOrEmpty(pGamePath)) {
                return false;
            }
            if (!File.Exists(Path.Combine(pGamePath, puLocalVer)) || !File.Exists(Path.Combine(pGamePath, pGameEXE))) {
                return false;
            }
            if (!File.Exists(Path.Combine(pGamePath, puPF)) || !File.Exists(Path.Combine(pGamePath, puHF))) {
                return false;
            }
            return true;
        }

        public bool CheckDefLauncher() {
            return CheckDefLauncher(Model.DefLauncherPath);
        }

        public bool CheckDefLauncher(string pDefLauncherPath) {
            if (string.IsNullOrEmpty(pDefLauncherPath)) {
                return false;
            }
            if (!File.Exists(Path.Combine(pDefLauncherPath, pDefLauncherEXE))) {
                return false;
            }
            return true;
        }

        public bool CheckUpdateAccess() {
            return !IsFileLocked(GetGameEXE()) && !IsFileLocked(GetPFPath()) && !IsFileLocked(GetHFPath());
        }

        private DMOFileSystem _GameFS = null;

        public DMOFileSystem GetFS() {
            if (_GameFS == null) {
                _GameFS = new DMOFileSystem();
            }
            return _GameFS;
        }

        public bool OpenFS(FileAccess fAccess) {
            return GetFS().Open(fAccess, 16, GetHFPath(), GetPFPath());
        }

        #endregion Check Section

        #region Get/Set Section

        public string GetGameEXE() {
            if (string.IsNullOrEmpty(Model.GamePath)) {
                return null;
            }
            return Path.Combine(Model.GamePath, pGameEXE);
        }

        public string GetDefLauncherEXE() {
            if (string.IsNullOrEmpty(DefLauncherPath)) {
                return null;
            }
            return Path.Combine(DefLauncherPath, pDefLauncherEXE);
        }

        public string GetPFPath() {
            if (string.IsNullOrEmpty(Model.GamePath)) {
                return null;
            }
            return Path.Combine(Model.GamePath, puPF);
        }

        public string GetHFPath() {
            if (string.IsNullOrEmpty(Model.GamePath)) {
                return null;
            }
            return Path.Combine(Model.GamePath, puHF);
        }

        public string GetImportPath() {
            if (string.IsNullOrEmpty(Model.GamePath)) {
                return null;
            }
            return Path.Combine(Model.GamePath, puImportDir);
        }

        public string GetLocalVerFile() {
            if (string.IsNullOrEmpty(Model.GamePath)) {
                return null;
            }
            return Path.Combine(Model.GamePath, puLocalVer);
        }

        public string GetRemoteVerURL() {
            return puRemoteVer;
        }

        public string GetPatchURL() {
            return puRemotePatch;
        }

        public bool IsLastSessionAvailable() {
            return pLastSessionAvailable;
        }

        #endregion Get/Set Section

        #region Property change handler

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Property change handler

        #region Registry

        public string GetGamePathFromRegistry() {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(pGamePathRegKey);
            string path = (string)reg.GetValue(pGamePathRegVal);
            reg.Close();
            return path;
        }

        public string GetDefLauncherPathFromRegistry() {
            RegistryKey reg = Registry.CurrentUser.CreateSubKey(pDefLauncherPathRegKey);
            string path = (string)reg.GetValue(pDefLauncherPathRegVal);
            reg.Close();
            return path;
        }

        public void SetRegistryPaths() {
            if (!string.IsNullOrEmpty(Model.GamePath)) {
                RegistryKey reg = Registry.CurrentUser.CreateSubKey(pGamePathRegKey);
                reg.SetValue(pGamePathRegVal, Model.GamePath);
                reg.Close();
            }

            if (!string.IsNullOrEmpty(Model.DefLauncherPath)) {
                RegistryKey reg = Registry.CurrentUser.CreateSubKey(pDefLauncherPathRegKey);
                reg.SetValue(pDefLauncherPathRegVal, Model.DefLauncherPath);
                reg.Close();
            }
        }

        #endregion Registry

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose) {
            if (dispose) {
                if (_GameFS != null) {
                    _GameFS.Dispose();
                }
            }
        }
    }
}