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

using System.Xml.Serialization;
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Model.Config {

    /// <summary>
    /// News data model
    /// </summary>
    public class NewsData : CrossDomainObject {

        /// <summary>
        /// Gets or sets tab index to show at app start
        /// </summary>
        [XmlAttribute("FirstTab")]
        public byte FirstTab {
            set;
            get;
        }

        /// <summary>
        /// Gets or sets Twitter user timeline source URL
        /// </summary>
        [XmlAttribute("TwitterUrl")]
        public string TwitterUrl {
            set;
            get;
        }

        /// <summary>
        /// Initializes a new <see cref="NewsData"/> instance
        /// </summary>
        public NewsData() {
        }

        /// <summary>
        /// Initializes a new <see cref="NewsData"/> based on another
        /// </summary>
        /// <param name="nd">Source <see cref="NewsData"/></param>
        public NewsData(NewsData nd) {
            this.FirstTab = nd.FirstTab;
            this.TwitterUrl = nd.TwitterUrl;
        }
    }
}