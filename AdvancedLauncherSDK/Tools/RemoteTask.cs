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
using System.Threading;
using System.Threading.Tasks;
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Tools {

    public class RemoteTask<T> : CrossDomainObject {
        private readonly EventWaitHandle CompletionEvent;

        public string Id {
            get; private set;
        }

        public bool IsCompleted {
            get; private set;
        }

        public bool IsFaulted {
            get; private set;
        }

        public T Result {
            get; private set;
        }

        private Task<T> WorkerTask {
            get; set;
        }

        public RemoteTask(Task<T> WorkerTask) {
            this.WorkerTask = WorkerTask;
            Id = Guid.NewGuid().ToString();
            CompletionEvent = new EventWaitHandle(false, EventResetMode.ManualReset, Id);
            WorkerTask.GetAwaiter().OnCompleted(() => {
                try {
                    WorkerTask.GetAwaiter().GetResult();
                    IsFaulted = false;
                    IsCompleted = WorkerTask.GetAwaiter().IsCompleted;
                } catch (Exception) {
                    IsFaulted = true;
                }
                Finished();
            });
        }

        protected void Finished() {
            Result = WorkerTask.Result;
            CompletionEvent.Set();
        }
    }

    public static class RemoteTaskExt {

        public static Task<T> Wait<T>(this RemoteTask<T> worker) {
            return Task.Factory.StartNew<T>(() => {
                var waiter = new EventWaitHandle(false, EventResetMode.ManualReset, worker.Id);
                waiter.WaitOne();
                return worker.Result;
            });
        }
    }
}