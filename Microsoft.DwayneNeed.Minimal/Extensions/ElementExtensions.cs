using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.DwayneNeed.Extensions {

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