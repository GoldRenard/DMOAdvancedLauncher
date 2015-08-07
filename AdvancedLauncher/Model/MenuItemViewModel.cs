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
using System.Windows.Media;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;

namespace AdvancedLauncher.Model {

    public class MenuItemViewModel : NamedItemViewModel<MenuItem> {
        private static SolidColorBrush Brush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        public MenuItemViewModel(MenuItem Item, ILanguageManager LanguageManager)
            : base(Item, LanguageManager) {
        }

        public VisualBrush IconBrush {
            get {
                if (Item.IconName != null) {
                    var IconCanvas = App.Current.TryFindResource(Item.IconName) as System.Windows.Controls.Canvas;
                    if (IconCanvas != null) {
                        if (!IconCanvas.Resources.Contains("BlackBrush")) {
                            IconCanvas.Resources.Add("BlackBrush", Brush);
                        }
                        return new VisualBrush(IconCanvas);
                    }
                }
                return null;
            }
        }

        public Thickness IconMargin {
            get {
                if (Item.IconMargin != null) {
                    return Item.IconMargin.ToRealThickness();
                }
                return new Thickness(0);
            }
        }

        public bool IsSeparator {
            get {
                return Item.IsSeparator;
            }
        }

        public Visibility SeparatorVisibility {
            get {
                return Item.SeparatorVisibility;
            }
        }

        public Visibility ItemVisibility {
            get {
                return Item.ItemVisibility;
            }
        }

        protected override void NotifyPropertyChanged(string propertyName) {
            base.NotifyPropertyChanged(propertyName);
            if ("IconName".Equals(propertyName)) {
                NotifyPropertyChanged("IconBrush");
            }
        }
    }
}