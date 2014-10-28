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

using System;
using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AdvancedLauncher.Controls {
    public sealed partial class HintPasswordBox : UserControl, INotifyPropertyChanged {
        private bool IsPreventUpdate = false;
        private const string FakePass = "empty_pass";
        public HintPasswordBox() {
            InitializeComponent();
            LayoutRoot.DataContext = this;
            pBox.PasswordChanged += pBox_PasswordChanged;
        }

        void pBox_PasswordChanged(object sender, RoutedEventArgs e) {
            if (IsPreventUpdate) {
                return;
            }

            IsEmpty = pBox.Password.Length == 0;
            Password = pBox.SecurePassword;
            NotifyPropertyChanged("Password");
        }

        #region TextProperty

        public bool IsEmpty {
            get { return pBox.Password.Length == 0; }
            set { NotifyPropertyChanged("IsEmpty"); }
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(SecureString), typeof(HintPasswordBox), new PropertyMetadata(OnPasswordChanged));
        public static readonly DependencyProperty HindTextProperty = DependencyProperty.Register("HintText", typeof(string), typeof(HintPasswordBox));

        static void OnPasswordChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            (obj as HintPasswordBox).OnPasswordChanged((HintPasswordBox)obj, args);
        }

        void OnPasswordChanged(HintPasswordBox sender, DependencyPropertyChangedEventArgs args) {
            if (sender.pBox.IsFocused)
                return;
            IsPreventUpdate = true;
            if (Password != null) {
                if (Password.Length > 0)
                    pBox.Password = FakePass;
                else
                    pBox.Password = string.Empty;
            } else
                pBox.Password = string.Empty;
            IsPreventUpdate = false;
            NotifyPropertyChanged("Password");
            NotifyPropertyChanged("IsEmpty");
        }

        public SecureString Password {
            set { SetValue(PasswordProperty, value); }
            get { return (SecureString)GetValue(PasswordProperty); }
        }

        Thickness _TextMargin;
        public Thickness TextMargin {
            get { return _TextMargin; }
            set { if (_TextMargin != value) { _TextMargin = value; NotifyPropertyChanged("TextMargin"); } }
        }

        string _HintText;
        public string HintText {
            get { return _HintText; }
            set { if (_HintText != value) { _HintText = value; NotifyPropertyChanged("_HintText"); } }
        }

        double _HintFontSize;
        public double HintFontSize {
            get { return _HintFontSize; }
            set { if (_HintFontSize != value) { _HintFontSize = value; NotifyPropertyChanged("HintFontSize"); } }
        }

        FontFamily _HintFontFamily;
        public FontFamily HintFontFamily {
            get { return _HintFontFamily; }
            set { if (_HintFontFamily != value) { _HintFontFamily = value; NotifyPropertyChanged("HintFontFamily"); } }
        }

        FontStretch _HintFontStretch;
        public FontStretch HintFontStretch {
            get { return _HintFontStretch; }
            set { if (_HintFontStretch != value) { _HintFontStretch = value; NotifyPropertyChanged("HintFontStretch"); } }
        }

        FontStyle _HintFontStyle;
        public FontStyle HintFontStyle {
            get { return _HintFontStyle; }
            set { if (_HintFontStyle != value) { _HintFontStyle = value; NotifyPropertyChanged("HintFontWeight"); } }
        }

        FontWeight _HintFontWeight;
        public FontWeight HintFontWeight {
            get { return _HintFontWeight; }
            set { if (_HintFontWeight != value) { _HintFontWeight = value; NotifyPropertyChanged("HintFontWeight"); } }
        }

        #endregion

        #region ImageProperty
        ImageSource _Image;
        public ImageSource Image {
            get { return _Image; }
            set { if (_Image != value) { _Image = value; NotifyPropertyChanged("Image"); } }
        }

        Thickness _ImageMargin;
        public Thickness ImageMargin {
            get { return _ImageMargin; }
            set { if (_ImageMargin != value) { _ImageMargin = value; NotifyPropertyChanged("ImageMargin"); } }
        }

        double _ImageWidth;
        public double ImageWidth {
            get { return _ImageWidth; }
            set { if (_ImageWidth != value) { _ImageWidth = value; NotifyPropertyChanged("ImageWidth"); } }
        }

        double _ImageOpacity = 1.0;
        public double ImageOpacity {
            get { return _ImageOpacity; }
            set { if (_ImageOpacity != value) { _ImageOpacity = value; NotifyPropertyChanged("ImageOpacity"); } }
        }

        double _ImageHeight;
        public double ImageHeight {
            get { return _ImageHeight; }
            set { if (_ImageHeight != value) { _ImageHeight = value; NotifyPropertyChanged("ImageHeight"); } }
        }

        Stretch _ImageStretch;
        public Stretch ImageStretch {
            get { return _ImageStretch; }
            set { if (_ImageStretch != value) { _ImageStretch = value; NotifyPropertyChanged("ImageStretch"); } }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
