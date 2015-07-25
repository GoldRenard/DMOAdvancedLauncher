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
using System.Diagnostics;
using System.IO;

namespace AdvancedLauncher.Service.Execution {

    public abstract class AbstractLauncher : ILauncher {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(AbstractLauncher));

        /// <summary>
        /// Is current launcher supported in the envronment
        /// </summary>
        public abstract bool IsSupported {
            get;
        }

        /// <summary>
        /// Name of this launcher
        /// </summary>
        public abstract string Name {
            get;
        }

        /// <summary>
        /// Mnemonic of this launcher
        /// </summary>
        public string Mnemonic {
            get {
                return GetType().FullName;
            }
        }

        /// <summary>
        /// Execute process
        /// </summary>
        /// <param name="application">Path to executable</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        public bool Execute(string application) {
            return Execute(application, String.Empty);
        }

        /// <summary>
        /// Execute process with arguments
        /// </summary>
        /// <param name="application">Path to executable</param>
        /// <param name="arguments">Arguments</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        public abstract bool Execute(string application, string arguments);

        /// <summary>
        /// Check is executable already working
        /// </summary>
        /// <param name="path">Path to executable</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        protected static bool IsExecutableWorking(string path) {
            string processName = Path.GetFileNameWithoutExtension(path);
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes) {
                if (process.MainModule.FileName.Equals(path)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Execute process
        /// </summary>
        /// <param name="application">Path to executable</param>
        /// <param name="arguments">Arguments</param>
        /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
        public static bool StartProcess(string application, string arguments) {
            try {
                Process proc = new Process();
                proc.StartInfo.FileName = application;
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(application);
                proc.StartInfo.Arguments = arguments;
                proc.Start();
            } catch (Exception e) {
                LOGGER.Debug("Failed to start: [program={0}, commandline={1}]", e);
                return false;
            }
            return true;
        }

        public override bool Equals(object obj) {
            if (!obj.GetType().IsSubclassOf(typeof(AbstractLauncher))) {
                return false;
            }
            AbstractLauncher another = obj as AbstractLauncher;
            if (another.Mnemonic.Equals(Mnemonic)) {
                return true;
            }
            return base.Equals(obj);
        }
    }
}