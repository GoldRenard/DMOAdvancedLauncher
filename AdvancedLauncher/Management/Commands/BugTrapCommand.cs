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
using AdvancedLauncher.SDK.Management.Commands;

namespace AdvancedLauncher.Management.Commands {

    public class BugTrapCommand : AbstractCommand {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(BugTrapCommand));

        public BugTrapCommand()
            : base("bugtrap", "Throws an unhandled exception to BugTrap") {
        }

        public override bool DoCommand(string[] args) {
            throw new Exception("NO NOT REPORT THIS \"BUG\"");
        }
    }
}