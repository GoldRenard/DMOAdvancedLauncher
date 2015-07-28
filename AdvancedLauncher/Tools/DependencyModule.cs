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
using AdvancedLauncher.Management.Configuration;
using AdvancedLauncher.Management.Execution;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Model;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Commands;
using AdvancedLauncher.SDK.Management.Execution;
using AdvancedLauncher.UI.Windows;
using Ninject;
using Ninject.Modules;

namespace AdvancedLauncher.Tools {

    internal class DependencyModule : NinjectModule {

        public override void Load() {
            // Services
            Bind<IEnvironmentManager>().To<EnvironmentManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<ILanguageManager>().To<LanguageManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<IUpdateManager>().To<UpdateManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<IProfileManager>().To<ProfileManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<ILoginManager>().To<LoginManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<ITaskManager>().To<TaskManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<IGameUpdateManager>().To<GameUpdateManager>().OnActivation(m => m.Initialize()); // not singletone!

            // Launchers
            Bind<ILauncherManager>().To<LauncherManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<ILauncher>().To<DirectLauncher>().InSingletonScope();
            Bind<ILauncher>().To<AppLocaleLauncher>().InSingletonScope();
            Bind<ILauncher>().To<NTLeaLauncher>().InSingletonScope();

            // Commands
            Bind<ICommandManager>().To<CommandManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<ICommand>().To<EchoCommand>().InSingletonScope();
            Bind<ICommand>().To<ExecCommand>().InSingletonScope();
            Bind<ICommand>().To<ExitCommand>().InSingletonScope();
            Bind<ICommand>().To<HelpCommand>().InSingletonScope();
            Bind<ICommand>().To<LicenseCommand>().InSingletonScope();

            // Game Configurations
            Bind<IConfigurationManager>().To<ConfigurationManager>().InSingletonScope().OnActivation(m => m.Initialize());
            Bind<IGameConfiguration>().To<JoymaxConfiguration>().InSingletonScope();
            Bind<IGameConfiguration>().To<AeriaConfiguration>().InSingletonScope();
            Bind<IGameConfiguration>().To<KDMODMConfiguration>().InSingletonScope();
            Bind<IGameConfiguration>().To<KDMOIMBCConfiguration>().InSingletonScope();

            // ViewModels
            Bind<DigimonItemViewModel>().ToSelf();
            Bind<GuildInfoItemViewModel>().ToSelf();
            Bind<JoymaxItemViewModel>().ToSelf();
            Bind<TamerItemViewModel>().ToSelf();
            Bind<TwitterItemViewModel>().ToSelf();

            // Components
            Bind<IconHolder>().ToSelf().InSingletonScope();
            Bind<MainWindow>().ToSelf().InSingletonScope();
            Bind<About>().ToSelf().InSingletonScope();
            Bind<Settings>().ToSelf().InSingletonScope();
            Bind<Logger>().ToSelf().InSingletonScope().OnActivation(e => e.Initialize());
        }

        public override void VerifyRequiredModulesAreLoaded() {
            Kernel.Get<IEnvironmentManager>();
        }
    }
}