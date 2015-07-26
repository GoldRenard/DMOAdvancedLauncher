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
using System.Windows.Input;

namespace AdvancedLauncher.UI.Commands {

    public class ModelCommand : ICommand {

        public event EventHandler CanExecuteChanged;

        private readonly Action<object> _execute = null;
        private readonly Predicate<object> _canExecute = null;

        public ModelCommand(Action<object> execute)
            : this(execute, null) {
        }

        public ModelCommand(Action<object> execute, Predicate<object> canExecute) {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) {
            return _canExecute != null ? _canExecute(parameter) : true;
        }

        public void Execute(object parameter) {
            if (_execute != null)
                _execute(parameter);
        }

        public void OnCanExecuteChanged() {
            CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}