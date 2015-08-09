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

namespace AdvancedLauncher.SDK.Management.Plugins {

    /// <summary>
    /// Base <see cref="IPlugin"/> implementation
    /// </summary>
    /// <seealso cref="IPluginHost"/>
    /// <seealso cref="IPlugin"/>
    public abstract class AbstractPlugin : CrossDomainObject, IPlugin {

        /// <summary>
        /// Gets name of plugin
        /// </summary>
        public abstract string Name {
            get;
        }

        /// <summary>
        /// Gets author's name of plugin
        /// </summary>
        public abstract string Author {
            get;
        }

        /// <summary>
        /// Plugin activation entry point
        /// </summary>
        /// <param name="PluginHost">The <see cref="IPluginHost"/> interface with accessable API</param>
        public abstract void OnActivate(IPluginHost PluginHost);

        /// <summary>
        /// Plugin stop entry point. You MUST free all your resources here,
        /// remove added menus, windows, pages, configurations, etc.
        /// </summary>
        /// <param name="PluginHost">The <see cref="IPluginHost"/> interface with accessable API</param>
        public abstract void OnStop(IPluginHost PluginHost);
    }
}