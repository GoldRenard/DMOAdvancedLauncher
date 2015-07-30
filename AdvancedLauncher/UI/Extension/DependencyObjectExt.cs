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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AdvancedLauncher.UI.Extension {

    public static class DependencyObjectExt {

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

        public static T FindAncestor<T>(this DependencyObject obj) where T : DependencyObject {
            while (obj != null) {
                T objTest = obj as T;
                if (objTest != null)
                    return objTest;
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }
    }
}