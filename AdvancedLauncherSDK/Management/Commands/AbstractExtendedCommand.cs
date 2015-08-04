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

namespace AdvancedLauncher.SDK.Management.Commands {

    public abstract class AbstractExtendedCommand : AbstractCommand {
        private const string HELP_COMMAND_NAME = "help";

        protected delegate bool SubCommand(string[] args);

        private Dictionary<string, SubCommand> _SubCommands;

        protected Dictionary<string, SubCommand> SubCommands {
            get {
                if (_SubCommands == null) {
                    _SubCommands = CreateCommands();
                    _SubCommands.Add(HELP_COMMAND_NAME, HelpCommand);
                }
                return _SubCommands;
            }
            private set {
                _SubCommands = value;
            }
        }

        public AbstractExtendedCommand(string commandName, string commandDescription) : base(commandName, commandDescription) {
        }

        public override bool DoCommand(string[] args) {
            if (!CheckInput(args)) {
                return false;
            }
            SubCommand SubCommand;
            SubCommands.TryGetValue(args[1], out SubCommand);
            return SubCommand(args);
        }

        protected abstract Dictionary<string, SubCommand> CreateCommands();

        protected abstract void LogInfo(string value);

        protected bool CheckSubCommand(string command) {
            return SubCommands.ContainsKey(command);
        }

        protected List<string> GetCommandList() {
            List<string> output = new List<string>();
            output.Add("Available commands:");
            int i = 1;
            foreach (string command in SubCommands.Keys) {
                output.Add(string.Format(" {0}. {1}", i++, command));
            }
            return output;
        }

        private bool CheckInput(string[] args) {
            bool valid = true;
            if (args == null) {
                valid = false;
            }
            if (args.Length < 2) {
                valid = false;
            }
            if (!valid) {
                HelpCommand(args);
                return valid;
            }
            if (!SubCommands.ContainsKey(args[1])) {
                valid = false;
                LogInfo(string.Format("No such subcommand for \"{0}\". Enter {1} for list of subcommands.", GetName(), HELP_COMMAND_NAME));
            }
            return valid;
        }

        private bool HelpCommand(string[] args) {
            foreach (string output in GetCommandList()) {
                LogInfo(output);
            }
            return true;
        }
    }
}