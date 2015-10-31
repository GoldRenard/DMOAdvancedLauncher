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
using System.Linq;
using AdvancedLauncher.UI.Controls.AutoCompleteBox;
using Ninject;

namespace AdvancedLauncher.Tools {

    public class WikiSuggestionProvider : ISuggestionProvider {

        [Inject]
        public WikiProvider Provider {
            get;
            set;
        }

        public WikiSuggestionProvider() {
            App.Kernel.Inject(this);
        }

        public System.Collections.IEnumerable GetSuggestions(string filter) {
            if (string.IsNullOrEmpty(filter)) {
                return null;
            }
            if (filter.Length < 2) {
                return null;
            }
            try {
                var suggestions = Provider.OpenSearch(filter);
                if (suggestions != null) {
                    return suggestions.Select(x => new WikiProvider.Suggestion() { Value = x }).ToList();
                }
            } catch (Exception) { }
            return null;
        }
    }
}