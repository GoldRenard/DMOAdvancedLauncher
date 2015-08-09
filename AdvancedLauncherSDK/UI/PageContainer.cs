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

using System.Windows.Controls;

namespace AdvancedLauncher.SDK.UI {

    /// <summary>
    /// <see cref="ControlContainer"/> implementation for <see cref="IWindowManager"/>'s pages.
    /// </summary>
    /// <seealso cref="IRemoteControl"/>
    /// <seealso cref="WindowContainer"/>
    /// <seealso cref="ControlContainer"/>
    public class PageContainer : ControlContainer {

        public PageContainer(Control Control) : base(Control) {
        }

        /// <summary>
        /// Page show handler
        /// </summary>
        public override void OnShow() {
            AbstractPageControl pageControl = this.Control as AbstractPageControl;
            if (pageControl != null) {
                pageControl.OnShowInternal();
            }
        }

        /// <summary>
        /// Page close handler
        /// </summary>
        public override void OnClose() {
            AbstractPageControl pageControl = this.Control as AbstractPageControl;
            if (pageControl != null) {
                pageControl.OnCloseInternal();
            }
        }
    }
}