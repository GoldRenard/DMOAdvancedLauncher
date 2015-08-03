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

    public class Proxy : CrossDomainObject {

        public string[] PluginLibs {
            get; set;
        }

        public List<PluginInfo> PluginInfos {
            get; set;
        }

        public void LoadInfos() {
            Type pluginType = typeof(IPlugin);
            foreach (var assemblyPath in PluginLibs) {
                var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(assemblyPath).FullName);
                foreach (Type type in assembly.GetExportedTypes()) {
                    if (type.IsAbstract) {
                        continue;
                    }
                    if (pluginType.IsAssignableFrom(type)) {
                        PluginInfos.Add(new PluginInfo(assemblyPath, type.FullName));
                    }
                }
            }
        }
    }
}