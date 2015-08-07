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
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.UI {

    public abstract class AbstractPageControl : AbstractUserControl {
        protected bool IsPageActivated = false;

        protected bool IsPageVisible = false;

        protected IProfileManager ProfileManager {
            get;
            private set;
        }

        public AbstractPageControl(ILanguageManager LanguageManager, IWindowManager WindowManager, IProfileManager ProfileManager)
            : base(LanguageManager, WindowManager) {
            this.Container = new PageContainer(this);
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                this.ProfileManager = ProfileManager;
                ProfileManager.ProfileChanged += OnProfileChangedInternal;
            }
        }

        public void OnShowInternal() {
            if (!IsPageActivated) {
                OnProfileChangedInternal(this, BaseEventArgs.Empty);
            }
            IsPageActivated = true;
            IsPageVisible = true;
            OnShow();
        }

        public virtual void OnCloseInternal() {
            IsPageVisible = false;
            OnClose();
        }

        protected virtual void OnShow() {
        }

        protected virtual void OnClose() {
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