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
using System.Diagnostics;
using System.Text;
using AdvancedLauncher.SDK.Management.Commands;

namespace AdvancedLauncher.Management.Commands {

    public class ExecCommand : AbstractCommand {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(ExecCommand));

        public ExecCommand()
            : base("exec", "Execute program") {
        }

        public override bool DoCommand(string[] args) {
            if (args.Length < 2) {
                LOGGER.InfoFormat("Usage: {0} <executable> [arguments]", args[0]);
                return false;
            }
            List<String> argList = new List<string>(args);
            string executable = argList[1];
            argList.RemoveRange(0, 2);

            try {
                if (argList.Count > 0) {
                    Process.Start(executable, ParseArguments(argList));
                } else {
                    Process.Start(executable);
                }
            } catch (Exception ex) {
                LOGGER.Error(ex);
            }
            return true;
        }

        private string ParseArguments(List<String> args) {
            StringBuilder builder = new StringBuilder();

            foreach (String arg in args) {
                builder.Append(arg + " ");
            }
            return builder.ToString().Trim();
        }
    }
}