﻿// ======================================================================
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
using System.Windows.Controls;

namespace AdvancedLauncher.UI.Validation {

    internal class ProfileNameValidationRule : AbstractValidationRule {

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) {
            if (string.IsNullOrEmpty(value.ToString().Trim())) {
                return new ValidationResult(false, LanguageManager.Model.Settings_ProfileNameHint);
            }
            int code = 0;

            if (value.ToString().IndexOfAny("*^%@&^@#><>!.,$|`~?:\":\\/';=-+".ToCharArray()) != -1)
                return new ValidationResult(false, LanguageManager.Model.Settings_ProfileNameHint);

            foreach (char chr in value.ToString()) {
                code = Convert.ToInt32(chr);
                if (Char.IsControl(chr)) {
                    return new ValidationResult(false, LanguageManager.Model.Settings_ProfileNameHint);
                }
            }
            return new ValidationResult(true, LanguageManager.Model.Settings_ProfileNameHint);
        }
    }
}