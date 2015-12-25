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
using System.IO;
using System.Reflection;
using IntelleSoft.BugTrap;

namespace AdvancedLauncher.Tools {

    internal class BugTrapBootstrapper {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(BugTrapBootstrapper));

        static BugTrapBootstrapper() {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.AssemblyResolve += (_, e) => {
                LOGGER.Info(e.Name);
                if (e.Name.StartsWith("BugTrap", StringComparison.OrdinalIgnoreCase)) {
                    return Assembly.LoadFile(Path.Combine(assemblyDir, (IntPtr.Size == 4) ? "BugTrap.dll" : "BugTrap-x64.dll"));
                }
                return null;
            };
        }

        public static void Inject() {
            var currentAsm = Assembly.GetExecutingAssembly();
            AssemblyTitleAttribute title = currentAsm.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute;
            ExceptionHandler.AppName = title.Title;
            ExceptionHandler.AppVersion = currentAsm.GetName().Version.ToString();
            ExceptionHandler.DumpType = MinidumpType.Normal;
            ExceptionHandler.Flags = FlagsType.DetailedMode | FlagsType.EditMail;
            ExceptionHandler.ReportFormat = ReportFormatType.Text;
            ExceptionHandler.SupportEMail = ExceptionHandler.NotificationEMail = "goldrenard@gmail.com";
            ExceptionHandler.SupportHost = "bugtrap.renamon.ru";
            ExceptionHandler.SupportPort = 30700;
            ExceptionHandler.SupportURL = "https://github.com/GoldRenard/DMOAdvancedLauncher/issues";
            ExceptionHandler.BeforeUnhandledException += (s, e) => {
                dynamic args = e;
                LOGGER.Error("BugTrap BeforeUnhandledException", args.ExceptionObject);
            };
        }
    }
}