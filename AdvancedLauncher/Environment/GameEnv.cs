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
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using AdvancedLauncher.Service;
using DMOLibrary.DMOFileSystem;
using Microsoft.Win32;

namespace AdvancedLauncher.Environment {
    public class GameEnv : INotifyPropertyChanged {
        public enum GameType {
            ADMO = 0,
            GDMO = 1,
            KDMO_IMBC = 2,
            KDMO_DM = 3
        }

        [XmlAttribute("Type")]
        public GameType Type;
        [XmlElement("GamePath")]
        public string pGamePath;
        [XmlElement("DefLauncherPath")]
        public string pDefLauncherPath;

        [XmlIgnore]
        public string GamePath {
            set {
                pGamePath = value; Initialize(); NotifyPropertyChanged("GamePath");
            }
            get {
                return pGamePath;
            }
        }

        [XmlIgnore]
        public string DefLauncherPath {
            set {
                pDefLauncherPath = value; Initialize(); NotifyPropertyChanged("DefLauncherPath");
            }
            get {
                return pDefLauncherPath;
            }
        }

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

        private string puPF;
        private string puHF;
        private string puImportDir;

        [XmlIgnore]
        public bool IsInitialized = false;

        #region Load Section

        public GameEnv() {
        }
        public GameEnv(GameEnv gameEnv) {
            this.Type = gameEnv.Type;
            this.pGamePath = gameEnv.pGamePath;
            this.pDefLauncherPath = gameEnv.pDefLauncherPath;
        }

        public void Initialize() {
            IsInitialized = true;
            LoadType(Type);
            if (string.IsNullOrEmpty(pGamePath)) {
                pGamePath = GetGamePathFromRegistry();
            }
            if (string.IsNullOrEmpty(pDefLauncherPath)) {
                pDefLauncherPath = GetDefLauncherPathFromRegistry();
            }
        }

        public void LoadType(GameType type) {
            this.Type = type;
            puPF = @"Data\Pack01.pf";
            puHF = @"Data\Pack01.hf";
            puImportDir = @"Pack01";
            pLastSessionAvailable = false;

            switch (type) {
                case GameType.ADMO: {
                        pGamePathRegKey = pDefLauncherPathRegKey = "Software\\Aeria Games\\DMO";
                        pGamePathRegVal = pDefLauncherPathRegVal = "Path";
                        pGameEXE = "GDMO.exe";
                        pDefLauncherEXE = "DMLauncher.exe";
                        puRemoteVer = "http://patch.dmo.joymax.com/Aeria/PatchInfo_GDMO.ini";
                        puRemotePatch = "http://patch.dmo.joymax.com/Aeria/GDMO{0}.zip";
                        puLocalVer = @"LauncherLib\vGDMO.ini";
                        break;
                    }
                case GameType.GDMO: {
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
                case GameType.KDMO_IMBC: {
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

        #endregion

        #region Check Section

        public bool CheckGame() {
            if (!IsInitialized) {
                Initialize();
            }
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

        public bool CheckGame(string pGamePath) {
            if (!IsInitialized) {
                Initialize();
            }
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
            if (!IsInitialized) {
                Initialize();
            }
            if (string.IsNullOrEmpty(pDefLauncherPath)) {
                return false;
            }
            if (!File.Exists(Path.Combine(pDefLauncherPath, pDefLauncherEXE))) {
                return false;
            }
            return true;
        }

        public bool CheckDefLauncher(string pDefLauncherPath) {
            if (!IsInitialized) {
                Initialize();
            }
            if (string.IsNullOrEmpty(pDefLauncherPath)) {
                return false;
            }
            if (!File.Exists(Path.Combine(pDefLauncherPath, pDefLauncherEXE))) {
                return false;
            }
            return true;
        }

        public bool CheckUpdateAccess() {
            return !Utils.IsFileLocked(GetGameEXE()) && !Utils.IsFileLocked(GetPFPath()) && !Utils.IsFileLocked(GetHFPath());
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

        #endregion

        #region Get/Set Section

        public string GetGameEXE() {
            if (!IsInitialized)
                Initialize();
            if (string.IsNullOrEmpty(pGamePath))
                return null;
            if (string.IsNullOrEmpty(pGameEXE))
                return null;
            return Path.Combine(pGamePath, pGameEXE);
        }

        public string GetDefLauncherEXE() {
            if (!IsInitialized)
                Initialize();
            if (string.IsNullOrEmpty(DefLauncherPath))
                return null;
            if (string.IsNullOrEmpty(pDefLauncherEXE))
                return null;
            return Path.Combine(DefLauncherPath, pDefLauncherEXE);
        }

        public string GetPFPath() {
            if (!IsInitialized)
                Initialize();
            if (string.IsNullOrEmpty(pGamePath))
                return null;
            return Path.Combine(pGamePath, puPF);
        }

        public string GetHFPath() {
            if (!IsInitialized)
                Initialize();
            if (string.IsNullOrEmpty(pGamePath))
                return null;
            return Path.Combine(pGamePath, puHF);
        }
        public string GetImportPath() {
            if (!IsInitialized)
                Initialize();
            if (string.IsNullOrEmpty(pGamePath))
                return null;
            return Path.Combine(pGamePath, puImportDir);
        }

        public string GetLocalVerFile() {
            if (!IsInitialized)
                Initialize();
            if (string.IsNullOrEmpty(pGamePath))
                return null;
            return Path.Combine(pGamePath, puLocalVer);
        }
        public string GetRemoteVerURL() {
            if (!IsInitialized)
                Initialize();
            if (string.IsNullOrEmpty(puRemoteVer))
                return null;
            return puRemoteVer;
        }
        public string GetPatchURL() {
            if (!IsInitialized)
                Initialize();
            if (string.IsNullOrEmpty(puRemotePatch))
                return null;
            return puRemotePatch;
        }
        public bool IsLastSessionAvailable() {
            if (!IsInitialized)
                Initialize();
            return pLastSessionAvailable;
        }

        #endregion

        #region Property change handler
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

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
            if (!string.IsNullOrEmpty(pGamePath)) {
                RegistryKey reg = Registry.CurrentUser.CreateSubKey(pGamePathRegKey);
                reg.SetValue(pGamePathRegVal, pGamePath);
                reg.Close();
            }

            if (!string.IsNullOrEmpty(pDefLauncherPath)) {
                RegistryKey reg = Registry.CurrentUser.CreateSubKey(pDefLauncherPathRegKey);
                reg.SetValue(pDefLauncherPathRegVal, pDefLauncherPath);
                reg.Close();
            }
        }

        #endregion
    }
}
