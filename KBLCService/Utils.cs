// ======================================================================
// DMO KEYBOARD LAYOUT CHANGER SERVICE
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Microsoft.Win32;

namespace KBLCService {

    /// <summary>
    /// Utility functions
    /// </summary>
    internal static class Utils {

        /// <summary>
        /// Проверка, запущен ли экземпляр этого приложения
        /// </summary>
        /// <param name="mutex_name"></param>
        /// <returns>True если приложение уже запущено</returns>
        public static Mutex CreateMutex() {
            bool created = false;
            Mutex applicationMutex = new Mutex(true, "D684AD4E-7C2F-4840-82BD-CF8A419094ED", out created);
            return created ? applicationMutex : null;
        }

        /// <summary>
        /// Получение значение параметра реестра
        /// </summary>
        /// <param name="fullPath">Путь</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        /// <returns>Значение параметра</returns>
        public static object GetRegistryValue(string fullPath, object defaultValue) {
            string keyName = Path.GetDirectoryName(fullPath);
            string valueName = Path.GetFileName(fullPath);
            return Registry.GetValue(keyName, valueName, defaultValue);
        }

        /// <summary>
        /// Проверка, указан ли параметр в командной строке
        /// </summary>
        /// <param name="args">Массив аргументов командной строки</param>
        /// <param name="param">Параметр</param>
        /// <returns>True если указан</returns>
        public static bool HasParameter(string[] args, string param) {
            return args.Contains(param);
        }

        /// <summary>
        /// Закрытие приложения
        /// </summary>
        public static void CloseApp() {
            if (Application.Current != null) {
                if (Application.Current.Dispatcher.CheckAccess()) {
                    Application.Current.Shutdown();
                } else {
                    Application.Current.Dispatcher.BeginInvoke(((Action)(() => Application.Current.Shutdown())));
                }
            }
        }

        /// <summary>
        /// Проверка, 8ка ли или выше
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows8OrNewer() {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT && (os.Version.Major > 6 || (os.Version.Major == 6 && os.Version.Minor >= 2));
        }
    }
}