using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace AdvancedLauncher.Controls {
    public class AutoScrollBehavior : Behavior<ScrollViewer> {
        private ScrollViewer _scrollViewer = null;
        private double _height = 0.0d;

        protected override void OnAttached() {
            base.OnAttached();

            this._scrollViewer = base.AssociatedObject;
            this._scrollViewer.LayoutUpdated += new EventHandler(_scrollViewer_LayoutUpdated);
        }

        private void _scrollViewer_LayoutUpdated(object sender, EventArgs e) {
            if (this._scrollViewer.ExtentHeight != _height) {
                this._scrollViewer.ScrollToVerticalOffset(this._scrollViewer.ExtentHeight);
                this._height = this._scrollViewer.ExtentHeight;
            }
        }

        protected override void OnDetaching() {
            base.OnDetaching();

            if (this._scrollViewer != null)
                this._scrollViewer.LayoutUpdated -= new EventHandler(_scrollViewer_LayoutUpdated);
        }
    }
}
