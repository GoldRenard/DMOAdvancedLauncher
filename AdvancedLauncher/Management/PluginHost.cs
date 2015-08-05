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

using AdvancedLauncher.SDK.Management;
using Ninject;

namespace AdvancedLauncher.Management {

    public class PluginHost : CrossDomainObject, IPluginHost {

        [Inject]
        public ILogManager LogManager {
            get;
            set;
        }

        [Inject]
        public ICommandManager CommandManager {
            get;
            set;
        }

        [Inject]
        public IConfigurationManager ConfigurationManager {
            get;
            set;
        }

        [Inject]
        public IDatabaseManager DatabaseManager {
            get;
            set;
        }

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get;
            set;
        }

        [Inject]
        public IDialogManager DialogManager {
            get;
            set;
        }

        /*
        [Inject]
        public IProfileManager ProfileManager {
            get;
            set;
        }

        [Inject]
        public ITaskManager TaskManager {
            get;
            set;
        }

        [Inject]
        public IUpdateManager UpdateManager {
            get;
            set;
        }

        [Inject]
        public IWindowManager WindowManager {
            get;
            set;
        }*/

        public void Initialize() {
            // nothing to do
        }
    }
}