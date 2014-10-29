// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Environment.Commands;
using log4net.Core;

namespace AdvancedLauncher.Windows {

    public partial class Logger : UserControl {
        private Storyboard ShowWindow, HideWindow;

        public delegate void AddLogHandler(LoggingEvent logEvent);

        private int recentIndex = -1;

        private class ClearCommand : Command {
            private readonly Logger loggerInstance;

            public ClearCommand(Logger loggerInstance)
                : base("clear", "Clears the console log") {
                this.loggerInstance = loggerInstance;
            }

            public override void DoCommand(string[] args) {
                loggerInstance._LogEntries.Clear();
                loggerInstance._LogEntriesFiltered.Clear();
            }
        }

        private enum LogLevel {
            DEBUG,
            ERROR,
            FATAL,
            INFO,
            WARN,
            OTNER
        }

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

        private ObservableCollection<LoggingEvent> _LogEntries = new ObservableCollection<LoggingEvent>();
        private ObservableCollection<LoggingEvent> _LogEntriesFiltered = new ObservableCollection<LoggingEvent>();

        public ObservableCollection<LoggingEvent> LogEntries {
            get {
                return _LogEntries;
            }
        }

        public ObservableCollection<LoggingEvent> LogEntriesFiltered {
            get {
                return _LogEntriesFiltered;
            }
        }

        private Logger() {
            InitializeComponent();
            LanguageEnv.Languagechanged += delegate() {
                this.DataContext = LanguageEnv.Strings;
            };
            ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
            HideWindow = ((Storyboard)this.FindResource("HideWindow"));
            this.Items.ItemsSource = LogEntriesFiltered;

            CommandHandler.RegisterCommand(new ClearCommand(this));
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

        public void AddEntry(LoggingEvent logEvent) {
            if (this.Dispatcher != null && !this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new AddLogHandler((_logEvent) => {
                    AddEntry(_logEvent);
                }), logEvent);
                return;
            }
            AddFilteredEntry(logEvent);
            _LogEntries.Add(logEvent);
        }

        public void AddFilteredEntry(LoggingEvent logEvent) {
            if (this.Dispatcher != null && !this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new AddLogHandler((_logEvent) => {
                    AddFilteredEntry(_logEvent);
                }), logEvent);
                return;
            }
            if (IsApplicable(logEvent) == true) {
                _LogEntriesFiltered.Add(logEvent);
            }
        }

        #region Filter Things

        private void OnFilterChecked(object sender, RoutedEventArgs e) {
            _LogEntriesFiltered.Clear();
            foreach (LoggingEvent log in LogEntries) {
                AddFilteredEntry(log);
            }
        }

        private LogLevel ConvertLevel(Level logLevel) {
            if (Level.Debug.Equals(logLevel)) {
                return LogLevel.DEBUG;
            } else if (Level.Error.Equals(logLevel)) {
                return LogLevel.ERROR;
            } else if (Level.Fatal.Equals(logLevel)) {
                return LogLevel.FATAL;
            } else if (Level.Warn.Equals(logLevel)) {
                return LogLevel.WARN;
            } else if (Level.Info.Equals(logLevel)) {
                return LogLevel.INFO;
            }
            return LogLevel.OTNER;
        }

        private bool? IsApplicable(LoggingEvent logEvent) {
            switch (ConvertLevel(logEvent.Level)) {
                case LogLevel.DEBUG:
                    return FilterDebug.IsChecked;

                case LogLevel.ERROR:
                    return FilterError.IsChecked;

                case LogLevel.FATAL:
                    return FilterFatal.IsChecked;

                case LogLevel.INFO:
                    return FilterInfo.IsChecked;

                case LogLevel.WARN:
                    return FilterWarn.IsChecked;

                default:
                    return true;
            }
        }

        #endregion Filter Things

        #region Console Things

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

        #endregion Console Things
    }
}