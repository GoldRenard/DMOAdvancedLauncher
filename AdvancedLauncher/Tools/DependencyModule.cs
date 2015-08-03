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

using AdvancedLauncher.Management;
using AdvancedLauncher.Management.Commands;
using AdvancedLauncher.Model;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Commands;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Management.Execution;
using AdvancedLauncher.UI.Windows;
using Ninject;
using Ninject.Activation.Strategies;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace AdvancedLauncher.Tools {

    internal class DependencyModule : NinjectModule {

        public override void Load() {
            Kernel.Components.Add<IActivationStrategy, ActivationStrategy>();
            // Singletone services
            Bind<IEnvironmentManager>().To<EnvironmentManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<IWindowManager>().To<WindowManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<IProfileManager>().To<ProfileManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<IDialogManager>().To<DialogManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<ILanguageManager>().To<LanguageManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<ILogManager>().To<LogManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<ITaskManager>().To<TaskManager>().InSingletonScope().OnActivation(m => m.Initialize());

            // Multiinstance services
            Bind<IUpdateManager>().To<UpdateManager>().OnActivation(m => m.Initialize());
            Bind<IFileSystemManager>().To<FileSystemManager>().OnActivation(m => m.Initialize(App.Kernel.Get<ILogManager>()));

            // Launchers
            Bind<ILauncherManager>().To<LauncherManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Kernel.Bind(e => {
                e.FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<ILauncher>()
                .BindAllInterfaces()
                .Configure(c => c.InSingletonScope());
            });

            // Commands
            Bind<ICommandManager>().To<CommandManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Kernel.Bind(e => {
                e.FromThisAssembly()
                .SelectAllClasses()
                .InNamespaceOf<EchoCommand>()
                .InheritedFrom<ICommand>()
                .BindAllInterfaces()
                .Configure(c => c.InSingletonScope());
            });

            // Game Configurations
            Bind<IConfigurationManager>().To<ConfigurationManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Kernel.Bind(e => {
                e.FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<IConfiguration>()
                .BindAllInterfaces()
                .Configure(c => c.InSingletonScope());
            });

            // ViewModels
            Bind<DigimonItemViewModel>().ToSelf();
            Bind<GuildInfoItemViewModel>().ToSelf();
            Bind<JoymaxItemViewModel>().ToSelf();
            Bind<TamerItemViewModel>().ToSelf();
            Bind<TwitterItemViewModel>().ToSelf();

            // Components
            Bind<MainWindow>().ToSelf().InSingletonScope(); // be careful with injecting this on initialization of MainWindow itself (UserControls, etc)
            Bind<Logger>().ToSelf().InSingletonScope();
            Bind<About>().ToSelf().InSingletonScope();
            Bind<Settings>().ToSelf().InSingletonScope();
            Bind<Splashscreen>().ToSelf().InSingletonScope();

            Bind<LoginManager>().ToSelf().InSingletonScope();
            Bind<ProxyManager>().ToSelf().InSingletonScope();
            Bind<IconHolder>().ToSelf().InSingletonScope();

            // Plugin System
            Bind<PluginManager>().To<PluginManager>().InSingletonScope();
            Bind<IPluginHost>().To<PluginHost>().InSingletonScope().OnActivation(m => m.Initialize());
        }
    }
}