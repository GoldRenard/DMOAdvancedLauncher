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
using System.Windows;
using DMOLibrary.Profiles;
using DMOLibrary.Profiles.Aeria;
using DMOLibrary.Profiles.Joymax;
using DMOLibrary.Profiles.Korea;
using DMOLibrary.DMOFileSystem;

namespace AdvancedLauncher
{
    public partial class App : Application
    {
        public static char SubVersion = 'c';
        public static DMOProfile DMOProfile;
        Window WpfBugWindow = new Window()
        {
            AllowsTransparency = true,
            Background = System.Windows.Media.Brushes.Transparent,
            WindowStyle = WindowStyle.None,
            Top = 0,
            Left = 0,
            Width = 1,
            Height = 1,
            ShowInTaskbar = false
        };

        public App()
        {
            //it's a cute fix for "SplashScreen & Modal Dialogs" WPF's bug ;)
            WpfBugWindow.Show();
#if DEBUG
            Utils.SetDebug("Debug.log");
#endif
            SettingsProvider.LoadSettings();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                string type = e.Args.Length > 0 ? e.Args[0].ToLower() : "dmojoymax";
                string profile = e.Args.Length > 1 ? e.Args[1] : string.Empty;

                //Check for bad chars in profile name (cuz it will be used for filename)
                if (profile.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
                {
                    Application.Current.Shutdown();
                    return;
                }
#if DEBUG
                Utils.WriteDebug("Type=" + type + "; Profile=" + profile);
#endif
                string mutex_type = type;
                if (mutex_type == "dmokoreaimbc")
                    mutex_type = "dmokorea";
                if (!ApplicationHelper.AlreadyRunningMutex("327D5E06-1C06-47EC-B391-" + mutex_type))
                {
#if DEBUG
                    Utils.WriteDebug("Creating new instance for type=" + type);
#endif
                    if (type == "dmojoymax")
                        App.DMOProfile = new DMOJoymax(this.Dispatcher, profile);
                    else if (type == "dmoaeria")
                        App.DMOProfile = new DMOAeria(this.Dispatcher, profile);
                    else if (type == "dmokorea")
                        App.DMOProfile = new DMOKorea(this.Dispatcher, profile);
                    else if (type == "dmokoreaimbc")
                        App.DMOProfile = new DMOKoreaIMBC(this.Dispatcher, profile);
                    else
                    {
                        Application.Current.Shutdown();
                        return;
                    }

                    DMOFileSystem res_fs = new DMOFileSystem(32, SettingsProvider.APP_PATH + SettingsProvider.RES_HF_FILE, SettingsProvider.APP_PATH + SettingsProvider.RES_PF_FILE);
                    res_fs.WriteDirectory(SettingsProvider.APP_PATH + SettingsProvider.RES_IMPORT_DIR, true);
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    WpfBugWindow.Close();
                }
                else
                    Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
#if DEBUG
                Utils.WriteDebug(ex.Message + " " + ex.StackTrace);
#endif
                MessageBox.Show(ex.Message + " " + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            DateTime t = DateTime.Now;
            MiniDump.MiniDumpToFile(string.Format("Crash_{0:00}_{1:00}_{2:00}{3:00}{4:0000}.dmp", t.Hour, t.Minute, t.Day, t.Month, t.Year));
            //e.Handled = true;
        }
    }
}
