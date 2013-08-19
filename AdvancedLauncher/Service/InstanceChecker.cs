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
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace AdvancedLauncher.Service
{
    class InstanceChecker
    {
        static Mutex mutex;
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        public static bool AlreadyRunning(string mutex_name)
        {
            long runningId = 50000;
            bool InstanceRunning = false;

            Process proc = Process.GetCurrentProcess();
            Process[] runningProcesses = Process.GetProcesses();

            foreach (Process p in runningProcesses)
            {
                if (p.Id != proc.Id)
                {
                    bool Created = false;
                    mutex = new Mutex(true, mutex_name + p.Id.ToString(), out Created);
                    if (!Created)
                    {
                        InstanceRunning = true;
                        runningId = p.Id;
                        break;
                    }
                    else
                        mutex.ReleaseMutex();
                }
            }

            if (!InstanceRunning)
            {
                mutex = new Mutex(true, mutex_name + proc.Id.ToString());
                mutex.ReleaseMutex();
            }
            else
            {
                IntPtr hWnd = Process.GetProcessById((int)runningId).MainWindowHandle;
                if (IsIconic(hWnd))
                    ShowWindowAsync(hWnd, 9);
                SetForegroundWindow(hWnd);
            }

            return InstanceRunning;
        }
    }
}
