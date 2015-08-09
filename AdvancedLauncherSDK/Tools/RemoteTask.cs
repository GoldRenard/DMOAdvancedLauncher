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

    /// <summary>
    /// <see cref="Task"/> wrapper. Async calls across AppDomains aren't supported out the box, so this is an workaround.
    /// </summary>
    /// <typeparam name="T">Async return type</typeparam>
    public class RemoteTask<T> : CrossDomainObject {
        private readonly EventWaitHandle CompletionEvent;

        /// <summary>
        /// Gets task identifier
        /// </summary>
        public string Id {
            get; private set;
        }

        /// <summary>
        /// Gets <b>True</b> if task execution was completed
        /// </summary>
        public bool IsCompleted {
            get; private set;
        }

        /// <summary>
        /// Gets <b>True</b> if task execution was failed
        /// </summary>
        public bool IsFaulted {
            get; private set;
        }

        /// <summary>
        /// Gets result of task execution
        /// </summary>
        public T Result {
            get; private set;
        }

        private Task<T> WorkerTask {
            get; set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RemoteTask{T}"/> for specified <see cref="Task{T}"/>.
        /// </summary>
        /// <param name="WorkerTask">Original task instance</param>
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

        /// <summary>
        /// Calls on task finish
        /// </summary>
        protected void Finished() {
            Result = WorkerTask.Result;
            CompletionEvent.Set();
        }
    }

    /// <summary>
    /// Extension to return real <see cref="Task"/>, linked with  <see cref="RemoteTask{T}"/>, so we can use await call
    /// </summary>
    public static class RemoteTaskExt {

        /// <summary>
        /// Waits until <see cref="RemoteTask{T}"/> completion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="worker">Current <see cref="RemoteTask{T}"/> worker</param>
        /// <returns></returns>
        public static Task<T> Wait<T>(this RemoteTask<T> worker) {
            return Task.Factory.StartNew<T>(() => {
                var waiter = new EventWaitHandle(false, EventResetMode.ManualReset, worker.Id);
                waiter.WaitOne();
                return worker.Result;
            });
        }
    }
}