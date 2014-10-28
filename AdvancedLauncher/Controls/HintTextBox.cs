// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

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

    public class HintTextBox : TextBox {

        static HintTextBox() {
            TextMarginProperty = DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(HintTextBox));
            IsEmptyProperty = DependencyProperty.Register("IsEmpty", typeof(bool), typeof(HintTextBox));
            HintTextProperty = DependencyProperty.Register("HintText", typeof(string), typeof(HintTextBox));
            HintFontSizeProperty = DependencyProperty.Register("HintFontSize", typeof(double), typeof(HintTextBox));
            HintFontFamilyProperty = DependencyProperty.Register("HintFontFamily", typeof(FontFamily), typeof(HintTextBox));
            HintFontStretchProperty = DependencyProperty.Register("HintFontStretch", typeof(FontStretch), typeof(HintTextBox));
            HintFontStyleProperty = DependencyProperty.Register("HintFontStyle", typeof(FontStyle), typeof(HintTextBox));
            HintFontWeightProperty = DependencyProperty.Register("HintFontWeight", typeof(FontWeight), typeof(HintTextBox));

            ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(HintTextBox));
            ImageMarginProperty = DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(HintTextBox));
            ImageMarginProperty = DependencyProperty.Register("ImageOpacity", typeof(double), typeof(HintTextBox), new UIPropertyMetadata(1.0));
            ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(HintTextBox));
            ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(HintTextBox));
            ImageStretchProperty = DependencyProperty.Register("ImageStretch", typeof(Stretch), typeof(HintTextBox));
        }

        public HintTextBox() {
            IsEmpty = true;
            this.TextChanged += HintTextBox_TextChanged;
        }

        private void HintTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            IsEmpty = this.Text.Length == 0;
        }

        #region TextProperty

        public static DependencyProperty TextMarginProperty;
        public static DependencyProperty IsEmptyProperty;
        public static DependencyProperty HintTextProperty;
        public static DependencyProperty HintFontSizeProperty;
        public static DependencyProperty HintFontFamilyProperty;
        public static DependencyProperty HintFontStretchProperty;
        public static DependencyProperty HintFontStyleProperty;
        public static DependencyProperty HintFontWeightProperty;

        public Thickness TextMargin {
            get {
                return (Thickness)base.GetValue(TextMarginProperty);
            }
            set {
                base.SetValue(TextMarginProperty, value);
            }
        }

        public bool IsEmpty {
            get {
                return (bool)base.GetValue(IsEmptyProperty);
            }
            set {
                base.SetValue(IsEmptyProperty, value);
            }
        }

        public string HintText {
            get {
                return (string)base.GetValue(HintTextProperty);
            }
            set {
                base.SetValue(HintTextProperty, value);
            }
        }

        public double HintFontSize {
            get {
                return (double)base.GetValue(HintFontSizeProperty);
            }
            set {
                base.SetValue(HintFontSizeProperty, value);
            }
        }

        public FontFamily HintFontFamily {
            get {
                return (FontFamily)base.GetValue(HintFontFamilyProperty);
            }
            set {
                base.SetValue(HintFontFamilyProperty, value);
            }
        }

        public FontStretch HintFontStretch {
            get {
                return (FontStretch)base.GetValue(HintFontStretchProperty);
            }
            set {
                base.SetValue(HintFontStretchProperty, value);
            }
        }

        public FontStyle HintFontStyle {
            get {
                return (FontStyle)base.GetValue(HintFontStyleProperty);
            }
            set {
                base.SetValue(HintFontStyleProperty, value);
            }
        }

        public FontWeight HintFontWeight {
            get {
                return (FontWeight)base.GetValue(HintFontWeightProperty);
            }
            set {
                base.SetValue(HintFontWeightProperty, value);
            }
        }

        #endregion TextProperty

        #region ImageProperty

        public static DependencyProperty ImageProperty;
        public static DependencyProperty ImageMarginProperty;
        public static DependencyProperty ImageOpacityProperty;
        public static DependencyProperty ImageWidthProperty;
        public static DependencyProperty ImageHeightProperty;
        public static DependencyProperty ImageStretchProperty;

        public ImageSource Image {
            get {
                return (ImageSource)base.GetValue(ImageProperty);
            }
            set {
                base.SetValue(ImageProperty, value);
            }
        }

        public Thickness ImageMargin {
            get {
                return (Thickness)base.GetValue(ImageMarginProperty);
            }
            set {
                base.SetValue(ImageMarginProperty, value);
            }
        }

        public double ImageOpacity {
            get {
                return (double)base.GetValue(ImageOpacityProperty);
            }
            set {
                base.SetValue(ImageOpacityProperty, value);
            }
        }

        public double ImageWidth {
            get {
                return (double)base.GetValue(ImageWidthProperty);
            }
            set {
                base.SetValue(ImageWidthProperty, value);
            }
        }

        public double ImageHeight {
            get {
                return (double)base.GetValue(ImageHeightProperty);
            }
            set {
                base.SetValue(ImageHeightProperty, value);
            }
        }

        public Stretch ImageStretch {
            get {
                return (Stretch)base.GetValue(ImageStretchProperty);
            }
            set {
                base.SetValue(ImageStretchProperty, value);
            }
        }

        #endregion ImageProperty
    }
}