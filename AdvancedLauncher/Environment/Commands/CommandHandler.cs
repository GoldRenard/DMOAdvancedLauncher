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

namespace AdvancedLauncher.Environment.Commands {

    public static class CommandHandler {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(CommandHandler));
        private static Dictionary<String, Command> commands = new Dictionary<String, Command>();

        private const string ENTER_COMMAND = "Please enter the command or \"help\" to view available commands";
        private const string UNKNOWN_COMMAND = "Unknown command \"{0}\"";

        private static List<String> recentCommands = new List<string>();

        static CommandHandler() {
            RegisterCommand(new HelpCommand());
            RegisterCommand(new EchoCommand());
            RegisterCommand(new ExecCommand());
            RegisterCommand(new ExitCommand());
            RegisterCommand(new LicenceCommand());
        }

        public static void Send(string input) {
            if (string.IsNullOrEmpty(input)) {
                LOGGER.Info(ENTER_COMMAND);
                return;
            }
            LOGGER.Info("] " + input);
            recentCommands.Add(input);
            string[] args = input.Split(' ');
            if (!commands.ContainsKey(args[0])) {
                LOGGER.Info(String.Format(UNKNOWN_COMMAND, args[0]));
                return;
            }
            Command command;
            commands.TryGetValue(args[0], out command);
            if (command == null) {
                LOGGER.Info(String.Format(UNKNOWN_COMMAND, args[0]));
                return;
            }
            command.DoCommand(args);
        }

        public static void RegisterCommand(Command command) {
            if (commands.ContainsKey(command.GetName())) {
                LOGGER.ErrorFormat("Can't register command {0} because command with this name already registered!", command.GetName());
                return;
            }
            commands.Add(command.GetName(), command);
        }

        public static void UnRegisterCommand(string name) {
            commands.Remove(name);
        }

        public static void UnRegisterCommand(Command command) {
            UnRegisterCommand(command.GetName());
        }

        public static Dictionary<String, Command> GetCommands() {
            return commands;
        }

        public static List<String> GetRecent() {
            return recentCommands;
        }
    }
}