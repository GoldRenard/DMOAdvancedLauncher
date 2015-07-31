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

using System.Collections.ObjectModel;
using AdvancedLauncher.SDK.Management.Windows;

namespace AdvancedLauncher.SDK.Management {

    public interface IWindowManager : IManager {

        ObservableCollection<MenuItem> MenuItems {
            get;
        }

        void ShowWindow(IWindow window);

        void GoHome();

        /// <summary>
        /// Returns to last opened window.
        /// </summary>
        /// <param name="currentWindow"></param>
        void GoBack();

        /// <summary>
        /// Returns to last opened window in case that current window is passed as parameter
        /// </summary>
        /// <param name="currentWindow">Desired current window. It does nothing if argument is not current window.</param>
        void GoBack(IWindow currentWindow);

        void AddMenuItem(MenuItem menuItem);

        bool RemoveMenuItem(MenuItem menuItem);

        T FindResource<T>(string name);
    }
}