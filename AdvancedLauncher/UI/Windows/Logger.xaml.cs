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

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Commands;
using log4net.Core;
using Ninject;

namespace AdvancedLauncher.UI.Windows {

    public partial class Logger : AbstractWindowControl {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(Logger));

        private delegate void AddLogHandler(LoggingEvent logEvent);

        private int recentIndex = -1;

        private bool FirstShow = true;

        #region Properties and structs

        private enum LogLevel {
            DEBUG,
            ERROR,
            FATAL,
            INFO,
            WARN,
            OTNER
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

        [Inject]
        public ICommandManager CommandManager {
            get; set;
        }

        #endregion Properties and structs

        public Logger() {
            InitializeComponent();
            this.Items.ItemsSource = LogEntriesFiltered;
        }

        public void PrintHeader() {
            LOGGER.Info("Digimon Masters Online Advanced Launcher, Copyright (C) 2015 Egorov Ilya" + System.Environment.NewLine +
                "This program comes with ABSOLUTELY NO WARRANTY; for details type `license'." + System.Environment.NewLine +
                "This is free software, and you are welcome to redistribute it" + System.Environment.NewLine +
                "under certain conditions; type `license' for details." + System.Environment.NewLine);
        }

        public override void OnShow() {
            this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate () {
                ConsoleInput.Focus();
            }));
            if (FirstShow) {
                CommandManager.RegisterCommand(new ClearCommand(this));
                PrintHeader();
                FirstShow = false;
            }
        }

        public void AddEntry(LoggingEvent logEvent) {
            if (this.Dispatcher != null && !this.Dispatcher.CheckAccess()) {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new AddLogHandler((_logEvent) => {
                    AddEntry(_logEvent);
                }), logEvent);
                return;
            }
            AddFilteredEntry(logEvent);
            _LogEntries.Add(logEvent);
        }

        public void AddFilteredEntry(LoggingEvent logEvent) {
            if (this.Dispatcher != null && !this.Dispatcher.CheckAccess()) {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new AddLogHandler((_logEvent) => {
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
            CommandManager.Send(ConsoleInput.Text.Trim());
            ConsoleInput.Clear();
            recentIndex = CommandManager.GetRecent().Count;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Return:
                    OnConsoleSendClick(sender, e);
                    break;

                case Key.Up:
                case Key.Down:
                    int newIndex = GetNewIndex(e.Key);
                    if (newIndex >= 0 && CommandManager.GetRecent().Count > newIndex) {
                        recentIndex = newIndex;
                        ConsoleInput.Text = CommandManager.GetRecent()[newIndex];
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

        public class ClearCommand : AbstractCommand {

            private Logger Logger {
                get;
                set;
            }

            public ClearCommand(Logger Logger)
                : base("clear", "Clears the console log") {
                this.Logger = Logger;
            }

            public override bool DoCommand(string[] args) {
                Logger.LogEntries.Clear();
                Logger.LogEntriesFiltered.Clear();
                Logger.PrintHeader();
                return true;
            }
        }

        #endregion Console Things
    }
}