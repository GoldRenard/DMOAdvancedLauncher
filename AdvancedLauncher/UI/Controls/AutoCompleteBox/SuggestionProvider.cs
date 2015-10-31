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

using System;
using System.Collections;

namespace AdvancedLauncher.UI.Controls.AutoCompleteBox {

    public class SuggestionProvider : ISuggestionProvider {

        #region Private Fields

        private Func<string, IEnumerable> _method;

        #endregion Private Fields

        #region Public Constructors

        public SuggestionProvider(Func<string, IEnumerable> method) {
            if (method == null) {
                throw new ArgumentNullException("method");
            }
            _method = method;
        }

        #endregion Public Constructors

        #region Public Methods

        public System.Collections.IEnumerable GetSuggestions(string filter) {
            return _method(filter);
        }

        #endregion Public Methods
    }
}