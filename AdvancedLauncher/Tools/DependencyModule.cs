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
using AdvancedLauncher.Management.Execution;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Model;
using AdvancedLauncher.UI.Windows;
using Ninject.Modules;

namespace AdvancedLauncher.Tools {

    internal class DependencyModule : NinjectModule {

        public override void Load() {
            // Services
            Bind<IEnvironmentManager>().To<EnvironmentManager>().InSingletonScope();
            Bind<ILanguageManager>().To<LanguageManager>().InSingletonScope();
            Bind<IUpdateManager>().To<UpdateManager>().InSingletonScope();
            Bind<IProfileManager>().To<ProfileManager>().InSingletonScope();
            Bind<ILoginManager>().To<LoginManager>().InSingletonScope();

            // Launchers
            Bind<ILauncherManager>().To<LauncherManager>().InSingletonScope();
            Bind<DirectLauncher>().ToSelf().InSingletonScope();
            Bind<AppLocaleLauncher>().ToSelf().InSingletonScope();
            Bind<NTLeaLauncher>().ToSelf().InSingletonScope();

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
            Bind<Logger>().ToSelf().InSingletonScope();
            Bind<Settings>().ToSelf().InSingletonScope();
        }
    }
}