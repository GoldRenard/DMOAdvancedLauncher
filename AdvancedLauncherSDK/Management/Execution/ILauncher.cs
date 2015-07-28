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

namespace AdvancedLauncher.SDK.Management.Execution {

    public interface ILauncher {

        /// <summary>
        /// Is current launcher supported in the envronment
        /// </summary>
        bool IsSupported {
            get;
        }

        /// <summary>
        /// Name of this launcher
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Name of this launcher
        /// </summary>
        string Mnemonic {
            get;
        }

        /// <summary>
        /// Execute process
        /// </summary>
        /// <param name="application">Path to executable</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        bool Execute(string application);

        /// <summary>
        /// Execute process with arguments
        /// </summary>
        /// <param name="application">Path to executable</param>
        /// <param name="arguments">Arguments</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        bool Execute(string application, string arguments);
    }
}