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

using AdvancedLauncher.SDK.Model;

namespace AdvancedLauncher.SDK.Management {

    /// <summary>
    /// Task manager API. It locks application closing until all tasks complete.
    /// </summary>
    /// <seealso cref="TaskEntry"/>
    public interface ITaskManager : IManager {

        /// <summary>
        /// Aquires new lock for specified <see cref="TaskEntry"/>
        /// </summary>
        /// <param name="entry"><see cref="TaskEntry"/> to lock</param>
        void AquireLock(TaskEntry entry);

        /// <summary>
        /// Releases specified <see cref="TaskEntry"/>
        /// </summary>
        /// <param name="entry"><see cref="TaskEntry"/> to release</param>
        /// <returns><b>True</b> on success</returns>
        bool ReleaseLock(TaskEntry entry);

        /// <summary>
        /// Gets <b>True</b> if any lock exists
        /// </summary>
        bool IsBusy {
            get;
        }

        /// <summary>
        /// Schedules application closing. It waits until all locks releases and then closes the application.
        /// </summary>
        /// <param name="forceClose">If <b>True</b>, it will close app directly event with locks.</param>
        void CloseApp(bool forceClose = false);
    }
}