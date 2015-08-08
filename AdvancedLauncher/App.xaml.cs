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

using System.Reflection;
using System.Security.Principal;
using System.Windows;
using AdvancedLauncher.Management.Internal;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.Tools;
using AdvancedLauncher.UI.Windows;
using log4net.Config;
using Ninject;
using static AdvancedLauncher.Tools.ExceptionHandler;

namespace AdvancedLauncher {

    public partial class App : Application {
        private static IKernel _Kernel;

        public static IKernel Kernel {
            get {
                if (_Kernel == null) {
                    _Kernel = new StandardKernel(new DependencyModule());
                }
                return _Kernel;
            }
        }

        public App() {
            if (ExceptionHandler.IsAvailable) {
                var currentAsm = Assembly.GetExecutingAssembly();
                AssemblyTitleAttribute title = currentAsm.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute;
                ExceptionHandler.AppName = title.Title;
                ExceptionHandler.AppVersion = currentAsm.GetName().Version.ToString();
                ExceptionHandler.DumpType = MinidumpType.Normal;
                ExceptionHandler.Flags = FlagsType.DetailedMode | FlagsType.EditMail;
                ExceptionHandler.ReportFormat = ReportFormatType.Text;
                ExceptionHandler.SupportEMail = "goldrenard@gmail.com";
                ExceptionHandler.SupportHost = "bugtrap.renamon.ru";
                ExceptionHandler.SupportPort = 30700;
                ExceptionHandler.SupportURL = "https://github.com/GoldRenard/DMOAdvancedLauncher/issues";
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e) {
            Kernel.Inject(this);
            XmlConfigurator.Configure();
            if (IsAdministrator()) {
                if (!InstanceChecker.AlreadyRunning("27ec7e49-6567-4ee2-9ad6-073705189109")) {
                    // initialization sequence
                    Kernel.Get<IEnvironmentManager>();
                    Kernel.Get<Splashscreen>().Show();
                    Kernel.Get<PluginManager>().Start();
                    Kernel.Get<IProfileManager>().Start();
                    Kernel.Get<IWindowManager>().Start();
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