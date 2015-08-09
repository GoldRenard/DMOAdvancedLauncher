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

namespace AdvancedLauncher.SDK.Management {

    /// <summary>
    /// API accessor for plugins
    /// </summary>
    /// <seealso cref="Plugins.IPlugin"/>
    public interface IPluginHost : IManager {

        /// <summary>
        /// Gets <see cref="ILogManager"/> API.
        /// </summary>
        /// <seealso cref="ILogManager"/>
        ILogManager LogManager {
            get;
        }

        /// <summary>
        /// Gets <see cref="ICommandManager"/> API.
        /// </summary>
        /// <seealso cref="ICommandManager"/>
        ICommandManager CommandManager {
            get;
        }

        /// <summary>
        /// Gets <see cref="IConfigurationManager"/> API.
        /// </summary>
        /// <seealso cref="IConfigurationManager"/>
        IConfigurationManager ConfigurationManager {
            get;
        }

        /// <summary>
        /// Gets <see cref="IDatabaseManager"/> API.
        /// </summary>
        /// <seealso cref="IDatabaseManager"/>
        IDatabaseManager DatabaseManager {
            get;
        }

        /// <summary>
        /// Gets <see cref="IEnvironmentManager"/> API.
        /// </summary>
        /// <seealso cref="IEnvironmentManager"/>
        IEnvironmentManager EnvironmentManager {
            get;
        }

        /// <summary>
        /// Gets <see cref="IDialogManager"/> API.
        /// </summary>
        /// <seealso cref="IDialogManager"/>
        IDialogManager DialogManager {
            get;
        }

        /// <summary>
        /// Gets <see cref="IProfileManager"/> API.
        /// </summary>
        /// <seealso cref="IProfileManager"/>
        IProfileManager ProfileManager {
            get;
        }

        /// <summary>
        /// Gets <see cref="ITaskManager"/> API.
        /// </summary>
        /// <seealso cref="ITaskManager"/>
        ITaskManager TaskManager {
            get;
        }

        /// <summary>
        /// Gets <see cref="IWindowManager"/> API.
        /// </summary>
        /// <seealso cref="IWindowManager"/>
        IWindowManager WindowManager {
            get;
        }

        /// <summary>
        /// Gets <see cref="ILanguageManager"/> API.
        /// </summary>
        /// <seealso cref="ILanguageManager"/>
        ILanguageManager LanguageManager {
            get;
        }
    }
}