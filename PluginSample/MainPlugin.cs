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
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Management.Plugins;
using AdvancedLauncher.SDK.Management.Windows;
using AdvancedLauncher.SDK.Model;
using AdvancedLauncher.SDK.UI;

namespace PluginSample {

    public class MainPlugin : AbstractPlugin {
        private IPluginHost PluginHost;

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

        private IConfiguration Configuration;

        private MenuItem item;

        private PageItem pageItem;

        public override void OnActivate(IPluginHost PluginHost) {
            this.PluginHost = PluginHost;
            /*this.Configuration = new TestConfig(PluginHost.DatabaseManager, PluginHost.LogManager);
            PluginHost.ConfigurationManager.RegisterConfiguration(Configuration);

            item = new MenuItem("DMOTranslator", "appbar_information", new Thickness(9, 4, 9, 4), false);
            item.Click += OnClick;
            PluginHost.WindowManager.AddMenuItem(item);

            ApplicationWindowControl appWindow = new ApplicationWindowControl(new ProcessStartInfo(@"D:\Games\GDMO\DMOTools\DMOTranslator.exe"));
            pageItem = new PageItem("DMOTranslator", appWindow);
            PluginHost.WindowManager.AddPageItem(pageItem);*/
        }

        private void OnClick(object sender, AdvancedLauncher.SDK.Model.Events.BaseEventArgs e) {
            ApplicationWindowControl appWindow = new ApplicationWindowControl(new ProcessStartInfo(@"D:\Games\GDMO\DMOTools\DMOTranslator.exe"));
            WindowContainer WindowContainer = new WindowContainer(appWindow, PluginHost.WindowManager);
            PluginHost.WindowManager.ShowWindow(new WindowContainer(appWindow, PluginHost.WindowManager));
        }

        public override void OnStop(IPluginHost PluginHost) {
            if (Configuration != null) {
                PluginHost.ConfigurationManager.UnRegisterConfiguration(Configuration);
            }
            if (item != null) {
                PluginHost.WindowManager.RemoveMenuItem(item);
            }
            if (pageItem != null) {
                PluginHost.WindowManager.RemovePageItem(pageItem);
            }
        }
    }
}