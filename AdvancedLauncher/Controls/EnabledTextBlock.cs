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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AdvancedLauncher.Controls {
    public class EnabledTextBlock : TextBlock {

        static EnabledTextBlock() {
            IsEnabledProperty.OverrideMetadata(typeof(EnabledTextBlock), new UIPropertyMetadata(true, (d, e) => {
                var childrencount = VisualTreeHelper.GetChildrenCount(d);
                for (int i = 0; i < childrencount; i++) {
                    var child = VisualTreeHelper.GetChild(d, i);
                    child.CoerceValue(IsEnabledProperty);
                }
            }, (d, basevalue) => {
                return basevalue;
            }));
        }
    }
}
