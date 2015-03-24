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
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace AdvancedLauncher.Controls {

    public class AutoScrollBehavior : Behavior<ScrollViewer> {
        private ScrollViewer Target = null;
        private double Height = 0.0d;
        private const double DELTA_MULTIPLY = 2;

        protected override void OnAttached() {
            base.OnAttached();
            this.Target = base.AssociatedObject;
            this.Target.LayoutUpdated += new EventHandler(OnLayoutUpdated);
        }

        private void OnLayoutUpdated(object sender, EventArgs e) {
            if (this.Target.ExtentHeight != Height
                && this.Height - this.Target.ContentVerticalOffset <= this.Target.ViewportHeight * DELTA_MULTIPLY) {
                this.Target.ScrollToVerticalOffset(this.Target.ExtentHeight);
                this.Height = this.Target.ExtentHeight;
            }
        }

        protected override void OnDetaching() {
            base.OnDetaching();
            if (this.Target != null) {
                this.Target.LayoutUpdated -= new EventHandler(OnLayoutUpdated);
            }
        }
    }
}