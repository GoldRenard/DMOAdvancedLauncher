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

using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Tools;

namespace AdvancedLauncher.SDK.Model {

    /// <summary>
    /// Main menu item container
    /// </summary>
    public class MenuItem : NamedItem {

        /// <summary>
        /// Click event handler
        /// </summary>
        public event BaseEventHandler Click;

        public MenuItem(string Name, bool IsNameBinding = false)
            : this(Name, null, null, false, IsNameBinding) {
        }

        public MenuItem(string Name, string IconName, Thickness IconMargin, bool IsNameBinding = false)
            : this(Name, IconName, IconMargin, false, IsNameBinding) {
        }

        private MenuItem(string Name, string IconName, Thickness IconMargin, bool IsSeparator, bool IsNameBinding = false)
            : base(Name, IsNameBinding) {
            this.IconMargin = IconMargin;
            this.IconName = IconName;
            this.IsSeparator = IsSeparator;
        }

        private string _IconName;

        /// <summary>
        /// Icon name for menu item. It is resource name. You can use MahApps.Metro.Resources icons:
        /// https://github.com/MahApps/MahApps.Metro/MahApps.Metro.Resources/Icons.xaml
        /// </summary>
        public string IconName {
            get {
                return _IconName;
            }
            set {
                if (_IconName != value) {
                    _IconName = value;
                }
                NotifyPropertyChanged("IconName");
            }
        }

        private Thickness _IconMargin;

        /// <summary>
        /// Icon margin
        /// </summary>
        public Thickness IconMargin {
            get {
                return _IconMargin;
            }
            set {
                if (_IconMargin != value) {
                    _IconMargin = value;
                }
                NotifyPropertyChanged("IconMargin");
            }
        }

        private bool _IsSeparator;

        /// <summary>
        /// Is it separator instead of usual item
        /// </summary>
        public bool IsSeparator {
            get {
                return _IsSeparator;
            }
            set {
                if (_IsSeparator != value) {
                    _IsSeparator = value;
                }
                NotifyPropertyChanged("IsSeparator");
                NotifyPropertyChanged("SeparatorVisibility");
                NotifyPropertyChanged("ItemVisibility");
            }
        }

        /// <summary>
        /// Separator visibility based on <see cref="IsSeparator"/>
        /// </summary>
        public System.Windows.Visibility SeparatorVisibility {
            get {
                return IsSeparator ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Item visibility based on not being <see cref="IsSeparator"/>
        /// </summary>
        public System.Windows.Visibility ItemVisibility {
            get {
                return !IsSeparator ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Returns new separator item
        /// </summary>
        public static MenuItem Separator {
            get {
                return new MenuItem("", null, new Thickness(), true, false);
            }
        }

        /// <summary>
        /// OnClick handler accessor
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Arguments</param>
        public void OnClick(object sender, BaseEventArgs args) {
            if (Click != null) {
                Click(sender, args);
            }
        }
    }
}