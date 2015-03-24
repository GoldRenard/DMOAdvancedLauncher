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

namespace AdvancedLauncher.Controls {

    public partial class HaguruLoader : UserControl {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(HaguruLoader));
        public static readonly DependencyProperty SummaryProperty = DependencyProperty.Register("Summary", typeof(string), typeof(HaguruLoader));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(HaguruLoader));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(HaguruLoader));

        public HaguruLoader() {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        public string Title {
            get {
                return this.GetValue(TitleProperty) as string;
            }
            set {
                this.SetValue(TitleProperty, value);
            }
        }

        public string Summary {
            get {
                return this.GetValue(SummaryProperty) as string;
            }
            set {
                this.SetValue(SummaryProperty, value);
            }
        }

        public double Value {
            get {
                return (double)this.GetValue(ValueProperty);
            }
            set {
                this.SetValue(ValueProperty, value);
            }
        }

        public double Maximum {
            get {
                return (double)this.GetValue(MaximumProperty);
            }
            set {
                this.SetValue(MaximumProperty, value);
            }
        }
    }
}