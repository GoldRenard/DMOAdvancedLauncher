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
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Management.Plugins;
using AdvancedLauncher.SDK.Model;

namespace PluginSample {

    public class MainPlugin : AbstractPlugin {

        public override string Author {
            get {
                return "GoldRenard";
            }
        }

        public override string Name {
            get {
                return "SimplePlugin";
            }
        }

        public class TestCommand : AbstractCommand {
            private readonly IPluginHost PluginHost;

            public TestCommand(IPluginHost PluginHost) : base("doit", "Let the launcher do it!") {
                this.PluginHost = PluginHost;
            }

            public override bool DoCommand(string[] args) {
                string username = PluginHost.ProfileManager.CurrentProfile.Name;
                PluginHost.LogManager.InfoFormat("Did it, {0}!", username);
                return true;
            }
        }

        private IPluginHost PluginHost;

        private ICommand Command;

        private IConfiguration Configuration;

        private MenuItem menuItem;

        public override void OnActivate(IPluginHost PluginHost) {
            this.PluginHost = PluginHost;
            this.Command = new TestCommand(PluginHost);
            PluginHost.CommandManager.RegisterCommand(Command);
            this.Configuration = new TestConfig(PluginHost.DatabaseManager, PluginHost.LogManager);
            PluginHost.ConfigurationManager.RegisterConfiguration(Configuration);

            menuItem = new MenuItem("Do it!");
            menuItem.Click += OnClick;
            PluginHost.WindowManager.AddMenuItem(menuItem);
        }

        private void OnClick(object sender, AdvancedLauncher.SDK.Model.Events.BaseEventArgs e) {
            PluginHost.DialogManager.ShowMessageDialog("Did it!", "Yeah, I DID IT!");
        }

        public override void OnStop(IPluginHost PluginHost) {
            if (Command != null) {
                PluginHost.CommandManager.UnRegisterCommand(Command);
            }
            if (Configuration != null) {
                PluginHost.ConfigurationManager.UnRegisterConfiguration(Configuration);
            }
            if (menuItem != null) {
                PluginHost.WindowManager.RemoveMenuItem(menuItem);
            }
        }
    }
}