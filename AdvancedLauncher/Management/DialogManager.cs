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
using System.Security.Permissions;
using System.Threading.Tasks;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Tools;
using AdvancedLauncher.UI.Windows;
using MahApps.Metro.Controls.Dialogs;
using Ninject;

namespace AdvancedLauncher.Management {

    // TODO Figure out what exactly permissions required by MahApps DialogManager to work properly.
    // In partual trust environment it shows only transparent overlay, but not dialog itself.
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    public class DialogManager : CrossDomainObject, IDialogManager {

        [Inject]
        public ILanguageManager LanguageManager {
            get;
            set;
        }

        public void Initialize() {
            // nothing to do here
        }

        /// <summary> Error MessageBox </summary>
        /// <param name="text">Content of error</param>
        public void ShowErrorDialog(string text) {
            ShowMessageDialog(LanguageManager.Model.Error, text);
        }

        /// <summary>
        /// Shows Metro MessageBox Dialog
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        public void ShowMessageDialog(string title, string message) {
            MainWindow MainWindow = App.Kernel.Get<MainWindow>();
            if (MainWindow.Dispatcher != null && !MainWindow.Dispatcher.CheckAccess()) {
                MainWindow.Dispatcher.BeginInvoke(new Func<string, string, bool>((t, m) => {
                    ShowMessageDialog(t, m);
                    return true;
                }), title, message);
                return;
            }
            MainWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, new MetroDialogSettings() {
                AffirmativeButtonText = "OK",
                ColorScheme = MetroDialogColorScheme.Accented
            });
        }

        /// <summary> Error MessageBox Async </summary>
        /// <param name="text">Content of error</param>
        /// <returns>Dummy True to able wait the return</returns>
        private async Task<bool> ShowErrorDialogAsyncInternal(string text) {
            return await ShowMessageDialogAsyncInternal(LanguageManager.Model.Error, text);
        }

        /// <summary>
        /// Shows Metro MessageBox Dialog Async
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <returns>True if Yes clicked</returns>
        private async Task<bool> ShowMessageDialogAsyncInternal(string title, string message) {
            MainWindow MainWindow = App.Kernel.Get<MainWindow>();
            if (MainWindow.Dispatcher != null && !MainWindow.Dispatcher.CheckAccess()) {
                return await MainWindow.Dispatcher.Invoke<Task<bool>>(new Func<Task<bool>>(async () => {
                    return await ShowMessageDialogAsyncInternal(title, message);
                }));
            }
            await MainWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, new MetroDialogSettings() {
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
        private async Task<bool> ShowYesNoDialogInternal(string title, string message) {
            MainWindow MainWindow = App.Kernel.Get<MainWindow>();
            if (MainWindow.Dispatcher != null && !MainWindow.Dispatcher.CheckAccess()) {
                return await MainWindow.Dispatcher.Invoke<Task<bool>>(new Func<Task<bool>>(async () => {
                    return await ShowYesNoDialogInternal(title, message);
                }));
            }
            MessageDialogResult result = await MainWindow.ShowMessageAsync(title, message,
                MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() {
                    AffirmativeButtonText = LanguageManager.Model.Yes,
                    NegativeButtonText = LanguageManager.Model.No,
                    ColorScheme = MetroDialogColorScheme.Accented
                });
            return result == MessageDialogResult.Affirmative;
        }

        public RemoteTask<bool> ShowErrorDialogAsync(string text) {
            return new RemoteTask<bool>(ShowErrorDialogAsyncInternal(text));
        }

        public RemoteTask<bool> ShowMessageDialogAsync(string title, string message) {
            return new RemoteTask<bool>(ShowMessageDialogAsyncInternal(title, message));
        }

        public RemoteTask<bool> ShowYesNoDialog(string title, string message) {
            return new RemoteTask<bool>(ShowYesNoDialogInternal(title, message));
        }
    }
}