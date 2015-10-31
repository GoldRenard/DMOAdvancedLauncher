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
using System.IO;
using AdvancedLauncher.UI.Controls.AutoCompleteBox;

namespace AdvancedLauncher.Tools {

    public class FilesystemSuggestionProvider : ISuggestionProvider {

        public System.Collections.IEnumerable GetSuggestions(string filter) {
            if (string.IsNullOrEmpty(filter)) {
                return null;
            }
            if (filter.Length < 3) {
                return null;
            }

            if (filter[1] != ':') {
                return null;
            }

            List<System.IO.FileSystemInfo> lst = new List<System.IO.FileSystemInfo>();
            string dirFilter = "*";
            string dirPath = filter;
            if (!filter.EndsWith("\\")) {
                int index = filter.LastIndexOf("\\");
                dirPath = filter.Substring(0, index + 1);
                dirFilter = filter.Substring(index + 1) + "*";
            }
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            lst.AddRange(dirInfo.GetDirectories(dirFilter));
            lst.AddRange(dirInfo.GetFiles(dirFilter));
            return lst;
        }
    }
}