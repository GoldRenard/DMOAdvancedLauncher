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
// ======================================================================s

using System;
using System.Windows;
using DMOLibrary.DMOFileSystem;

namespace AdvancedLauncher
{
    public partial class App : Application
    {

        public App()
        {
            DMOLibrary.AssemblyResolver.HandleUnresovledAssemblies();
            if (ApplicationLauncher.AlreadyRunning())
                Application.Current.Shutdown();
            else
            {
                SettingsProvider.LoadSettings();

                DMOFileSystem res_fs = new DMOFileSystem(32, SettingsProvider.APP_PATH + SettingsProvider.RES_HF_FILE, SettingsProvider.APP_PATH + SettingsProvider.RES_PF_FILE);
                res_fs.WriteDirectory(SettingsProvider.APP_PATH + SettingsProvider.RES_IMPORT_DIR, true);

                MainWindow mw = new MainWindow();
                mw.Show();
            }
        }
    }
}
