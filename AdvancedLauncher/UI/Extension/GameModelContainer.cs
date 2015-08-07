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
using AdvancedLauncher.SDK.Model.Config;

namespace AdvancedLauncher.UI.Extension {

    public class GameModelContainer : DependencyObject {
        public static readonly DependencyProperty GameModelProperty = DependencyProperty.Register("GameModel", typeof(GameModel), typeof(GameModelContainer));

        public GameModel GameModel {
            get {
                return (GameModel)GetValue(GameModelProperty);
            }

            set {
                SetValue(GameModelProperty, value);
            }
        }
    }
}