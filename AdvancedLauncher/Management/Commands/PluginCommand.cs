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

using System.Collections.Generic;
using System.Linq;
using AdvancedLauncher.Model;
using AdvancedLauncher.SDK.Management.Commands;
using Ninject;

namespace AdvancedLauncher.Management.Commands {

    public class PluginCommand : AbstractExtendedCommand {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(PluginCommand));

        public PluginCommand()
            : base("plugin", "Plugin commands. Enter \"plugin help\" for more info") {
        }

        private PluginManager _PluginManager;

        private PluginManager PluginManager {
            get {
                // we should not inject it here
                if (_PluginManager == null) {
                    _PluginManager = App.Kernel.Get<PluginManager>();
                }
                return _PluginManager;
            }
        }

        protected override Dictionary<string, SubCommand> CreateCommands() {
            Dictionary<string, SubCommand> commands = new Dictionary<string, SubCommand>();
            commands.Add("list", PluginList);
            commands.Add("start", PluginStart);
            commands.Add("stop", PluginStop);
            return commands;
        }

        protected override void LogInfo(string value) {
            LOGGER.Info(value);
        }

        private bool PluginList(string[] args) {
            List<PluginContainer> containers = PluginManager.GetPlugins();
            if (containers.Count == 0) {
                LogInfo("No plugins yet");
                return true;
            }

            List<string> columnNames = new List<string>();
            List<string> numColumn = new List<string>();
            List<string> nameColumn = new List<string>();
            List<string> authorColumn = new List<string>();
            List<string> statusColumn = new List<string>();

            columnNames.Add("No");
            columnNames.Add("Name");
            columnNames.Add("Author");
            columnNames.Add("Status");

            int i = 1;
            foreach (PluginContainer command in containers) {
                numColumn.Add(i++.ToString());
                nameColumn.Add(command.Name);
                authorColumn.Add(command.Author);
                statusColumn.Add(command.Status.ToString());
            }

            LogInfo("Available plugins:");
            LogInfo(" ");
            foreach (string line in PrintTable(columnNames, numColumn, nameColumn, authorColumn, statusColumn)) {
                LogInfo(line);
            }
            return true;
        }

        private bool PluginStart(string[] args) {
            if (args.Length < 3) {
                LogInfo("Usage: plugin start [name or number].");
                return false;
            }
            PluginContainer container = PickContainer(args);
            if (container == null) {
                LogInfo("No such plugin");
                return false;
            }
            return PluginManager.StartPlugin(container);
        }

        private bool PluginStop(string[] args) {
            if (args.Length < 3) {
                LogInfo("Usage: plugin stop [name or number].");
                return false;
            }
            PluginContainer container = PickContainer(args);
            if (container == null) {
                LogInfo("No such plugin");
                return false;
            }
            return PluginManager.StopPlugin(container);
        }

        private PluginContainer PickContainer(string[] args) {
            List<PluginContainer> containers = PluginManager.GetPlugins();
            int number;
            if (int.TryParse(args[2], out number)) {
                if (number < 0 || number > containers.Count) {
                    return null;
                }
                return containers[number - 1];
            }
            return containers.FirstOrDefault(c => c.Name == args[2]);
        }
    }
}