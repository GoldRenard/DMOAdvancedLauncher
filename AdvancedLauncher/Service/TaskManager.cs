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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace AdvancedLauncher.Service {

    /// <summary> Данный класс реализует своеобразный менеджер задач,
    /// цель которого - не дать приложению закрыться, пока есть хоть одна задача </summary>
    public static class TaskManager {

        /// <summary> Структура задачи </summary>
        public struct Task {
            public object Owner;
        }

        /// <summary> Список задач </summary>
        private static List<Task> _Tasks = new List<Task>();

        /// <summary> Предоставляет ссылку на список задач </summary>
        public static List<Task> Tasks {
            get {
                return _Tasks;
            }
        }

        /// <summary> Проверка занятости приложения </summary>
        /// <returns> <see langword="true"/> если приложение занято какой-либо задачей,
        /// <see langword="false"/> если свободно и может быть закрыто. </returns>
        public static bool IsBusy {
            get {
                return _Tasks.Count != 0;
            }
        }

        /// <summary> Метод закрытия приложения. Приложение будет закрыто тогда,
        /// когда не останется ни одной задачи </summary>
        public static void CloseApp() {
            BackgroundWorker queueWorker = new BackgroundWorker();
            queueWorker.DoWork += (s, e) => {
                while (IsBusy) {
                    System.Threading.Thread.Sleep(100);
                }
            };
            queueWorker.RunWorkerCompleted += (s, e) => {
                if (!Application.Current.Dispatcher.CheckAccess()) {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
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