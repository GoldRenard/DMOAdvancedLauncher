using System.Windows;
using System.Windows.Controls;

namespace AdvancedLauncher.Service {

    public static class RemoveChildHelper {

        public static void RemoveChild(this DependencyObject parent, UIElement child) {
            var panel = parent as Panel;
            if (panel != null) {
                panel.Children.Remove(child);
                return;
            }

            var decorator = parent as Decorator;
            if (decorator != null) {
                if (decorator.Child == child) {
                    decorator.Child = null;
                }
                return;
            }

            var contentPresenter = parent as ContentPresenter;
            if (contentPresenter != null) {
                if (contentPresenter.Content == child) {
                    contentPresenter.Content = null;
                }
                return;
            }

            var contentControl = parent as ContentControl;
            if (contentControl != null) {
                if (contentControl.Content == child) {
                    contentControl.Content = null;
                }
                return;
            }
        }
    }
}