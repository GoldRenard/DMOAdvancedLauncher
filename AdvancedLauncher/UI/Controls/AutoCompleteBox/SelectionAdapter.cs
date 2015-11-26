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
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AdvancedLauncher.UI.Controls.AutoCompleteBox {

    public class SelectionAdapter {

        #region "Fields"

        private Selector _selectorControl;

        #endregion "Fields"

        #region "Constructors"

        public SelectionAdapter(Selector selector) {
            SelectorControl = selector;
            SelectorControl.PreviewMouseUp += OnSelectorMouseDown;
        }

        #endregion "Constructors"

        #region "Events"

        public delegate void CancelEventHandler(object sender, EventArgs e);

        public delegate void CommitEventHandler(object sender, EventArgs e);

        public delegate void SelectionChangedEventHandler(object sender, EventArgs e);

        public event CancelEventHandler Cancel;

        public event CommitEventHandler Commit;

        public event SelectionChangedEventHandler SelectionChanged;

        #endregion "Events"

        #region "Properties"

        public Selector SelectorControl {
            get {
                return _selectorControl;
            }
            set {
                _selectorControl = value;
            }
        }

        #endregion "Properties"

        #region "Methods"

        public void HandleKeyDown(KeyEventArgs key) {
            Debug.WriteLine(key.Key);
            switch (key.Key) {
                case Key.Down:
                    IncrementSelection();
                    break;

                case Key.Up:
                    DecrementSelection();
                    break;

                case Key.Enter:
                    if (Commit != null) {
                        Commit(this, EventArgs.Empty);
                    }

                    break;

                case Key.Escape:
                    if (Cancel != null) {
                        Cancel(this, EventArgs.Empty);
                    }

                    break;

                case Key.Tab:
                    if (Commit != null) {
                        Commit(this, EventArgs.Empty);
                    }

                    break;
            }
        }

        private void DecrementSelection() {
            if (SelectorControl.SelectedIndex == -1) {
                SelectorControl.SelectedIndex = SelectorControl.Items.Count - 1;
            } else {
                SelectorControl.SelectedIndex -= 1;
            }
            if (SelectionChanged != null) {
                SelectionChanged(this, EventArgs.Empty);
            }
        }

        private void IncrementSelection() {
            if (SelectorControl.SelectedIndex == SelectorControl.Items.Count - 1) {
                SelectorControl.SelectedIndex = -1;
            } else {
                SelectorControl.SelectedIndex += 1;
            }
            if (SelectionChanged != null) {
                SelectionChanged(this, EventArgs.Empty);
            }
        }

        private void OnSelectorMouseDown(object sender, MouseButtonEventArgs e) {
            if (Commit != null) {
                Commit(this, EventArgs.Empty);
            }
        }

        #endregion "Methods"
    }
}