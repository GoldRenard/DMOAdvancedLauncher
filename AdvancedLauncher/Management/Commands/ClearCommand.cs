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

using AdvancedLauncher.SDK.Management.Commands;
using AdvancedLauncher.UI.Windows;
using Ninject;

namespace AdvancedLauncher.Management.Commands {

    public class ClearCommand : AbstractCommand {

        [Inject]
        public Logger Logger {
            get;
            set;
        }

        public ClearCommand()
            : base("clear", "Clears the console log") {
        }

        public override bool DoCommand(string[] args) {
            Logger.LogEntries.Clear();
            Logger.LogEntriesFiltered.Clear();
            Logger.PrintHeader();
            return true;
        }
    }
}