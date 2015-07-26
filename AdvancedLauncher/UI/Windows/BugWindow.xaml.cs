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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using log4net;
using log4net.Appender;
using MahApps.Metro.Controls;

namespace AdvancedLauncher.UI.Windows {

    public partial class BugWindow : MetroWindow {
        private static readonly log4net.ILog LOGGER = LogManager.GetLogger(typeof(BugWindow));

        private Exception _Exception;

        public BugWindow(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            InitializeComponent();
            LoadIcon();
            LOGGER.Error("Exception detected", e.Exception);
            _Exception = e.Exception;
            ExceptionText.Text = _Exception.Message;
            StackTrace.Text = _Exception.ToString();

            if (string.IsNullOrEmpty(GetLogFileName())) {
                LogFile.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void LoadIcon() {
            using (var icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/app_icon.ico")).Stream, 64, 64)) {
                using (var bitmap = icon.ToBitmap()) {
                    MemoryStream memoryStream = new MemoryStream();
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    PngBitmapDecoder pbd = new PngBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                    ErrorIcon.Source = pbd.Frames[0];
                }
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void OnSubmitClick(object sender, RoutedEventArgs e) {
            try {
                Process.Start("mailto:goldrenard@gmail.com?subject=DMO%20Advanced%20Launcher%20has%20crashed&body=" + GenerateReport(_Exception));
            } catch {
            }
            this.Close();
        }

        private string GenerateReport(Exception ex) {
            StringBuilder strB = new StringBuilder();
            strB.Append("Hello! I've just got crash of AdvancedLauncher and provide next info about this crash:%0D%0A");
            strB.Append("=====================================================================%0D%0A");
            strB.Append(string.Format("Application version: {0} %0D%0A", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            strB.Append(string.Format("Operating System: {0} %0D%0A", OSVersion()));
            strB.Append(string.Format("Exception Reason: {0} %0D%0A", ex.Message));
            strB.Append(string.Format("Exception StackTrace:%0D%0A{0}%0D%0A", ex.ToString().Replace(System.Environment.NewLine, "%0D%0A")));
            strB.Append("=====================================================================%0D%0A%0D%0A");
            strB.Append("Detailed information (what you do to get such a crash):%0D%0A");
            return strB.ToString();
        }

        private string OSVersion() {
            var name = (from x in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<ManagementObject>()
                        select x.GetPropertyValue("Caption")).First();
            if (name != null) {
                return name.ToString() + (System.Environment.Is64BitOperatingSystem ? " 64-Bit" : " 32-Bit");
            }
            return "Unknown";
        }

        public String GetLogFileName() {
            String filename = null;
            IAppender[] appenders = LOGGER.Logger.Repository.GetAppenders();
            // Check each appender this logger has
            foreach (IAppender appender in appenders) {
                Type t = appender.GetType();
                // Get the file name from the first FileAppender found and return
                if (t.Equals(typeof(FileAppender)) || t.Equals(typeof(RollingFileAppender))) {
                    filename = ((FileAppender)appender).File;
                    break;
                }
            }
            return filename;
        }

        private void OnLogRequested(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrEmpty(GetLogFileName())) {
                System.Diagnostics.Process.Start("explorer.exe", @"/select, " + GetLogFileName());
            }
        }
    }
}