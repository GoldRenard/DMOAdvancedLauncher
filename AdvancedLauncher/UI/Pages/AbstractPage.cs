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
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.UI.Controls;

namespace AdvancedLauncher.UI.Pages {

    public abstract class AbstractPage : AbstractUserControl {
        protected bool IsPageActivated = false;

        protected bool IsPageVisible = false;

        public AbstractPage() : base() {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                ProfileManager.ProfileChanged += OnProfileChangedInternal;
                LanguageManager.LanguageChanged += OnLanguageChanged;
            }
        }

        private void OnLanguageChanged(object sender, BaseEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new BaseEventHandler((s, e2) => {
                    OnLanguageChanged(sender, e2);
                }), sender, e);
                return;
            }
            this.DataContext = LanguageManager.Model;
        }

        public virtual void PageActivate() {
            if (!IsPageActivated) {
                OnProfileChangedInternal(this, BaseEventArgs.Empty);
            }
            IsPageActivated = true;
            IsPageVisible = true;
        }

        public virtual void PageClose() {
            IsPageVisible = false;
        }

        private void OnProfileChangedInternal(object sender, BaseEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new BaseEventHandler((s, e2) => {
                    OnProfileChangedInternal(sender, e2);
                }), sender, e);
                return;
            }
            OnProfileChanged(sender, e);
        }

        protected abstract void OnProfileChanged(object sender, BaseEventArgs e);
    }
}