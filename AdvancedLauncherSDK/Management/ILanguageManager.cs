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
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Events.Proxy;

namespace AdvancedLauncher.SDK.Management {

    /// <summary>
    /// Language manager API. Provides access to strings and changing event.
    /// </summary>
    /// <seealso cref="LanguageModel"/>
    public interface ILanguageManager : IManager {

        /// <summary>
        /// LanguageChanged event. You should not use this directly, is doesn't work correctly for
        /// cross-domain transparent proxy instances.
        /// Use <see cref="LanguageChangedProxy(EventProxy{BaseEventArgs}, bool)"/> instead.
        /// </summary>
        event BaseEventHandler LanguageChanged;

        /// <summary>
        /// Registers new event listener for language change event.
        /// </summary>
        /// <param name="proxy"><see cref="EventProxy{T}"/> instance</param>
        /// <param name="subscribe"><b>True</b> if you want to subscribe, <b>false</b> otherwise.</param>
        void LanguageChangedProxy(EventProxy<BaseEventArgs> proxy, bool subscribe = true);

        /// <summary>
        /// Language model (stringa accessor)
        /// </summary>
        LanguageModel Model {
            get;
        }
    }
}