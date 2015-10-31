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
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.UI.Extension;
using Ninject;

namespace AdvancedLauncher.UI.Validation {

    public class GamePathValidationRule : AbstractValidationRule {

        [Inject]
        public IConfigurationManager ConfigurationManager {
            get; set;
        }

        public GameModelContainer Container {
            get;
            set;
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {
            if (Container == null) {
                return new ValidationResult(false, LanguageManager.Model.PleaseSelectGamePath);
            }
            if (Container.GameModel == null) {
                return new ValidationResult(false, LanguageManager.Model.PleaseSelectGamePath);
            }
            if (Container.GameModel.Type == null) {
                return new ValidationResult(false, LanguageManager.Model.PleaseSelectGamePath);
            }
            if (ConfigurationManager.CheckGame(Container.GameModel)) {
                return new ValidationResult(true, LanguageManager.Model.PleaseSelectGamePath);
            }
            return new ValidationResult(false, LanguageManager.Model.PleaseSelectGamePath);
        }
    }
}