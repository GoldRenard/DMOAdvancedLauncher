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
using System.Text;

namespace AdvancedLauncher.Environment.Commands {

    public class EchoCommand : Command {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(EchoCommand));

        public EchoCommand()
            : base("echo", "Echo to console") {
        }

        public override void DoCommand(string[] args) {
            StringBuilder builder = new StringBuilder();
            bool skipEcho = false;
            foreach (String arg in args) {
                if (!skipEcho) {
                    skipEcho = true;
                    continue;
                }
                builder.Append(String.Format("{0} ", arg));
            }
            LOGGER.Info(builder);
        }
    }
}