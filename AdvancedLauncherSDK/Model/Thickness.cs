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

    [Serializable]
    public sealed class Thickness {

        public double Left {
            get;
            set;
        }

        public double Top {
            get;
            set;
        }

        public double Right {
            get;
            set;
        }

        public double Bottom {
            get;
            set;
        }

        public Thickness() : this(0) {
        }

        public Thickness(double uniformLength) {
            Left = Top = Right = Bottom = uniformLength;
        }

        public Thickness(double left, double top, double right, double bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override bool Equals(object obj) {
            if (obj is Thickness) {
                Thickness otherObj = (Thickness)obj;
                return (this == otherObj);
            }
            return (false);
        }

        public bool Equals(Thickness thickness) {
            return (this == thickness);
        }

        public System.Windows.Thickness ToRealThickness() {
            return new System.Windows.Thickness(Left, Top, Right, Bottom);
        }

        public override int GetHashCode() {
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
        }
    }
}