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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;

namespace AdvancedLauncher.Management {

    /// <summary> Данный класс реализует своеобразный менеджер задач,
    /// цель которого - не дать приложению закрыться, пока есть хоть одна задача </summary>
    public class TaskManager : CrossDomainObject, ITaskManager {

        public void Initialize() {
            // nothing to do here
        }

        /// <summary> Список задач </summary>
        private List<TaskEntry> _Tasks = new List<TaskEntry>();

        public void AquireLock(TaskEntry entry) {
            lock (_Tasks) {
                _Tasks.Add(entry);
            }
        }

        public bool ReleaseLock(TaskEntry entry) {
            lock (_Tasks) {
                return _Tasks.Remove(entry);
            }
        }

        /// <summary> Проверка занятости приложения </summary>
        /// <returns> <see langword="true"/> если приложение занято какой-либо задачей,
        /// <see langword="false"/> если свободно и может быть закрыто. </returns>
        public bool IsBusy {
            get {
                return _Tasks.Count != 0;
            }
        }

        /// <summary> Метод закрытия приложения. Приложение будет закрыто тогда,
        /// когда не останется ни одной задачи </summary>
        public void CloseApp(bool forceClose = false) {
            BackgroundWorker queueWorker = new BackgroundWorker();
            queueWorker.DoWork += (s, e) => {
                while (IsBusy && !forceClose) {
                    System.Threading.Thread.Sleep(100);
                }
            };
            queueWorker.RunWorkerCompleted += (s, e) => {
                if (Application.Current == null) {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    return;
                }
                if (!Application.Current.Dispatcher.CheckAccess()) {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate () {
                        Application.Current.Shutdown();
                    }));
                } else {
                    Application.Current.Shutdown();
                }
            };
            queueWorker.RunWorkerAsync();
        }
    }
}