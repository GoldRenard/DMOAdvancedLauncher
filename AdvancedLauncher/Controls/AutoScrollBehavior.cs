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