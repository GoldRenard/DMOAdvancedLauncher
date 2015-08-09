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
    /// Game update status changed event handler
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public delegate void UpdateStatusEventHandler(object sender, UpdateStatusEventArgs e);

    /// <summary>
    /// Game update status changed event args
    /// </summary>
    public class UpdateStatusEventArgs : BaseEventArgs {

        /// <summary>
        /// Update stage enumeration
        /// </summary>
        public enum Stage {

            /// <summary>Download update stage</summary>
            DOWNLOADING,

            /// <summary>Extracting update stage</summary>
            EXTRACTING,

            /// <summary>Installing update stage</summary>
            INSTALLING
        }

        /// <summary>
        /// Gets or sets update stage
        /// </summary>
        public Stage UpdateStage {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets current patch number
        /// </summary>
        public int CurrentPatch {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets max patch number
        /// </summary>
        public int MaxPatch {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets current progress
        /// </summary>
        public double Progress {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets max progress
        /// </summary>
        public double MaxProgress {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets summary progress
        /// </summary>
        public double SummaryProgress {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets summary max progress
        /// </summary>
        public double SummaryMaxProgress {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="UpdateStatusEventArgs"/> for specified: <see cref="Stage"/>, CurrentPatch, MaxPatch,
        /// Progress, MaxProgress, SummaryProgress and SummaryMaxProgress
        /// </summary>
        /// <param name="UpdateStage">Update stage</param>
        /// <param name="CurrentPatch">Current patch number</param>
        /// <param name="MaxPatch">Max patch number</param>
        /// <param name="Progress">Current progress</param>
        /// <param name="MaxProgress">Max progress</param>
        /// <param name="SummaryProgress">Summary progress</param>
        /// <param name="SummaryMaxProgress">Summary max progress</param>
        public UpdateStatusEventArgs(Stage UpdateStage, int CurrentPatch, int MaxPatch, double Progress, double MaxProgress, double SummaryProgress, double SummaryMaxProgress) {
            this.CurrentPatch = CurrentPatch;
            this.MaxPatch = MaxPatch;
            this.UpdateStage = UpdateStage;
            this.Progress = Progress;
            this.MaxProgress = MaxProgress;
            this.SummaryProgress = SummaryProgress;
            this.SummaryMaxProgress = SummaryMaxProgress;
        }
    }
}