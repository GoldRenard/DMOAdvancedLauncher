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
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.Management {

    public class LogManager : ILogManager {
        private log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(LogManager));

        public void Initialize() {
            LOGGER = log4net.LogManager.GetLogger(typeof(LogManager));
        }

        public void InitializeLogFor(Type type) {
            LOGGER = log4net.LogManager.GetLogger(type);
        }

        public bool IsDebugEnabled {
            get {
                return LOGGER.IsDebugEnabled;
            }
        }

        public bool IsErrorEnabled {
            get {
                return LOGGER.IsErrorEnabled;
            }
        }

        public bool IsFatalEnabled {
            get {
                return LOGGER.IsFatalEnabled;
            }
        }

        public bool IsInfoEnabled {
            get {
                return LOGGER.IsInfoEnabled;
            }
        }

        public bool IsWarnEnabled {
            get {
                return LOGGER.IsWarnEnabled;
            }
        }

        public void Debug(object message) {
            LOGGER.Debug(message);
        }

        public void Debug(object message, Exception exception) {
            LOGGER.Debug(message, exception);
        }

        public void DebugFormat(string format, object arg0) {
            LOGGER.DebugFormat(format, arg0);
        }

        public void DebugFormat(string format, params object[] args) {
            LOGGER.DebugFormat(format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args) {
            LOGGER.DebugFormat(provider, format, args);
        }

        public void DebugFormat(string format, object arg0, object arg1) {
            LOGGER.DebugFormat(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2) {
            LOGGER.DebugFormat(format, arg0, arg1, arg2);
        }

        public void Error(object message) {
            LOGGER.Error(message);
        }

        public void Error(object message, Exception exception) {
            LOGGER.Error(message, exception);
        }

        public void ErrorFormat(string format, object arg0) {
            LOGGER.ErrorFormat(format, arg0);
        }

        public void ErrorFormat(string format, params object[] args) {
            LOGGER.ErrorFormat(format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {
            LOGGER.ErrorFormat(provider, format, args);
        }

        public void ErrorFormat(string format, object arg0, object arg1) {
            LOGGER.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2) {
            LOGGER.ErrorFormat(format, arg0, arg1, arg2);
        }

        public void Fatal(object message) {
            LOGGER.Fatal(message);
        }

        public void Fatal(object message, Exception exception) {
            LOGGER.Fatal(message, exception);
        }

        public void FatalFormat(string format, object arg0) {
            LOGGER.FatalFormat(format, arg0);
        }

        public void FatalFormat(string format, params object[] args) {
            LOGGER.FatalFormat(format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args) {
            LOGGER.FatalFormat(provider, format, args);
        }

        public void FatalFormat(string format, object arg0, object arg1) {
            LOGGER.FatalFormat(format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2) {
            LOGGER.FatalFormat(format, arg0, arg1, arg2);
        }

        public void Info(object message) {
            LOGGER.Info(message);
        }

        public void Info(object message, Exception exception) {
            LOGGER.Info(message, exception);
        }

        public void InfoFormat(string format, object arg0) {
            LOGGER.InfoFormat(format, arg0);
        }

        public void InfoFormat(string format, params object[] args) {
            LOGGER.InfoFormat(format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args) {
            LOGGER.InfoFormat(provider, format, args);
        }

        public void InfoFormat(string format, object arg0, object arg1) {
            LOGGER.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2) {
            LOGGER.InfoFormat(format, arg0, arg1, arg2);
        }

        public void Warn(object message) {
            LOGGER.Warn(message);
        }

        public void Warn(object message, Exception exception) {
            LOGGER.Warn(message, exception);
        }

        public void WarnFormat(string format, object arg0) {
            LOGGER.WarnFormat(format, arg0);
        }

        public void WarnFormat(string format, params object[] args) {
            LOGGER.WarnFormat(format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args) {
            LOGGER.WarnFormat(provider, format, args);
        }

        public void WarnFormat(string format, object arg0, object arg1) {
            LOGGER.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2) {
            LOGGER.WarnFormat(format, arg0, arg1, arg2);
        }
    }
}