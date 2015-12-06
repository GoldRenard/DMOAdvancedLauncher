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

using System.Diagnostics;
using System.IO;
using AdvancedLauncher.Tools.Execution;

namespace AdvancedLauncher.Management.Execution {

    /// <summary>
    /// Base class for application which aren't compatible with Steam
    /// </summary>
    public abstract class SteamSensitiveLauncher : AbstractLauncher {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(SteamSensitiveLauncher));

        /// <summary>
        /// Execute process with arguments
        /// </summary>
        /// <param name="application">Path to executable</param>
        /// <param name="arguments">Arguments</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        public abstract bool ExecuteInternal(string application, string arguments);

        /// <summary>
        /// Execute process with arguments. Is Steam found as parent process, it will execute program AS IS.
        /// </summary>
        /// <param name="application">Path to executable</param>
        /// <param name="arguments">Arguments</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        public override bool Execute(string application, string arguments) {
            LOGGER.DebugFormat("Trying to start: [application={0}, arguments={1}]", application, arguments);
            bool executed = false;
            if (File.Exists(application)) {
                Process parent = ParentProcessUtilities.GetParentProcess();
                bool isSteam = false;
                if (parent != null) {
                    isSteam = parent.ProcessName.ToLower().Equals("steam");
                }
                if (isSteam) {
                    LOGGER.DebugFormat("Steam found as parent process, run as is.");
                    if (StartProcess(application, arguments)) {
                        executed = true;
                    }
                } else {
                    executed = ExecuteInternal(application, arguments);
                    if (!executed) {
                        executed = StartProcess(application, arguments);
                    }
                }
            }
            return executed;
        }
    }
}