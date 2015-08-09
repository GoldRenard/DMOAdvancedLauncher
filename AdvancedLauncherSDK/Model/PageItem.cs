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
using AdvancedLauncher.SDK.UI;

namespace AdvancedLauncher.SDK.Model {

    /// <summary>
    /// MainPage item with control
    /// </summary>
    public class PageItem : NamedItem {

        /// <summary>
        /// Initializes a new instance of <see cref="PageItem"/> for specified name, <see cref="IRemoteControl"/> and binding flag (false by default).
        /// </summary>
        /// <param name="Name">Item name</param>
        /// <param name="Content">Content or item</param>
        /// <param name="IsNameBinding">Is it binding name</param>
        public PageItem(string Name, IRemoteControl Content, bool IsNameBinding = false)
            : base(Name, IsNameBinding) {
            this.Content = Content;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PageItem"/> for specified name, <see cref="Control"/> and binding flag (false by default).
        /// </summary>
        /// <param name="Name">Item name</param>
        /// <param name="Content">Content or item</param>
        /// <param name="IsNameBinding">Is it binding name</param>
        public PageItem(string Name, Control Content, bool IsNameBinding = false)
            : base(Name, IsNameBinding) {
            this.Content = new PageContainer(Content);
        }

        /// <summary>
        /// Content of item
        /// </summary>
        public IRemoteControl Content {
            get;
            private set;
        }
    }
}