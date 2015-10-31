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
using System.Windows.Data;

namespace AdvancedLauncher.UI.Extension {

    #region "Fields"

    public class BindingEvaluator : FrameworkElement {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(BindingEvaluator), new FrameworkPropertyMetadata(string.Empty));

        private Binding _valueBinding;

        #endregion "Fields"

        #region "Constructors"

        public BindingEvaluator(Binding binding) {
            ValueBinding = binding;
        }

        #endregion "Constructors"

        #region "Properties"

        public string Value {
            get {
                return (string)GetValue(ValueProperty);
            }

            set {
                SetValue(ValueProperty, value);
            }
        }

        public Binding ValueBinding {
            get {
                return _valueBinding;
            }
            set {
                _valueBinding = value;
            }
        }

        #endregion "Properties"

        #region "Methods"

        public string Evaluate(object dataItem) {
            this.DataContext = dataItem;
            SetBinding(ValueProperty, ValueBinding);
            return Value;
        }

        #endregion "Methods"
    }
}