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
using AdvancedLauncher.SDK.Management.Plugins;

namespace AdvancedLauncher.Model {

    internal class PluginContainer {

        public enum RuntimeStatus {
            ACTIVE,
            STOPPED,
            FAILED
        };

        public string Name {
            get; private set;
        }

        public string Author {
            get; private set;
        }

        public IPlugin Plugin {
            get; private set;
        }

        public PluginInfo Info {
            get; private set;
        }

        public RuntimeStatus Status {
            get; private set;
        }

        public AppDomain Domain {
            get; private set;
        }

        public Exception FailException {
            get; private set;
        }

        public PluginContainer(IPlugin Plugin, PluginInfo Info, RuntimeStatus Status, AppDomain Domain)
            : this(Plugin, Info, Status, Domain, null) {
        }

        public PluginContainer(IPlugin Plugin, PluginInfo Info, RuntimeStatus Status, Exception FailException)
            : this(Plugin, Info, Status, null, FailException) {
        }

        private PluginContainer(IPlugin Plugin, PluginInfo Info, RuntimeStatus Status, AppDomain Domain, Exception FailException) {
            this.Name = Plugin.Name;
            this.Author = Plugin.Author;
            this.Plugin = Plugin;
            this.Info = Info;
            this.Status = Status;
            this.Domain = Domain;
            this.FailException = FailException;
        }

        public PluginContainer(PluginContainer container, RuntimeStatus Status) {
            this.Name = container.Name;
            this.Author = container.Author;
            this.Info = container.Info;
            this.Status = Status;
        }
    }
}