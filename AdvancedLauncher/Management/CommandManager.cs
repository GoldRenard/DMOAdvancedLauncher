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
using System.Linq;
using System.Security.Permissions;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Commands;
using Ninject;

namespace AdvancedLauncher.Management {

    public class CommandManager : CrossDomainObject, ICommandManager {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(CommandManager));
        private const string ENTER_COMMAND = "Please enter the command or \"help\" to view available commands";
        private const string UNKNOWN_COMMAND = "Unknown command \"{0}\"";

        private ConcurrentDictionary<string, ICommand> Commands = new ConcurrentDictionary<string, ICommand>();

        private List<string> recentCommands = new List<string>();

        public void Initialize() {
            foreach (ICommand command in App.Kernel.GetAll<ICommand>()) {
                RegisterCommand(command);
            }
        }

        [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
        public bool Send(string input) {
            if (string.IsNullOrEmpty(input)) {
                LOGGER.Info(ENTER_COMMAND);
                return false;
            }
            LOGGER.Info("] " + input);
            recentCommands.Add(input);
            string[] args = input.Split(' ');
            if (!Commands.ContainsKey(args[0])) {
                LOGGER.Info(String.Format(UNKNOWN_COMMAND, args[0]));
                return false;
            }
            ICommand command;
            Commands.TryGetValue(args[0], out command);
            if (command == null) {
                LOGGER.Info(String.Format(UNKNOWN_COMMAND, args[0]));
                return false;
            }
            return command.DoCommand(args);
        }

        public void RegisterCommand(ICommand command) {
            if (command == null) {
                throw new ArgumentException("command argument cannot be null");
            }
            if (Commands.ContainsKey(command.GetName())) {
                LOGGER.ErrorFormat("Can't register command {0} because command with this name already registered!", command.GetName());
                return;
            }
            Commands.TryAdd(command.GetName(), command);
        }

        public bool UnRegisterCommand(ICommand command) {
            if (command == null) {
                throw new ArgumentException("command argument cannot be null");
            }
            var configToRemove = Commands.FirstOrDefault(kvp => kvp.Value.Equals(command));
            if (configToRemove.Key != null) {
                return Commands.TryRemove(configToRemove.Key, out command);
            }
            return false;
        }

        public IDictionary<string, ICommand> GetCommands() {
            return new Dictionary<string, ICommand>(Commands);
        }

        public List<string> GetRecent() {
            return new List<string>(recentCommands);
        }
    }
}