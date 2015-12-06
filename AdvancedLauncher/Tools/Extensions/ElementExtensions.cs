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
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace AdvancedLauncher.Tools.Extensions {

    public static class ElementExtensions {

        public static Point TransformElementToElement(this UIElement @this, Point pt, UIElement target) {
            // Find the HwndSource for this element and use it to transform
            // the point up into screen coordinates.
            HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(@this);
            pt = hwndSource.TransformDescendantToClient(pt, @this);
            pt = hwndSource.TransformClientToScreen(pt);

            // Find the HwndSource for the target element and use it to
            // transform the rectangle from screen coordinates down to the
            // target elemnent.
            HwndSource targetHwndSource = (HwndSource)PresentationSource.FromVisual(target);
            pt = targetHwndSource.TransformScreenToClient(pt);
            pt = targetHwndSource.TransformClientToDescendant(pt, target);

            return pt;
        }

        public static void DisposeSubTree(this UIElement @this) {
            int childrenCount = VisualTreeHelper.GetChildrenCount(@this);
            for (int iChild = 0; iChild < childrenCount; iChild++) {
                UIElement child = VisualTreeHelper.GetChild(@this, iChild) as UIElement;
                if (child != null) {
                    if (child is IDisposable) {
                        ((IDisposable)child).Dispose();

                        // Don't descend into the visual tree of an element we
                        // just disposed.  We rely on the element to properly
                        // dispose its content.
                    } else {
                        DisposeSubTree(child);
                    }
                }
            }
        }
    }
}