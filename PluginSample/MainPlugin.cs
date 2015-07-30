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

using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Commands;
using AdvancedLauncher.SDK.Management.Plugins;

namespace PluginSample {

    public class MainPlugin : IPlugin {

        public class TestCommand : ICommand {
            private readonly IPluginHost PluginHost;

            public TestCommand(IPluginHost PluginHost) {
                this.PluginHost = PluginHost;
            }

            public bool DoCommand(string[] args) {
                PluginHost.LogManager.Info("Did it!");
                return true;
            }

            public string GetDescription() {
                return "Just the test command";
            }

            public string GetName() {
                return "doit";
            }
        }

        public string Author {
            get {
                return "GoldRenard";
            }
        }

        public string Name {
            get {
                return "SimplePlugin";
            }
        }

        private ICommand DoItCommand;

        public void OnActivate(IPluginHost PluginHost) {
            DoItCommand = new TestCommand(PluginHost);
            PluginHost.CommandManager.RegisterCommand(DoItCommand);
        }

        public void OnStop(IPluginHost PluginHost) {
            if (DoItCommand != null) {
                PluginHost.CommandManager.UnRegisterCommand(DoItCommand);
            }
        }
    }
}