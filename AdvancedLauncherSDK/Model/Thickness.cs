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

namespace AdvancedLauncher.SDK.Tools {

    /// <summary>
    /// <see cref="System.Windows.Thickness"/> serializable wrapper
    /// </summary>
    [Serializable]
    public sealed class Thickness {

        /// <summary>
        /// Gets or sets the width, in pixels, of the left side of the bounding rectangle.
        /// </summary>
        public double Left {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width, in pixels, of the upper side of the bounding rectangle.
        /// </summary>
        public double Top {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width, in pixels, of the right side of the bounding rectangle.
        /// </summary>
        public double Right {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width, in pixels, of the lower side of the bounding rectangle.
        /// </summary>
        public double Bottom {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new default instance of <see cref="Thickness"/>
        /// </summary>
        public Thickness() : this(0) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Thickness"/> that has
        /// specific lengths applied to each side of the rectangle.
        /// </summary>
        /// <param name="uniformLength">The uniform length applied to all four sides of the bounding rectangle.</param>
        public Thickness(double uniformLength) {
            Left = Top = Right = Bottom = uniformLength;
        }

        /// <summary>
        /// Initializes a new instance of the System.Windows.Thickness structure that has
        /// specific lengths (supplied as a System.Double) applied to each side of the rectangle.
        /// </summary>
        /// <param name="left">The thickness for the left side of the rectangle.</param>
        /// <param name="top">The thickness for the upper side of the rectangle.</param>
        /// <param name="right">The thickness for the right side of the rectangle.</param>
        /// <param name="bottom">The thickness for the lower side of the rectangle.</param>
        public Thickness(double left, double top, double right, double bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="Thickness"/> are the same
        /// </summary>
        /// <param name="obj">The object to compare to this instance</param>
        /// <returns><b>True</b> of the object of the obj parameter is the same as the current instance</returns>
        public override bool Equals(object obj) {
            if (obj is Thickness) {
                Thickness otherObj = (Thickness)obj;
                return (this == otherObj);
            }
            return (false);
        }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="Thickness"/> are the same
        /// </summary>
        /// <param name="thickness">The object to compare to this instance</param>
        /// <returns><b>True</b> of the object of the obj parameter is the same as the current instance</returns>
        public bool Equals(Thickness thickness) {
            return (this == thickness);
        }

        /// <summary>
        /// Returns <see cref="System.Windows.Thickness"/> that represents this wrapper.
        /// </summary>
        /// <returns><see cref="System.Windows.Thickness"/> that represents this wrapper</returns>
        public System.Windows.Thickness ToRealThickness() {
            return new System.Windows.Thickness(Left, Top, Right, Bottom);
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Thickness"/>
        /// </summary>
        /// <returns>Hash code for this <see cref="Thickness"/></returns>
        public override int GetHashCode() {
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
        }
    }
}