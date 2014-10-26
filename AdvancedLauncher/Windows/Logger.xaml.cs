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
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using log4net.Appender;
using log4net.Core;

using AdvancedLauncher.Service;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Environment.Commands;
using System.IO;

namespace AdvancedLauncher.Windows {
    public partial class Logger : UserControl, INotifyPropertyChanged {
        private Storyboard ShowWindow, HideWindow;
        public delegate void AddLogHandler(LoggingEvent logEvent);
        private int recentIndex = -1;

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
                Dispatcher.BeginInvoke(
                    DispatcherPriority.ContextIdle,
                    new Action(delegate() {
                        ConsoleInput.Focus();
                    }));
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

        public void AddEntry(LoggingEvent logEvent) {
            if (this.Dispatcher != null && !this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new AddLogHandler((_logEvent) => {
                    AddEntry(_logEvent);
                }), logEvent);
                return;
            }
            if (LogEntries.Count == 0) {
                LogEntries.Add(logEvent);
            } else {
                LogEntries.Insert(0, logEvent);
            }
            NotifyPropertyChanged("LogEntries");
        }

        private void OnConsoleSendClick(object sender, RoutedEventArgs e) {
            CommandHandler.Send(ConsoleInput.Text.Trim());
            ConsoleInput.Clear();
            recentIndex = CommandHandler.GetRecent().Count;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Return:
                    OnConsoleSendClick(sender, e);
                    break;
                case Key.Up:
                case Key.Down:
                    int newIndex = GetNewIndex(e.Key);
                    if (newIndex >= 0 && CommandHandler.GetRecent().Count > newIndex) {
                        recentIndex = newIndex;
                        ConsoleInput.Text = CommandHandler.GetRecent()[newIndex];
                        ConsoleInput.CaretIndex = ConsoleInput.Text.Length;
                    }
                    break;
            }
        }

        private int GetNewIndex(Key key) {
            switch (key) {
                case Key.Up:
                    if (recentIndex <= 0) {
                        return recentIndex;
                    } else {
                        return recentIndex - 1;
                    }
                case Key.Down:
                    return recentIndex + 1;
                default:
                    return recentIndex;
            }
        }
    }
}
