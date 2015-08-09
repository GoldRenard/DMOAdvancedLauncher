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

    /// <summary>
    /// Extended <see cref="ICommand"/> base implementation with subcommands support
    /// </summary>
    /// <seealso cref="ICommand"/>
    /// <seealso cref="ICommandManager"/>
    /// <seealso cref="AbstractCommand"/>
    public abstract class AbstractExtendedCommand : AbstractCommand {
        private const string HELP_COMMAND_NAME = "help";

        /// <summary>
        /// Subcommand delegate
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns><B>True</B> if command successfully executed, <B>False</B> otherwise.</returns>
        protected delegate bool SubCommand(string[] args);

        private Dictionary<string, SubCommand> _SubCommands;

        /// <summary>
        /// Subcommand dictionary. Key is name, Value is action to execute (<see cref="SubCommand"/>).
        /// </summary>
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

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="commandName">Base command name to execute. See <see cref="ICommand.GetName"/></param>
        /// <param name="commandDescription">Command description. See <see cref="ICommand.GetDescription"/></param>
        public AbstractExtendedCommand(string commandName, string commandDescription) : base(commandName, commandDescription) {
        }

        /// <summary>
        /// Root command action
        /// </summary>
        /// <param name="args">Input arguments</param>
        /// <returns><B>True</B> if command successfully executed, <B>False</B> otherwise.</returns>
        public override bool DoCommand(string[] args) {
            if (!CheckInput(args)) {
                return false;
            }
            SubCommand SubCommand;
            SubCommands.TryGetValue(args[1], out SubCommand);
            return SubCommand(args);
        }

        /// <summary>
        /// Subcommand generation method. You must define your subcommands here.
        /// </summary>
        /// <returns>The subcommand dictionary. Key is name, Value is action to execute (<see cref="SubCommand"/>).</returns>
        protected abstract Dictionary<string, SubCommand> CreateCommands();

        /// <summary>
        /// You must define console output method here (usually <see cref="ILogManager.Info(object)"/>)
        /// </summary>
        /// <param name="value">Value you want to log</param>
        protected abstract void LogInfo(string value);

        /// <summary>
        /// Checks if specified command defined in subcommands list
        /// </summary>
        /// <param name="command">Command name</param>
        /// <returns><B>Yrue</B> if command is defined, <B>false</B> otherwise.</returns>
        protected bool CheckSubCommand(string command) {
            return SubCommands.ContainsKey(command);
        }

        /// <summary>
        /// Returns help list of all available subcommands
        /// </summary>
        /// <returns>Help list of all available subcommands</returns>
        protected List<string> GetCommandList() {
            List<string> output = new List<string>();
            output.Add("Available commands:");
            int i = 1;
            foreach (string command in SubCommands.Keys) {
                output.Add(string.Format(" {0}. {1}", i++, command));
            }
            return output;
        }

        /// <summary>
        /// Input validation
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns><B>True</B> if input is valid, <B>False</B> otherwise.</returns>
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

        /// <summary>
        /// Prints help
        /// </summary>
        /// <param name="args">Arguments (not used)</param>
        /// <returns>Not used. Always <b>True</b>.</returns>
        private bool HelpCommand(string[] args) {
            foreach (string output in GetCommandList()) {
                LogInfo(output);
            }
            return true;
        }
    }
}