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

namespace AdvancedLauncher.SDK.Model.Events {

    /// <summary>
    /// WebProfile Download status changed event handler
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    public delegate void DownloadStatusChangedEventHandler(object sender, DownloadStatusEventArgs e);

    /// <summary>
    /// Download status code
    /// </summary>
    public enum DMODownloadStatusCode {
        GETTING_GUILD = 0,
        GETTING_TAMER = 1
    }

    /// <summary>
    /// Download status event arguments
    /// </summary>
    public class DownloadStatusEventArgs : BaseEventArgs {

        /// <summary>
        /// Download status code
        /// </summary>
        public DMODownloadStatusCode Code {
            get;
            private set;
        }

        /// <summary>
        /// Download informations (e.g. Guild Name or Tamer Name)
        /// </summary>
        public string Info {
            get;
            private set;
        }

        /// <summary>
        /// Download current progress
        /// </summary>
        public int Progress {
            get;
            private set;
        }

        /// <summary>
        /// Download max desired progress
        /// </summary>
        public int MaxProgress {
            get;
            private set;
        }

        public DownloadStatusEventArgs(DMODownloadStatusCode Code, string Info, int Progress, int MaxProgress) {
            this.Code = Code;
            this.Info = Info;
            this.Progress = Progress;
            this.MaxProgress = MaxProgress;
        }
    }
}