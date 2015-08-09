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

using AdvancedLauncher.SDK.Tools;

namespace AdvancedLauncher.SDK.Management {

    /// <summary>
    /// Dialog manager, allow to show different dialogs at UI.
    /// </summary>
    public interface IDialogManager : IManager {

        /// <summary>
        /// Shows error dialog
        /// </summary>
        /// <param name="text">Text of error</param>
        void ShowErrorDialog(string text);

        /// <summary>
        /// Shows usual message dialog
        /// </summary>
        /// <param name="title">Title of dialog</param>
        /// <param name="message">Message of dialog</param>
        void ShowMessageDialog(string title, string message);

        /// <summary>
        /// Asynchronously shows error dialog
        /// </summary>
        /// <param name="text">Text of error</param>
        /// <returns><see cref="RemoteTask{T}"/> instance.
        /// You can use await statement with <see cref="RemoteTaskExt.Wait{T}(RemoteTask{T})"/> call.
        /// </returns>
        RemoteTask<bool> ShowErrorDialogAsync(string text);

        /// <summary>
        /// Asynchronously shows usual message dialog
        /// </summary>
        /// <param name="title">Title of dialog</param>
        /// <param name="message">Message of dialog</param>
        /// <returns><see cref="RemoteTask{T}"/> instance.
        /// You can use await statement with <see cref="RemoteTaskExt.Wait{T}(RemoteTask{T})"/> call.
        /// </returns>
        RemoteTask<bool> ShowMessageDialogAsync(string title, string message);

        /// <summary>
        /// Asynchronously shows yes/no confirmation dialog
        /// </summary>
        /// <param name="title">Title of dialog</param>
        /// <param name="message">Message of dialog</param>
        /// <returns><see cref="RemoteTask{T}"/> instance.
        /// You can use await statement with <see cref="RemoteTaskExt.Wait{T}(RemoteTask{T})"/> call.
        /// </returns>
        RemoteTask<bool> ShowYesNoDialog(string title, string message);
    }
}