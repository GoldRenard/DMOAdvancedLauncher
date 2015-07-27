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
// along with this program. If not, see<http://www.gnu.org/licenses/> .
// ======================================================================

using System;
using System.Threading.Tasks;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.UI.Windows;
using MahApps.Metro.Controls.Dialogs;
using Ninject;

namespace AdvancedLauncher.UI.Extension {

    internal static class DialogsHelper {

        /// <summary> Error MessageBox </summary>
        /// <param name="text">Content of error</param>
        public static void ShowErrorDialog(string text) {
            ILanguageManager LanguageManager = App.Kernel.Get<ILanguageManager>();
            ShowMessageDialog(LanguageManager.Model.Error, text);
        }

        /// <summary>
        /// Shows Metro MessageBox Dialog
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        public static void ShowMessageDialog(string title, string message) {
            if (MainWindow.Instance.Dispatcher != null && !MainWindow.Instance.Dispatcher.CheckAccess()) {
                MainWindow.Instance.Dispatcher.BeginInvoke(new Func<string, string, bool>((t, m) => {
                    ShowMessageDialog(t, m);
                    return true;
                }), title, message);
                return;
            }
            MainWindow.Instance.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, new MetroDialogSettings() {
                AffirmativeButtonText = "OK",
                ColorScheme = MetroDialogColorScheme.Accented
            });
        }

        /// <summary> Error MessageBox Async </summary>
        /// <param name="text">Content of error</param>
        /// <returns>Dummy True to able wait the return</returns>
        public async static Task<bool> ShowErrorDialogAsync(string text) {
            ILanguageManager LanguageManager = App.Kernel.Get<ILanguageManager>();
            return await ShowMessageDialogAsync(LanguageManager.Model.Error, text);
        }

        /// <summary>
        /// Shows Metro MessageBox Dialog Async
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <returns>True if Yes clicked</returns>
        public async static Task<bool> ShowMessageDialogAsync(string title, string message) {
            if (MainWindow.Instance.Dispatcher != null && !MainWindow.Instance.Dispatcher.CheckAccess()) {
                return await MainWindow.Instance.Dispatcher.Invoke<Task<bool>>(new Func<Task<bool>>(async () => {
                    return await ShowMessageDialogAsync(title, message);
                }));
            }
            await MainWindow.Instance.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, new MetroDialogSettings() {
                AffirmativeButtonText = "OK",
                ColorScheme = MetroDialogColorScheme.Accented
            });
            return true;
        }

        /// <summary>
        /// Shows Metro MessageBox Dialog with Yes/No buttons
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <returns>True if yes clicked</returns>
        public async static Task<bool> ShowYesNoDialog(string title, string message) {
            ILanguageManager LanguageManager = App.Kernel.Get<ILanguageManager>();
            if (MainWindow.Instance.Dispatcher != null && !MainWindow.Instance.Dispatcher.CheckAccess()) {
                return await MainWindow.Instance.Dispatcher.Invoke<Task<bool>>(new Func<Task<bool>>(async () => {
                    return await ShowYesNoDialog(title, message);
                }));
            }
            MessageDialogResult result = await MainWindow.Instance.ShowMessageAsync(title, message,
                MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() {
                    AffirmativeButtonText = LanguageManager.Model.Yes,
                    NegativeButtonText = LanguageManager.Model.No,
                    ColorScheme = MetroDialogColorScheme.Accented
                });
            return result == MessageDialogResult.Affirmative;
        }
    }
}