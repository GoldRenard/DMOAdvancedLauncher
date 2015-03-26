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

using System.Security.Principal;
using System.Windows;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Service;
using AdvancedLauncher.Windows;
using log4net.Config;

#if DEBUG

using DMOLibrary.Database.Context;

#endif

namespace AdvancedLauncher {

    public partial class App : Application {

        public static bool IsAdministrator() {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public App() {
            if (IsAdministrator()) {
                LauncherEnv.Load();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e) {
            XmlConfigurator.Configure();
            if (IsAdministrator()) {
                if (!InstanceChecker.AlreadyRunning("27ec7e49-6567-4ee2-9ad6-073705189109")) {
                    LauncherEnv.LoadTheme();
                    Splashscreen.ShowSplash();
#if DEBUG
                    Splashscreen.SetProgress("Initializing database...");
                    using (MainContext context = new MainContext()) {
                        context.Database.Initialize(false);
                    }
#endif
                    MainWindow mw = new MainWindow();
                    mw.Show();
                } else {
                    Application.Current.Shutdown();
                }
            } else
                MessageBox.Show("Administrator Privileges are required to run DMO AdvancedLauncher. Please run application as Administrator.", "Please run application as Administrator", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            BugWindow bw = new BugWindow(sender, e);
            bw.ShowDialog();
            e.Handled = true;
            Application.Current.Shutdown();
        }
    }
}