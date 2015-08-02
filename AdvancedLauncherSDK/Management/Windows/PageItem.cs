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
using System.Windows.Media;

namespace AdvancedLauncher.SDK.Management.Windows {

    public class PageItem : NamedItem {
        private bool _IsEnabled = true;

        private static SolidColorBrush Brush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        public PageItem(string Label, Control Content)
            : this(Label, null, null, Content) {
        }

        public PageItem(ILanguageManager LanguageManager, string BindingName, Control Content)
            : this(null, LanguageManager, BindingName, Content) {
        }

        private PageItem(string Label, ILanguageManager LanguageManager, string bindingName, Control Content)
            : base(Label, LanguageManager, bindingName) {
            this.Content = Content;
        }

        public Control Content {
            get;
            set;
        }

        public bool IsEnabled {
            set {
                _IsEnabled = value;
                NotifyPropertyChanged("IsEnabled");
            }
            get {
                return _IsEnabled;
            }
        }
    }
}