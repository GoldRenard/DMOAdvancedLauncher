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
using System.Globalization;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Config;
using Ninject;

namespace AdvancedLauncher.UI.Converters {

    public class ProfileNameConverter : AbstractConverter {

        [Inject]
        public IConfigurationManager ConfigurationManager {
            get; set;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Profile profile = value as Profile;
            if (profile != null) {
                return string.Format("{0} ({1})", profile.Name, ConfigurationManager.GetConfiguration(profile.GameModel).Name);
            }
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}