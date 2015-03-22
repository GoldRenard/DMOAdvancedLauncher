// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AdvancedLauncher.Controls {

    public partial class LogoControl : UserControl {

        public LogoControl() {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
        }

        public HeightToFontSizeConverter TitleConverter {
            get {
                return new HeightToFontSizeConverter(1);
            }
        }

        public class HeightToFontSizeConverter : IValueConverter {

            private readonly double Ratio = 1;

            public HeightToFontSizeConverter(double ratio) {
                this.Ratio = ratio;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
                var height = (double)value;
                return Ratio * height;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
                throw new NotImplementedException();
            }
        }
    }
}