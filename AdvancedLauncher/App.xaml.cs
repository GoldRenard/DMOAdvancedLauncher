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
using System.Security.Principal;
using System.Windows;
using AdvancedLauncher.Management;
using AdvancedLauncher.Management.Internal;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.Tools.Container;
using AdvancedLauncher.Tools.Execution;
using AdvancedLauncher.UI.Windows;
using log4net.Config;
using Ninject;

namespace AdvancedLauncher {

    public partial class App : Application {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(App));

        public static void PrintHeader() {
            LOGGER.Info("Digimon Masters Online Advanced Launcher, Copyright (C) 2015 Egorov Ilya" + System.Environment.NewLine +
                "This program comes with ABSOLUTELY NO WARRANTY; for details type `license'." + System.Environment.NewLine +
                "This is free software, and you are welcome to redistribute it" + System.Environment.NewLine +
                "under certain conditions; type `license' for details." + System.Environment.NewLine);
        }

        public static IKernel Kernel {
            get;
            private set;
        }

        static App() {
#if RELEASE
            Tools.BugTrapBootstrapper.Inject();
#endif
            Kernel = new StandardKernel(new DependencyModule());
            AppDomain.CurrentDomain.UnhandledException += (s, e) => {
                LOGGER.Error("UnhandledException", e.ExceptionObject as Exception);
            };
        }

        private void Application_Startup(object sender, StartupEventArgs e) {
            XmlConfigurator.Configure();
            PrintHeader();
            Current.DispatcherUnhandledException += (s, ex) => {
                LOGGER.Error("DispatcherUnhandledException", ex.Exception as Exception);
            };
            if (IsAdministrator()) {
                if (!InstanceChecker.AlreadyRunning("27ec7e49-6567-4ee2-9ad6-073705189109")) {
                    // initialization sequence
                    Kernel.Get<IEnvironmentManager>();
                    Kernel.Get<Splashscreen>().Show();
                    Kernel.Get<PluginManager>().Start();
                    (Kernel.Get<IProfileManager>() as ProfileManager).Start();
                    (Kernel.Get<IWindowManager>() as WindowManager).Start();
                } else {
                    Application.Current.Shutdown();
                }
            } else
                MessageBox.Show("Administrator Privileges are required to run DMO AdvancedLauncher. Please run application as Administrator.", "Please run application as Administrator", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static bool IsAdministrator() {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}