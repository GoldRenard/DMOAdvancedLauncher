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
using System.Windows.Input;
using System.Windows.Media;

namespace AdvancedLauncher.SDK.Management.Windows {

    public class MenuItem : NamedItem {
        private static SolidColorBrush Brush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        public MenuItem(string Label, Canvas icon, Thickness iconMargin, ICommand command)
            : this(Label, null, null, icon, iconMargin, command) {
        }

        public MenuItem(ILanguageManager LanguageManager, string bindingName, Canvas icon, Thickness iconMargin, ICommand command)
            : this(null, LanguageManager, bindingName, icon, iconMargin, command) {
        }

        private MenuItem(string Label, ILanguageManager LanguageManager, string bindingName, Canvas icon, Thickness iconMargin, ICommand command)
            : base(Label, LanguageManager, bindingName) {
            Command = command;
            IconMargin = iconMargin;
            if (icon != null) {
                if (!icon.Resources.Contains("BlackBrush")) {
                    icon.Resources.Add("BlackBrush", Brush);
                }
                IconBrush = new VisualBrush(icon);
            }
            if (LanguageManager != null) {
                LanguageManager.LanguageChanged += (s, e) => {
                    this.NotifyPropertyChanged("Name");
                };
            }
        }

        public ICommand Command {
            get;
            set;
        }

        public bool IsEnabled {
            get {
                if (Command == null) {
                    return false;
                }
                return Command.CanExecute(Command);
            }
        }

        public VisualBrush IconBrush {
            get;
            set;
        }

        public Thickness IconMargin {
            get;
            set;
        }

        public Visibility SeparatorVisibility {
            get {
                return Command == null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ItemVisibility {
            get {
                return Command != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void NotifyEnabled() {
            NotifyPropertyChanged("IsEnabled");
        }

        public static MenuItem Separator {
            get {
                return new MenuItem("", null, null, null, new Thickness(), null);
            }
        }
    }
}