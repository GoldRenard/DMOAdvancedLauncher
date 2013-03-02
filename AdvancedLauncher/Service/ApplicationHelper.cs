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
using AdvancedLauncher;
using System.Threading;

    /// -------------------------------------------------------------------------------------------------
    /// <summary> Application Running Helper. </summary>
    /// -------------------------------------------------------------------------------------------------
public static class ApplicationHelper
{
    static Mutex mutex;
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    /// -------------------------------------------------------------------------------------------------
    /// <summary> check if current process already running. if running, set focus to existing process and
    ///           returns <see langword="true"/> otherwise returns <see langword="false"/>. </summary>
    /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
    /// -------------------------------------------------------------------------------------------------
    public static bool AlreadyRunning()
    {
        Process proc = Process.GetCurrentProcess();
        var arrProcesses = Process.GetProcessesByName(proc.ProcessName);
        if (arrProcesses.Length > 1)
        {
            for (var i = 0; i < arrProcesses.Length; i++)
                if (arrProcesses[i].Id != proc.Id)
                {
                    // get the window handle
                    IntPtr hWnd = arrProcesses[i].MainWindowHandle;
                    // if iconic, we need to restore the window
                    if (IsIconic(hWnd))
                        ShowWindowAsync(hWnd, 9);
                    // bring it to the foreground
                    SetForegroundWindow(hWnd);
                    break;
                }
            return true;
        }

        return false;
    }

    /// -------------------------------------------------------------------------------------------------
    /// <summary> check if current process's mutex already created. if created, set focus to existing process and
    ///           returns <see langword="true"/> otherwise returns <see langword="false"/>. </summary>
    /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
    /// -------------------------------------------------------------------------------------------------
    public static bool AlreadyRunningMutex(string mutex_name)
    {
        #if DEBUG
        Utils.WriteDebug("Checking mutex: " + mutex_name);
        #endif

        long runningId = 50000;
        bool InstanceRunning = false;

        Process proc = Process.GetCurrentProcess();
        Process[] runningProcesses = Process.GetProcesses();

        foreach (Process p in runningProcesses)
        {
            if (p.Id != proc.Id)
            {
                #if DEBUG
                Utils.WriteDebug("Checking mutex for process: " + p.Id);
                #endif

                bool Created = false;
                mutex = new Mutex(true, mutex_name + p.Id.ToString(), out Created);
                if (!Created)
                {
                    #if DEBUG
                    Utils.WriteDebug("Instance found with PID:" + p.Id);
                    #endif

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
            #if DEBUG
            Utils.WriteDebug("Instance not found, creating new mutex...");
            #endif

            mutex = new Mutex(true, mutex_name + proc.Id.ToString());
            mutex.ReleaseMutex();
        }
        else
        {
            #if DEBUG
            Utils.WriteDebug("Set runned instance to top...");
            #endif

            IntPtr hWnd = Process.GetProcessById((int)runningId).MainWindowHandle;
            if (IsIconic(hWnd))
                ShowWindowAsync(hWnd, 9);
            SetForegroundWindow(hWnd);
        }

        return InstanceRunning;
    }
}

