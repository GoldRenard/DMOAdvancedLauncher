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
using System.Collections.Generic;
using System.Reflection;

namespace AdvancedLauncher.SDK.Management.Plugins {

    /// <summary>
    /// Plugin loader proxy for internal use
    /// </summary>
    public class Proxy : CrossDomainObject {

        /// <summary>
        /// Patch to plugin assemblies to scan
        /// </summary>
        public string[] PluginLibs {
            get; set;
        }

        /// <summary>
        /// Loaded information structures of plugins found
        /// </summary>
        public List<PluginInfo> PluginInfos {
            get; set;
        }

        /// <summary>
        /// Scans specified <see cref="PluginLibs"/> for types implementing <see cref="IPlugin"/>.
        /// </summary>
        public void LoadInfos() {
            Type pluginType = typeof(IPlugin);
            foreach (var assemblyPath in PluginLibs) {
                try {
                    var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(assemblyPath).FullName);
                    byte[] asmToken = assembly.GetName().GetPublicKeyToken();
                    foreach (Type type in assembly.GetExportedTypes()) {
                        if (type.IsAbstract) {
                            continue;
                        }
                        if (pluginType.IsAssignableFrom(type)) {
                            PluginInfos.Add(new PluginInfo(assemblyPath, type.FullName, asmToken));
                        }
                    }
                } catch (Exception) {
                    continue;
                }
            }
        }
    }
}