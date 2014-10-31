// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Globalization;
using System.IO;
using AdvancedLauncher.Environment;

namespace AdvancedLauncher.Service {

    /// <summary>
    /// Application running helper
    /// </summary>
    public static class ApplicationLauncher {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(ApplicationLauncher));

        /// <summary>
        /// Execute process with AppLocale if it exists in system and allowed in SettingsProvider or execute it directly
        /// </summary>
        /// <param name="program">Path to program</param>
        /// <param name="UseAL">Should use AppLocale</param>
        /// <param name="useKBLC">Should use Keyboard Layout change service</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        public static bool Execute(string program, bool UseAL, bool useKBLC) {
            return Execute(program, string.Empty, UseAL, useKBLC);
        }

        /// <summary>
        /// Execute process with AppLocale if it exists in system and allowed in SettingsProvider or execute it directly
        /// </summary>
        /// <param name="program">Path to program</param>
        /// <param name="args">Arguments</param>
        /// <param name="UseAL">Should use AppLocale</param>
        /// <param name="useKBLC">Should use Keyboard Layout change service</param>
        /// <returns><see langword="true"/> if it succeeds, <see langword="false"/> if it fails.</returns>
        public static bool Execute(string program, string args, bool useAL, bool useKBLC) {
            LOGGER.DebugFormat("Trying to start: [program={0}, args={1}, useAL={2}, useKBLC={3}",
                program, args, useAL, useKBLC);
            bool executed = false;
            if (File.Exists(program)) {
                Process parent = ParentProcessUtilities.GetParentProcess();
                bool isSteam = false;
                if (parent != null) {
                    isSteam = parent.ProcessName.ToLower().Equals("steam");
                }
                if (isSteam) {
                    LOGGER.DebugFormat("Steam found as parent process. Force disable AppLocale.");
                }
                if (useAL && !isSteam) {
                    if (!ExecuteAppLocale(program, args)) {
                        if (StartProcess(program, args)) {
                            executed = true;
                        }
                    } else {
                        executed = true;
                    }
                } else {
                    if (StartProcess(program, args)) {
                        executed = true;
                    }
                }
            }
            if (executed && useKBLC && !IsExecutableWorking(LauncherEnv.GetKBLCFile())) {
                StartProcess(LauncherEnv.GetKBLCFile(), "-attach -notray");
            }
            return executed;
        }

        /// <summary>
        /// Execute process
        /// </summary>
        /// <param name="program">Path to program</param>
        /// <param name="commandline">Command line</param>
        /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
        public static bool StartProcess(string program, string commandline) {
            try {
                Process proc = new Process();
                proc.StartInfo.FileName = program;
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(program);
                proc.StartInfo.Arguments = commandline;
                proc.Start();
            } catch (Exception e) {
                LOGGER.Debug("Failed to start: [program={0}, commandline={1}]", e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Execute process with AppLocale if it exists in system and allowed in SettingsProvider
        /// </summary>
        /// <param name="program">Path to program</param>
        /// <param name="args">Command line</param>
        /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
        private static bool ExecuteAppLocale(string program, string args) {
            string appLocalePath = System.Environment.GetEnvironmentVariable("windir") + "\\apppatch\\AppLoc.exe";

            if (!IsALSupported) {
                return false;
            }

            if (!string.IsNullOrEmpty(args)) {
                StartProcess(appLocalePath, "\"" + program + "\" \"" + args + "\" \"/L0412\"");
            } else {
                StartProcess(appLocalePath, "\"" + program + "\" \"/L0412\"");
            }
            return true;
        }

        private static bool IsExecutableWorking(string path) {
            string processName = Path.GetFileNameWithoutExtension(path);
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes) {
                if (process.MainModule.FileName.Equals(path)) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsALInstalled {
            set;
            get;
        }

        public static bool IsKoreanSupported {
            set;
            get;
        }

        public static bool IsALSupported {
            get {
                string appLocalePath = System.Environment.GetEnvironmentVariable("windir") + "\\apppatch\\AppLoc.exe";
                IsALInstalled = File.Exists(appLocalePath);

                IsKoreanSupported = false;
                foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures))
                    if (ci.TwoLetterISOLanguageName == "ko") {
                        IsKoreanSupported = true;
                        break;
                    }
                return IsALInstalled && IsKoreanSupported;
            }
        }
    }
}