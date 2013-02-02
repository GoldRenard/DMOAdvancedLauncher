// ======================================================================
// GLOBAL DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
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

    /// -------------------------------------------------------------------------------------------------
    /// <summary> Application Running Helper. </summary>
    /// -------------------------------------------------------------------------------------------------
public static class ApplicationLauncher
{

    [DllImport("user32.dll")]
    private static extern
        bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern
        bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern
        bool IsIconic(IntPtr hWnd);

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
    /// <summary> Execute process with AppLocale if it exists in system and allowed in SettingsProvider
    ///           or execute it directly </summary>
    /// <param name="program">Path to program</param>
    /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
    /// -------------------------------------------------------------------------------------------------
    public static bool Execute(string program)
    {
        return Execute(program, string.Empty);
    }

    /// -------------------------------------------------------------------------------------------------
    /// <summary> Execute process with AppLocale if it exists in system and allowed in SettingsProvider
    ///           or execute it directly </summary>
    /// <param name="program">Path to program</param>
    /// <param name="args">Arguments</param>
    /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
    /// -------------------------------------------------------------------------------------------------
    public static bool Execute(string program, string args)
    {
        if (File.Exists(program))
        {
            if (!executeApplocale(program, args))
                StartProc(program, args);
            return true;
        }
        Utils.MSG_ERROR(LanguageProvider.strings.APPLAUNCHER_CANT_EXECURE + ": " + program);
        return false;
    }

    /// -------------------------------------------------------------------------------------------------
    /// <summary> Execute process </summary>
    /// <param name="program">Path to program</param>
    /// <param name="commandline">Command line</param>
    /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
    /// -------------------------------------------------------------------------------------------------
    private static bool StartProc(string program, string commandline)
    {
        try
        {
            Process proc = new Process();
            proc.StartInfo.FileName = program;
            proc.StartInfo.WorkingDirectory = SettingsProvider.GAME_PATH();
            proc.StartInfo.Arguments = commandline;
            proc.Start();
        }
        catch { return false; }
        return true;
    }

    /// -------------------------------------------------------------------------------------------------
    /// <summary> Execute process with AppLocale if it exists in system and allowed in SettingsProvider </summary>
    /// <param name="program">Path to program</param>
    /// <param name="args">Command line</param>
    /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
    /// -------------------------------------------------------------------------------------------------
    static bool executeApplocale(string program, string args)
    {

        if (!SettingsProvider.USE_APPLOC)
            return false;
        string apploc_dir = Environment.GetEnvironmentVariable("windir") + "\\apppatch\\AppLoc.exe";

        if (!File.Exists(apploc_dir))
        {
            if (MessageBoxResult.Yes == MessageBox.Show(LanguageProvider.strings.APPLAUNCHER_APPLOCALE_NOT_FOUND_MSG, LanguageProvider.strings.APPLAUNCHER_APPLOCALE_NOT_FOUND_TITLE, MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                Utils.OpenSite("http://www.microsoft.com/ru-ru/download/details.aspx?id=2043");
                return true;
            }
            else
                return false;
        }

        if (args != "")
            StartProc(apploc_dir, "\"" + program + "\" \"" + args + "\" \"/L0412\"");
        else
            StartProc(apploc_dir, "\"" + program + "\" \"/L0412\"");
        return true;
    }
}

