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
using System.Reflection;

namespace AdvancedLauncher.Tools {

    internal static class ExceptionHandler {
        private const string EXCEPTION_HANDLER_TYPE = "IntelleSoft.BugTrap.ExceptionHandler";

        private const string X86_LIBRARY = "BugTrapN.dll";

        private const string X64_LIBRARY = "BugTrapN-x64.dll";

        [Flags]
        public enum FlagsType {
            None = 0,
            DetailedMode = 1,
            EditMail = 2,
            AttachReport = 4,
            ListProcesses = 8,
            ShowAdvancedUI = 16,
            ScreenCapture = 32,
            NativeInfo = 64,
            InterceptSUEF = 128,
            DescribeError = 256,
            RestartApp = 512
        }

        [Flags]
        public enum MinidumpType {
            NoDump = -1,
            Normal = 0,
            WithDataSegs = 1,
            WithFullMemory = 2,
            WithHandleData = 4,
            FilterMemory = 8,
            ScanMemory = 16,
            WithUnloadedModules = 32,
            WithIndirectlyReferencedMemory = 64,
            FilterModulePaths = 128,
            WithProcessThreadData = 256,
            WithPrivateReadWriteMemory = 512,
            WithoutOptionalData = 1024
        }

        public enum ReportFormatType {
            Xml = 1,
            Text = 2
        }

        public enum ActivityType {
            ShowUI = 1,
            SaveReport = 2,
            MailReport = 3,
            SendReport = 4
        }

        private static Type _BaseType = null;

        private static Type BaseType {
            get {
                if (_BaseType == null) {
                    try {
                        Assembly bugTrap = Assembly.LoadFrom((IntPtr.Size == 4) ? X86_LIBRARY : X64_LIBRARY);
                        _BaseType = bugTrap.GetType(EXCEPTION_HANDLER_TYPE);
                    } catch (Exception) {
                        _BaseType = null;
                    }
                }
                return _BaseType;
            }
        }

        public static int HttpPort {
            get {
                return (int)GetFieldValue("HttpPort");
            }
            set {
                SetFieldValue("Activity", (int)value);
            }
        }

        public static ActivityType Activity {
            get {
                return (ActivityType)((int)GetPropertyValue("Activity"));
            }
            set {
                SetPropertyValue("Activity", (int)value);
            }
        }

        public static string AppName {
            get {
                return (string)GetPropertyValue("AppName");
            }
            set {
                SetPropertyValue("AppName", value);
            }
        }

        public static string AppVersion {
            get {
                return (string)GetPropertyValue("AppVersion");
            }
            set {
                SetPropertyValue("AppVersion", value);
            }
        }

        public static MinidumpType DumpType {
            get {
                return (MinidumpType)((int)GetPropertyValue("DumpType"));
            }
            set {
                SetPropertyValue("DumpType", (int)value);
            }
        }

        public static FlagsType Flags {
            get {
                return (FlagsType)((int)GetPropertyValue("Flags"));
            }
            set {
                SetPropertyValue("Flags", (int)value);
            }
        }

        public static string MailProfile {
            get {
                return (string)GetPropertyValue("MailProfile");
            }
        }

        public static string NotificationEMail {
            get {
                return (string)GetPropertyValue("NotificationEMail");
            }
            set {
                SetPropertyValue("NotificationEMail", value);
            }
        }

        public static string ReportFilePath {
            get {
                return (string)GetPropertyValue("ReportFilePath");
            }
            set {
                SetPropertyValue("ReportFilePath", value);
            }
        }

        public static ReportFormatType ReportFormat {
            get {
                return (ReportFormatType)((int)GetPropertyValue("ReportFormat"));
            }
            set {
                SetPropertyValue("ReportFormat", (int)value);
            }
        }

        public static string SupportEMail {
            get {
                return (string)GetPropertyValue("SupportEMail");
            }
            set {
                SetPropertyValue("SupportEMail", value);
            }
        }

        public static string SupportHost {
            get {
                return (string)GetPropertyValue("SupportHost");
            }
            set {
                SetPropertyValue("SupportHost", value);
            }
        }

        public static short SupportPort {
            get {
                return (short)GetPropertyValue("SupportPort");
            }
            set {
                SetPropertyValue("SupportPort", value);
            }
        }

        public static string SupportURL {
            get {
                return (string)GetPropertyValue("SupportURL");
            }
            set {
                SetPropertyValue("SupportURL", value);
            }
        }

        public static string UserMessage {
            get {
                return (string)GetPropertyValue("UserMessage");
            }
            set {
                SetPropertyValue("UserMessage", value);
            }
        }

        public static bool IsAvailable {
            get {
                return BaseType != null;
            }
        }

        private static object GetPropertyValue(string propertyName) {
            return BaseType.GetProperty(propertyName).GetValue(null);
        }

        private static void SetPropertyValue(string propertyName, object PropertyValue) {
            BaseType.GetProperty(propertyName).SetValue(null, PropertyValue);
        }

        private static object GetFieldValue(string propertyName) {
            return BaseType.GetField(propertyName).GetValue(null);
        }

        private static void SetFieldValue(string propertyName, object PropertyValue) {
            BaseType.GetField(propertyName).SetValue(null, PropertyValue);
        }

        /*public static event UnhandledExceptionDelegate AfterUnhandledException;
        public static event UnhandledExceptionDelegate BeforeUnhandledException;

        public static void ExportRegKey(string fileName, string key);
        public static void HandleException(Exception exception);
        public static void InstallHandler();
        public static void InstallSehFilter();
        public static void MailSnapshot();
        public static void MailSnapshot(Exception exception);
        public static void ReadVersionInfo();
        public static void ReadVersionInfo(AssemblyName assemblyName);
        public static void ReadVersionInfo(Assembly assembly);
        public static void SaveSnapshot(string fileName);
        public static void SaveSnapshot(Exception exception, string fileName);
        public static void SendSnapshot();
        public static void SendSnapshot(Exception exception);
        public static void SetMailProfile(string profile, string password);
        public static void UninstallHandler();
        public static void UninstallSehFilter();
        public string GetDialogMessage(DialogMessageType dlgMsg);
        public void SetDialogMessage(DialogMessageType dlgMsg, string value);*/
    }
}