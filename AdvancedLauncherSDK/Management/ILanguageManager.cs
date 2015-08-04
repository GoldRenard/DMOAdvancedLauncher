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

namespace AdvancedLauncher.SDK.Management {

    public interface ILanguageManager : IManager {

        string LanguagesPath {
            get;
        }

        string Initialize(string languagesPath, string currentLanguage);

        void Save(string filename, LanguageModel model);

        LanguageModel Read(string tFile);

        bool Load(string tName);

        string[] GetTranslations();

        event SDK.Model.Events.EventHandler LanguageChanged;

        string GetDefaultName();

        LanguageModel Model {
            set;
            get;
        }
    }
}