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

using System.Collections.Generic;
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Model.Web {

    /// <summary>
    /// Base class for <see cref="INewsProvider"/> interface
    /// </summary>
    /// <seealso cref="INewsProvider"/>
    public abstract class AbstractNewsProvider : CrossDomainObject, INewsProvider {

        protected ILogManager LogManager {
            get;
            private set;
        }

        public AbstractNewsProvider() {
        }

        public AbstractNewsProvider(ILogManager logManager) {
            Initialize(logManager);
        }

        /// <summary>
        /// Obtains news from remote source
        /// </summary>
        /// <returns>News collection</returns>
        public abstract List<NewsItem> GetNews();

        public void Initialize(ILogManager logManager) {
            this.LogManager = logManager;
        }
    }
}