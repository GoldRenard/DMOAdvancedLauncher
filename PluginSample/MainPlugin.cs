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

using System.Diagnostics;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Commands;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Management.Plugins;
using AdvancedLauncher.SDK.Management.Windows;
using AdvancedLauncher.SDK.Model.Events.Proxy;
using AdvancedLauncher.SDK.Tools;

namespace PluginSample {

    public class MainPlugin : AbstractPlugin {
        private IPluginHost PluginHost;

        public class TestCommand : AbstractCommand {
            private readonly IPluginHost PluginHost;

            public TestCommand(IPluginHost PluginHost)
                : base("doit", "Just the test command") {
                this.PluginHost = PluginHost;
            }

            public override bool DoCommand(string[] args) {
                PluginHost.LogManager.Info("SimplePlugin - Did it!");
                /*ProcessStartInfo pInfo = new ProcessStartInfo(@"D:\Games\GDMO\DMOTools\DMOTranslator.exe");
                ApplicationWindowControl appWindow = new ApplicationWindowControl(pInfo, PluginHost.LanguageManager, PluginHost.WindowManager);
                PluginHost.WindowManager.ShowWindow(appWindow.Container);*/
                return true;
            }
        }

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

        private ICommand DoItCommand;

        private IConfiguration Configuration;

        private BaseEventProxy ProfileChangedProxy;

        public override void OnActivate(IPluginHost PluginHost) {
            this.PluginHost = PluginHost;
            this.ProfileChangedProxy = new BaseEventProxy(OnProfileChanged);
            DoItCommand = new TestCommand(PluginHost);
            Configuration = new TestConfig(PluginHost.DatabaseManager, PluginHost.LogManager);
            PluginHost.CommandManager.RegisterCommand(DoItCommand);
            PluginHost.ConfigurationManager.RegisterConfiguration(Configuration);

            PluginHost.ProfileManager.ProfileChangedProxy(ProfileChangedProxy);
        }

        public override void OnStop(IPluginHost PluginHost) {
            if (DoItCommand != null) {
                PluginHost.CommandManager.UnRegisterCommand(DoItCommand);
            }
            if (Configuration != null) {
                PluginHost.ConfigurationManager.UnRegisterConfiguration(Configuration);
            }
            PluginHost.ProfileManager.ProfileChangedProxy(ProfileChangedProxy, false);
        }

        public void OnProfileChanged(object sender, AdvancedLauncher.SDK.Model.Events.BaseEventArgs e) {
            PluginHost.LogManager.Info("ProfileChanged!");
        }
    }
}