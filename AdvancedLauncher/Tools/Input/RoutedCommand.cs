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
using System.Windows.Input;

namespace AdvancedLauncher.Tools.Input {

    /// <summary>
    ///     A simple generic version of RoutedCommand that provides new
    ///     strongly typed methods to support command parameters of type T.
    /// </summary>
    public class RoutedCommand<T> : RoutedCommand {

        public RoutedCommand() {
        }

        public RoutedCommand(string name, Type ownerType) : base(name, ownerType) {
        }

        public RoutedCommand(string name, Type ownerType, InputGestureCollection inputGestures) : base(name, ownerType, inputGestures) {
        }

        /// <summary>
        ///     Determines whether this command can execute on the specified
        ///     target.
        /// </summary>
        public bool CanExecute(T parameter, IInputElement target) {
            return base.CanExecute(parameter, target);
        }

        /// <summary>
        ///     Executes the command on the specified target.
        /// </summary>
        public void Execute(T parameter, IInputElement target) {
            base.Execute(parameter, target);
        }
    }
}