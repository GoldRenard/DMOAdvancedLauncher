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
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace AdvancedLauncher.UI.Converters {

    public class BooleanConverter<T> : IValueConverter {

        public BooleanConverter(T trueValue, T falseValue) {
            True = trueValue;
            False = falseValue;
        }

        public T True {
            get; set;
        }

        public T False {
            get; set;
        }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is bool && ((bool)value) ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }
    }
}