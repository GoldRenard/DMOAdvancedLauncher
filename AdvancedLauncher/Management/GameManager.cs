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
using System.IO;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Model.Config;
using AdvancedLauncher.Tools;
using DMOLibrary.DMOFileSystem;
using Microsoft.Win32;
using Ninject;

namespace AdvancedLauncher.Management {

    public class GameManager : IGameManager {

        public enum GameType {
            ADMO = 0,
            GDMO = 1,
            KDMO_IMBC = 2,
            KDMO_DM = 3,
        }

        private const string puPF = @"Data\Pack01.pf";
        private const string puHF = @"Data\Pack01.hf";
        private const string puImportDir = @"Pack01";

        private ConcurrentDictionary<GameModel, DMOFileSystem> FileSystems = new ConcurrentDictionary<GameModel, DMOFileSystem>();
        private ConcurrentDictionary<GameType, IGameConfiguration> Configurations = new ConcurrentDictionary<GameType, IGameConfiguration>();

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        public void Initialize() {
            foreach (IGameConfiguration config in App.Kernel.GetAll<IGameConfiguration>()) {
                Configurations.TryAdd(config.GameType, config);
            }
        }

        #region Check Section

        public bool CheckGame(GameModel model) {
            IGameConfiguration config = GetConfiguration(model);
            string pGamePath = GetGamePath(model);
            if (string.IsNullOrEmpty(pGamePath)) {
                return false;
            }
            if (!File.Exists(Path.Combine(pGamePath, config.VarsionLocalPath)) || !File.Exists(Path.Combine(pGamePath, config.GameExecutable))) {
                return false;
            }
            if (!File.Exists(Path.Combine(pGamePath, puPF)) || !File.Exists(Path.Combine(pGamePath, puHF))) {
                return false;
            }
            return true;
        }

        public bool CheckLauncher(GameModel model) {
            IGameConfiguration config = GetConfiguration(model);
            string pLauncherPath = GetLauncherPath(model);
            if (string.IsNullOrEmpty(pLauncherPath)) {
                return false;
            }
            if (!File.Exists(Path.Combine(pLauncherPath, config.LauncherExecutable))) {
                return false;
            }
            return true;
        }

        public bool CheckUpdateAccess(GameModel model) {
            return !Utils.IsFileLocked(GetGameEXE(model)) && !Utils.IsFileLocked(GetPFPath(model)) && !Utils.IsFileLocked(GetHFPath(model));
        }

        public DMOFileSystem GetFileSystem(GameModel model) {
            DMOFileSystem fileSystem;
            if (FileSystems.TryGetValue(model, out fileSystem)) {
                return fileSystem;
            }
            fileSystem = new DMOFileSystem();
            FileSystems.TryAdd(model, fileSystem);
            return fileSystem;
        }

        public bool OpenFileSystem(GameModel model, FileAccess fAccess) {
            return GetFileSystem(model).Open(fAccess, 16, GetHFPath(model), GetPFPath(model));
        }

        #endregion Check Section

        #region Getters/Setters

        public string GetImportPath(GameModel model) {
            string path = GetGamePath(model);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return Path.Combine(path, puImportDir);
        }

        public string GetLocalVersionFile(GameModel model) {
            IGameConfiguration config = GetConfiguration(model);
            string path = GetGamePath(model);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return Path.Combine(path, config.VarsionLocalPath);
        }

        public string GetPFPath(GameModel model) {
            string path = GetGamePath(model);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return Path.Combine(path, puPF);
        }

        public string GetHFPath(GameModel model) {
            string path = GetGamePath(model);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return Path.Combine(path, puHF);
        }

        public string GetGameEXE(GameModel model) {
            IGameConfiguration config = GetConfiguration(model);
            string gamePath = GetGamePath(model);
            if (string.IsNullOrEmpty(gamePath)) {
                return null;
            }
            return Path.Combine(gamePath, config.GameExecutable);
        }

        public string GetLauncherEXE(GameModel model) {
            IGameConfiguration config = GetConfiguration(model);
            string path = GetLauncherPath(model);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return Path.Combine(path, config.LauncherExecutable);
        }

        public string GetLauncherPath(GameModel model) {
            IGameConfiguration config = GetConfiguration(model);
            if (string.IsNullOrEmpty(model.DefLauncherPath)) {
                return config.GetLauncherPathFromRegistry();
            }
            return model.DefLauncherPath;
        }

        public string GetGamePath(GameModel model) {
            IGameConfiguration config = GetConfiguration(model);
            if (string.IsNullOrEmpty(model.GamePath)) {
                return config.GetGamePathFromRegistry();
            }
            return model.GamePath;
        }

        public IGameConfiguration GetConfiguration(GameModel model) {
            IGameConfiguration config;
            if (Configurations.TryGetValue(model.Type, out config)) {
                return config;
            }
            throw new Exception("No game configuration for type: " + model.Type);
        }

        public void UpdateRegistryPaths(GameModel model) {
            IGameConfiguration config = GetConfiguration(model);
            string gamePath = GetGamePath(model);
            string launcherPath = GetGamePath(model);

            if (!string.IsNullOrEmpty(gamePath)) {
                RegistryKey reg = Registry.CurrentUser.CreateSubKey(config.GamePathRegKey);
                reg.SetValue(config.GamePathRegVal, gamePath);
                reg.Close();
            }

            if (!string.IsNullOrEmpty(launcherPath)) {
                RegistryKey reg = Registry.CurrentUser.CreateSubKey(config.LauncherPathRegKey);
                reg.SetValue(config.LauncherPathRegVal, launcherPath);
                reg.Close();
            }
        }

        #endregion Getters/Setters
    }
}