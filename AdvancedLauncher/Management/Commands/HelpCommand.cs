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
using System.Text;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Commands;
using Ninject;

namespace AdvancedLauncher.Management.Commands {

    public class HelpCommand : AbstractCommand {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(HelpCommand));

        [Inject]
        public ICommandManager CommandManager {
            get; set;
        }

        public HelpCommand()
            : base("help", "Shows help message") {
        }

        public override bool DoCommand(string[] args) {
            IDictionary<string, ICommand> commands = CommandManager.GetCommands();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine("Available commands:");
            foreach (String key in commands.Keys) {
                ICommand command;
                commands.TryGetValue(key, out command);
                builder.AppendLine(String.Format("\t{0} - {1}", key, command.GetDescription()));
            }
            LOGGER.Info(builder);
            return true;
        }
    }
}