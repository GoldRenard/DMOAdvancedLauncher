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

using System.Windows;
using System.Windows.Media;

namespace AdvancedLauncher.Tools.Extensions {

    public static class PresentationSourceExtensions {

        /// <summary>
        ///     Convert a point from "client" coordinate space of a window into
        ///     the coordinate space of the specified element of the same window.
        /// </summary>
        public static Point TransformClientToDescendant(this PresentationSource presentationSource, Point point, Visual descendant) {
            Point pt = TransformClientToRoot(presentationSource, point);
            return presentationSource.RootVisual.TransformToDescendant(descendant).Transform(pt);
        }

        /// <summary>
        ///     Convert a point from the coordinate space of the specified
        ///     element into the "client" coordinate space of the window.
        /// </summary>
        public static Point TransformDescendantToClient(this PresentationSource presentationSource, Point point, Visual descendant) {
            Point pt = descendant.TransformToAncestor(presentationSource.RootVisual).Transform(point);
            return TransformRootToClient(presentationSource, pt);
        }

        /// <summary>
        ///     Convert a point from "client" coordinate space of a window into
        ///     the coordinate space of the root element of the same window.
        /// </summary>
        public static Point TransformClientToRoot(this PresentationSource presentationSource, Point pt) {
            // Convert from pixels into DIPs.
            pt = presentationSource.CompositionTarget.TransformFromDevice.Transform(pt);

            // We need to include the root element's transform.
            pt = ApplyVisualTransform(presentationSource.RootVisual, pt, true);

            return pt;
        }

        /// <summary>
        ///     Convert a point from the coordinate space of the root element
        ///     into the "client" coordinate space of the same window.
        /// </summary>
        public static Point TransformRootToClient(this PresentationSource presentationSource, Point pt) {
            // We need to include the root element's transform.
            pt = ApplyVisualTransform(presentationSource.RootVisual, pt, false);

            // Convert from DIPs into pixels.
            pt = presentationSource.CompositionTarget.TransformToDevice.Transform(pt);

            return pt;
        }

        /// <summary>
        ///     Convert a point from "above" the coordinate space of a
        ///     visual into the the coordinate space "below" the visual.
        /// </summary>
        private static Point ApplyVisualTransform(Visual v, Point pt, bool inverse) {
            Matrix m = GetVisualTransform(v);

            if (inverse) {
                m.Invert();
            }

            return m.Transform(pt);
        }

        /// <summary>
        ///     Gets the matrix that will convert a point
        ///     from "above" the coordinate space of a visual
        ///     into the the coordinate space "below" the visual.
        /// </summary>
        private static Matrix GetVisualTransform(Visual v) {
            Matrix m = Matrix.Identity;

            // A visual can currently have two properties that affect
            // its coordinate space:
            //    1) Transform - any matrix
            //    2) Offset - a simpification for just a 2D offset.
            Transform transform = VisualTreeHelper.GetTransform(v);
            if (transform != null) {
                Matrix cm = transform.Value;
                m = Matrix.Multiply(m, cm);
            }

            Vector offset = VisualTreeHelper.GetOffset(v);
            m.Translate(offset.X, offset.Y);

            return m;
        }
    }
}