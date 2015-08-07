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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using AdvancedLauncher.Model;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Plugins;
using Ninject;

namespace AdvancedLauncher.Management.Internal {

    internal sealed class PluginManager {

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get;
            set;
        }

        [Inject]
        public IPluginHost PluginHost {
            get;
            set;
        }

        private ConcurrentDictionary<string, PluginContainer> Plugins = new ConcurrentDictionary<string, PluginContainer>();

        public void Start() {
            var pluginInfos = LoadFrom(EnvironmentManager.PluginsPath);
            foreach (PluginInfo pluginInfo in pluginInfos) {
                LoadPlugin(pluginInfo);
            }
        }

        private List<PluginInfo> LoadFrom(string pluginsDirectory) {
            string[] pluginList = Directory.GetFiles(pluginsDirectory, "*.dll");

            AppDomainSetup domainSetup = new AppDomainSetup();
            domainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            domainSetup.PrivateBinPath = "Plugins;bin";

            PermissionSet permissions = new PermissionSet(PermissionState.None);
            permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery | FileIOPermissionAccess.Read, pluginList));

            List<PluginInfo> result;
            var pluginLoader = AppDomain.CreateDomain("PluginLoader", null, domainSetup, permissions);
            try {
                string engineAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AdvancedLauncher.SDK.dll");
                Proxy proxy = (Proxy)pluginLoader.CreateInstanceAndUnwrap(AssemblyName.GetAssemblyName(engineAssemblyPath).FullName, typeof(Proxy).FullName);
                proxy.PluginInfos = new List<PluginInfo>();
                proxy.PluginLibs = pluginList;
                proxy.LoadInfos();
                result = proxy.PluginInfos;
            } finally {
                AppDomain.Unload(pluginLoader);
            }
            return result;
        }

        private bool LoadPlugin(PluginInfo info) {
            AppDomainSetup domainSetup = new AppDomainSetup();
            domainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            domainSetup.PrivateBinPath = @"Plugins;bin";

            PermissionSet permissions = new PermissionSet(PermissionState.None);
            permissions.AddPermission(new UIPermission(PermissionState.Unrestricted));
            permissions.AddPermission(new WebPermission(PermissionState.Unrestricted));
            permissions.AddPermission(new WebBrowserPermission(PermissionState.Unrestricted));
            permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));

            permissions.AddPermission(new SecurityPermission(
              SecurityPermissionFlag.Execution |
              SecurityPermissionFlag.UnmanagedCode |
              SecurityPermissionFlag.SerializationFormatter |
              SecurityPermissionFlag.Assertion));

            permissions.AddPermission(new FileIOPermission(
              FileIOPermissionAccess.PathDiscovery |
              FileIOPermissionAccess.Read,
              new string[] {
                  AppDomain.CurrentDomain.BaseDirectory,
                  EnvironmentManager.ResourcesPath,
              }));

            permissions.AddPermission(new FileIOPermission(
              FileIOPermissionAccess.PathDiscovery |
              FileIOPermissionAccess.Write |
              FileIOPermissionAccess.Read,
              EnvironmentManager.Resources3rdPath));

            // debug = REMOVE
            //permissions.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
            //permissions.AddPermission(new SecurityPermission(PermissionState.Unrestricted));

            //permissions = new PermissionSet(PermissionState.Unrestricted);

            AppDomain domain = AppDomain.CreateDomain(
              string.Format("PluginDomain [{0}]", Path.GetFileNameWithoutExtension(info.AssemblyPath)),
              null,
              domainSetup,
              permissions);
            domain.SetData("DataDirectory", EnvironmentManager.AppDataPath);

            PluginContainer container = null;
            string pluginName = null;
            IPlugin Instance = null;
            try {
                Instance = (IPlugin)domain.CreateInstanceFromAndUnwrap(info.AssemblyPath, info.TypeName);
                pluginName = Instance.Name;

                if (Plugins.TryGetValue(pluginName, out container)) {
                    if (container.Status == PluginContainer.RuntimeStatus.ACTIVE) {
                        AppDomain.Unload(domain);
                        return false;
                    }
                }
                try {
                    Instance.OnActivate(PluginHost);
                } finally {
                    Instance.OnStop(PluginHost);
                }
                container = new PluginContainer(Instance, info, PluginContainer.RuntimeStatus.ACTIVE, domain);
            } catch (Exception e) {
                AppDomain.Unload(domain);
                if (pluginName != null && Instance != null) {
                    container = new PluginContainer(Instance, info, PluginContainer.RuntimeStatus.FAILED, e);
                }
                return false;
            }
            return Plugins.AddOrUpdate(pluginName, container, (key, oldValue) => container) != null;
        }

        public List<PluginContainer> GetPlugins() {
            return new List<PluginContainer>(Plugins.Values);
        }

        public bool StopPlugin(PluginContainer container) {
            bool result = false;
            if (container.Status == PluginContainer.RuntimeStatus.ACTIVE) {
                container.Plugin.OnStop(PluginHost);
                AppDomain.Unload(container.Domain);
                container = new PluginContainer(container, PluginContainer.RuntimeStatus.STOPPED);
                result = Plugins.AddOrUpdate(container.Name, container, (key, oldValue) => container) != null;
            }
            return result;
        }

        public bool StartPlugin(PluginContainer container) {
            return LoadPlugin(container.Info);
        }
    }
}