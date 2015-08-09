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

namespace AdvancedLauncher.SDK.UI {

    /// <summary>
    /// Base <see cref="AbstractWindowControl"/> implementation for <see cref="IWindowManager"/>'s windows.
    /// </summary>
    public abstract class AbstractWindowControl : AbstractUserControl {

        public AbstractWindowControl(ILanguageManager LanguageManager, IWindowManager WindowManager)
            : base(LanguageManager, WindowManager) {
            this.Container = new WindowContainer(this, WindowManager);
        }

        protected void OnCloseClick(object sender, System.Windows.RoutedEventArgs e) {
            Close();
        }

        /// <summary>
        /// Window show handler
        /// </summary>
        public virtual void OnShow() {
            // nothing to do here
        }

        /// <summary>
        /// Returns to last opened window. You're free to override this method.
        /// </summary>
        public virtual void Close() {
            WindowManager.GoBack(Container);
        }
    }
}