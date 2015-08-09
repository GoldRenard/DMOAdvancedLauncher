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

namespace AdvancedLauncher.SDK.Model {

    /// <summary>
    /// News container
    /// </summary>
    /// <seealso cref="Web.INewsProvider"/>
    public class NewsItem : CrossDomainObject {

        /// <summary>
        /// Gets or sets news mode
        /// </summary>
        public string Mode {
            get; set;
        }

        /// <summary>
        /// Gets or sets news subject
        /// </summary>
        public string Subject {
            get; set;
        }

        /// <summary>
        /// Gets or sets news publish date
        /// </summary>
        public string Date {
            get; set;
        }

        /// <summary>
        /// Gets or sets news content
        /// </summary>
        public string Content {
            get; set;
        }

        /// <summary>
        /// Gets or sets news default URL
        /// </summary>
        public string Url {
            get; set;
        }

        /// <summary>
        /// Initializes a new <see cref="NewsItem"/> instance
        /// </summary>
        public NewsItem() {
        }

        /// <summary>
        /// Initializes a new <see cref="NewsItem"/> based on another
        /// </summary>
        /// <param name="item">Source <see cref="NewsItem"/></param>
        public NewsItem(NewsItem item) {
            this.Mode = item.Mode;
            this.Subject = item.Subject;
            this.Date = item.Date;
            this.Content = item.Content;
            this.Url = item.Url;
        }
    }
}