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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using log4net;
using log4net.Appender;
using log4net.Core;

using AdvancedLauncher.Service;
using AdvancedLauncher.Environment;
using System.IO;

namespace AdvancedLauncher.Windows {
    public partial class Logger : UserControl, INotifyPropertyChanged {
        private Storyboard ShowWindow, HideWindow;

        private static Logger _Instance;
        public static Logger Instance {
            get {
                if (!IsInstanceInitialized) {
                    _Instance = new Logger() {
                        Visibility = Visibility.Collapsed
                    };
                }
                return _Instance;
            }
        }
        public static bool IsInstanceInitialized {
            get {
                return _Instance != null;
            } 
        }

        private static ObservableCollection<LoggingEvent> _LogEntries = new ObservableCollection<LoggingEvent>();
        public static ObservableCollection<LoggingEvent> LogEntries {
            get {
                return _LogEntries;
            }
        }

        private Logger() {
            InitializeComponent();
            LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
            ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
            HideWindow = ((Storyboard)this.FindResource("HideWindow"));
            this.Items.ItemsSource = LogEntries;
        }

        public void Show(bool state) {
            if (state) {
                this.Visibility = Visibility.Visible;
                ShowWindow.Begin();
            } else {
                HideWindow.Begin();
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e) {
            Show(false);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public delegate void AddLogHandler(LoggingEvent logEvent);
        public void AddEntry(LoggingEvent logEvent) {
            if (this.Dispatcher != null && !this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new AddLogHandler((_logEvent) => {
                    LogEntries.Add(_logEvent);
                    NotifyPropertyChanged("LogEntries");
                }), logEvent);
                return;
            }
            LogEntries.Add(logEvent);
            NotifyPropertyChanged("LogEntries");
        }
    }
}
