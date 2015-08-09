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
using AdvancedLauncher.SDK.Management.Commands;

namespace AdvancedLauncher.SDK.Management {

    /// <summary>
    /// Command manager interface. You can register and unregister your commands here.
    /// </summary>'
    /// <seealso cref="ICommand"/>
    /// <seealso cref="AbstractCommand"/>
    /// <seealso cref="AbstractExtendedCommand"/>
    public interface ICommandManager : IManager {

        /// <summary>
        /// Executes the specified command
        /// </summary>
        /// <param name="input">Command line</param>
        /// <returns><b>True</b> on success</returns>
        bool Send(string input);

        /// <summary>
        /// Registers specified command
        /// </summary>
        /// <param name="Command">Your <see cref="ICommand"/> interface.</param>
        /// <seealso cref="ICommand"/>
        void RegisterCommand(ICommand Command);

        /// <summary>
        /// Unregisters specified command
        /// </summary>
        /// <param name="Command">Your <see cref="ICommand"/> interface.</param>
        /// <seealso cref="ICommand"/>
        /// <returns><b>True</b> on success</returns>
        bool UnRegisterCommand(ICommand command);

        /// <summary>
        /// Returns dictionary of registered commands
        /// </summary>
        /// <returns>Dictionary of registered commands</returns>
        IDictionary<string, ICommand> GetCommands();

        /// <summary>
        /// Returns recent executed commands
        /// </summary>
        /// <returns>Recent executed commands</returns>
        List<string> GetRecent();
    }
}