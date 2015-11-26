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

using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Security;
using System.Security.Permissions;
using log4net.Appender;
using log4net.Core;

namespace AdvancedLauncher.Tools {

    public class LogAppender : AppenderSkeleton {
        private static object lockObject = new object();

        public static ConcurrentQueue<LoggingEvent> Entries {
            get;
            private set;
        } = new ConcurrentQueue<LoggingEvent>();

        public static event NotifyCollectionChangedEventHandler CollectionChanged;

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        protected override void Append(LoggingEvent loggingEvent) {
            lock (lockObject) {
                Entries.Enqueue(loggingEvent);
                OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, loggingEvent));
            }
        }

        public static void Clear() {
            lock (lockObject) {
                LoggingEvent someItem;
                while (!Entries.IsEmpty) {
                    Entries.TryDequeue(out someItem);
                }
                OnCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
            if (CollectionChanged != null) {
                CollectionChanged(sender, args);
            }
        }
    }
}