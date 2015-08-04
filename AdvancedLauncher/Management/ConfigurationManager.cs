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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.Tools;
using Microsoft.Win32;
using Ninject;

namespace AdvancedLauncher.Management {

    public class ConfigurationManager : CrossDomainObject, IConfigurationManager {
        private const string puPF = @"Data\Pack01.pf";
        private const string puHF = @"Data\Pack01.hf";
        private const string puImportDir = @"Pack01";

        private ConcurrentDictionary<string, IConfiguration> Configurations = new ConcurrentDictionary<string, IConfiguration>();

        public event ConfigurationChangedEventHandler ConfigurationRegistered;

        public event ConfigurationChangedEventHandler ConfigurationUnRegistered;

        public void Initialize() {
            foreach (IConfiguration config in App.Kernel.GetAll<IConfiguration>()) {
                RegisterConfiguration(config);
            }
        }

        #region Check Section

        public bool CheckGame(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            IConfiguration config = GetConfiguration(model);
            string pGamePath = GetGamePath(model);

            new FileIOPermission(FileIOPermissionAccess.Read, pGamePath).Assert();
            if (string.IsNullOrEmpty(pGamePath)) {
                return false;
            }
            if (!File.Exists(Path.Combine(pGamePath, config.VersionLocalPath)) || !File.Exists(Path.Combine(pGamePath, config.GameExecutable))) {
                return false;
            }
            if (!File.Exists(Path.Combine(pGamePath, puPF)) || !File.Exists(Path.Combine(pGamePath, puHF))) {
                return false;
            }
            return true;
        }

        public bool CheckLauncher(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            IConfiguration config = GetConfiguration(model);
            string pLauncherPath = GetLauncherPath(model);
            if (string.IsNullOrEmpty(pLauncherPath)) {
                return false;
            }

            new FileIOPermission(FileIOPermissionAccess.Read, pLauncherPath).Assert();
            if (!File.Exists(Path.Combine(pLauncherPath, config.LauncherExecutable))) {
                return false;
            }
            return true;
        }

        public bool CheckUpdateAccess(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            string gameEXE = GetGameEXE(model);
            string pfFile = GetPFPath(model);
            string hfFile = GetHFPath(model);
            new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, new string[] { gameEXE, pfFile, hfFile }).Assert();
            return !Utils.IsFileLocked(gameEXE) && !Utils.IsFileLocked(pfFile) && !Utils.IsFileLocked(hfFile);
        }

        #endregion Check Section

        #region Getters/Setters

        public string GetImportPath(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            string path = GetGamePath(model);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return Path.Combine(path, puImportDir);
        }

        public string GetLocalVersionFile(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            IConfiguration config = GetConfiguration(model);
            string path = GetGamePath(model);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return Path.Combine(path, config.VersionLocalPath);
        }

        public string GetPFPath(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            string path = GetGamePath(model);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return Path.Combine(path, puPF);
        }

        public string GetHFPath(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            string path = GetGamePath(model);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return Path.Combine(path, puHF);
        }

        public string GetGameEXE(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            IConfiguration config = GetConfiguration(model);
            string gamePath = GetGamePath(model);
            if (string.IsNullOrEmpty(gamePath)) {
                return null;
            }
            return Path.Combine(gamePath, config.GameExecutable);
        }

        public string GetLauncherEXE(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            IConfiguration config = GetConfiguration(model);
            string path = GetLauncherPath(model);
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return Path.Combine(path, config.LauncherExecutable);
        }

        public string GetLauncherPath(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            IConfiguration config = GetConfiguration(model);
            if (string.IsNullOrEmpty(model.LauncherPath)) {
                return GetLauncherPathFromRegistry(config);
            }
            return model.LauncherPath;
        }

        public string GetGamePath(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            IConfiguration config = GetConfiguration(model);
            if (string.IsNullOrEmpty(model.GamePath)) {
                return GetGamePathFromRegistry(config);
            }
            return model.GamePath;
        }

        public IConfiguration GetConfiguration(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            return GetConfiguration(model.Type);
        }

        public IConfiguration GetConfiguration(string gameType) {
            if (gameType == null) {
                throw new ArgumentException("gameType argument cannot be null");
            }
            IConfiguration config;
            if (Configurations.TryGetValue(gameType, out config)) {
                return config;
            }
            return null;
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public bool RegisterConfiguration(IConfiguration configuration) {
            if (configuration == null) {
                throw new ArgumentException("configuration argument cannot be null");
            }
            if (Configurations.ContainsKey(configuration.GameType)) {
                throw new Exception(String.Format("Configuration with type {0} already registered!", configuration.GameType));
            }

            bool result = Configurations.TryAdd(configuration.GameType, configuration);
            if (result) {
                OnConfigurationRegistered(configuration);
            }
            return result;
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public bool UnRegisterConfiguration(IConfiguration configuration) {
            if (configuration == null) {
                throw new ArgumentException("configuration argument cannot be null");
            }
            var configToRemove = Configurations.FirstOrDefault(kvp => kvp.Value.Equals(configuration));
            if (configToRemove.Key != null) {
                bool result = Configurations.TryRemove(configToRemove.Key, out configuration);
                if (result) {
                    OnConfigurationUnRegistered(configuration);
                }
                return result;
            }
            return false;
        }

        #region Registry

        public string GetGamePathFromRegistry(IConfiguration config) {
            try {
                using (RegistryKey reg = Registry.CurrentUser.OpenSubKey(config.GamePathRegKey)) {
                    if (reg == null) {
                        return null;
                    }
                    return (string)reg.GetValue(config.GamePathRegVal);
                }
            } catch (SecurityException) {
                return null;
            }
        }

        public string GetLauncherPathFromRegistry(IConfiguration config) {
            try {
                using (RegistryKey reg = Registry.CurrentUser.OpenSubKey(config.LauncherPathRegKey)) {
                    if (reg == null) {
                        return null;
                    }
                    return (string)reg.GetValue(config.LauncherPathRegVal);
                }
            } catch (SecurityException) {
                return null;
            }
        }

        public void UpdateRegistryPaths(GameModel model) {
            if (model == null) {
                throw new ArgumentException("model argument cannot be null");
            }
            IConfiguration config = GetConfiguration(model);
            string gamePath = GetGamePath(model);
            string launcherPath = GetGamePath(model);

            try {
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
            } catch (SecurityException) {
                // ignore
            }
        }

        #endregion Registry

        public IEnumerator<IConfiguration> GetEnumerator() {
            return Configurations.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Configurations.Values.GetEnumerator();
        }

        protected virtual void OnConfigurationRegistered(IConfiguration configuration) {
            if (ConfigurationRegistered != null) {
                ConfigurationRegistered(this, new ConfigurationChangedEventArgs(configuration));
            }
        }

        protected virtual void OnConfigurationUnRegistered(IConfiguration configuration) {
            if (ConfigurationUnRegistered != null) {
                ConfigurationUnRegistered(this, new ConfigurationChangedEventArgs(configuration));
            }
        }

        #endregion Getters/Setters
    }
}