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

namespace AdvancedLauncher.SDK.Management.Commands {

    /// <summary>
    /// Command interface
    /// </summary>
    /// <seealso cref="ICommandManager"/>
    /// <seealso cref="AbstractCommand"/>
    /// <seealso cref="AbstractExtendedCommand"/>
    public interface ICommand {

        /// <summary>
        /// The command action
        /// </summary>
        /// <param name="args">Input arguments</param>
        /// <returns>Returns <B>true</B> if command successfully executed, <B>false</B> otherwise.</returns>
        bool DoCommand(string[] args);

        /// <summary>
        /// Command description for help
        /// </summary>
        /// <returns>Command description</returns>
        string GetDescription();

        /// <summary>
        /// Command name to execute
        /// </summary>
        /// <returns>Command name</returns>
        string GetName();
    }
}