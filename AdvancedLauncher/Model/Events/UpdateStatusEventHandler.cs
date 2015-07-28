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

namespace AdvancedLauncher.Model.Events {

    public delegate void UpdateStatusEventHandler(object sender, UpdateStatusEventEventArgs e);

    public class UpdateStatusEventEventArgs : EventArgs {

        public enum Stage {
            DOWNLOADING,
            EXTRACTING,
            INSTALLING
        }

        public Stage UpdateStage {
            get;
            private set;
        }

        public int CurrentPatch {
            get;
            private set;
        }

        public int MaxPatch {
            get;
            private set;
        }

        public double Progress {
            get;
            private set;
        }

        public double MaxProgress {
            get;
            private set;
        }

        public double SummaryProgress {
            get;
            private set;
        }

        public double SummaryMaxProgress {
            get;
            private set;
        }

        public UpdateStatusEventEventArgs(Stage UpdateStage, int CurrentPatch, int MaxPatch, double Progress, double MaxProgress, double SummaryProgress, double SummaryMaxProgress) {
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