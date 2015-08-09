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
using AdvancedLauncher.SDK.UI;

namespace AdvancedLauncher.SDK.Management {

    /// <summary>
    /// Window manager API. Allow add/remove menu items, mainpage tabs, show new windows
    /// </summary>
    public interface IWindowManager : IManager {

        /// <summary>
        /// Shows specifies <see cref="IRemoteControl"/>.
        /// </summary>
        /// <param name="window"><see cref="IRemoteControl"/> to show.</param>
        /// <seealso cref="IRemoteControl"/>
        void ShowWindow(IRemoteControl window);

        /// <summary>
        /// Returns to the home window.
        /// </summary>
        void GoHome();

        /// <summary>
        /// Returns to last opened window.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Returns to last opened window in case that current window is passed as parameter
        /// </summary>
        /// <param name="currentWindow">Desired current window. It does nothing if argument is not current window.</param>
        /// <seealso cref="IRemoteControl"/>
        void GoBack(IRemoteControl currentWindow);

        /// <summary>
        /// Add new menu item.
        /// </summary>
        /// <param name="menuItem">Menu item to add</param>
        /// <seealso cref="MenuItem"/>
        void AddMenuItem(MenuItem menuItem);

        /// <summary>
        /// Removes menu item.
        /// </summary>
        /// <param name="menuItem">Menu item to remove</param>
        /// <seealso cref="MenuItem"/>
        /// <returns><b>True</b> on success.</returns>
        bool RemoveMenuItem(MenuItem menuItem);

        /// <summary>
        /// Add new page item.
        /// </summary>
        /// <param name="pageItem">Page item to add</param>
        /// <seealso cref="PageItem"/>
        void AddPageItem(PageItem pageItem);

        /// <summary>
        /// Removes page item.
        /// </summary>
        /// <param name="pageItem">Page item to remove</param>
        /// <seealso cref="PageItem"/>
        /// <returns><b>True</b> on success.</returns>
        bool RemovePageItem(PageItem pageItem);
    }
}