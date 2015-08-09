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

    /// <summary>
    /// Base <see cref="AbstractUserControl"/> implementation for <see cref="IWindowManager"/> page items.
    /// </summary>
    public abstract class AbstractPageControl : AbstractUserControl {

        /// <summary>
        /// <b>True</b> if page was already activated
        /// </summary>
        protected bool IsPageActivated = false;

        /// <summary>
        /// <b>True</b> if page is visible
        /// </summary>
        protected bool IsPageVisible = false;

        /// <summary>
        /// Gets <see cref="IProfileManager"/> API
        /// </summary>
        protected IProfileManager ProfileManager {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AbstractPageControl"/> for specified <see cref="ILanguageManager"/>,
        /// <see cref="IWindowManager"/> and <see cref="IProfileManager"/>.
        /// </summary>
        /// <param name="LanguageManager">LanguageManager API</param>
        /// <param name="WindowManager">WindowManager API</param>
        /// <param name="ProfileManager">ProfileManager API</param>
        public AbstractPageControl(ILanguageManager LanguageManager, IWindowManager WindowManager, IProfileManager ProfileManager)
            : base(LanguageManager, WindowManager) {
            this.Container = new PageContainer(this);
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                this.ProfileManager = ProfileManager;
                ProfileManager.ProfileChanged += OnProfileChangedInternal;
            }
        }

        /// <summary>
        /// Internal <see cref="OnShow"/> handler
        /// </summary>
        public void OnShowInternal() {
            if (!IsPageActivated) {
                OnProfileChangedInternal(this, BaseEventArgs.Empty);
            }
            IsPageActivated = true;
            IsPageVisible = true;
            OnShow();
        }

        /// <summary>
        /// Internal <see cref="OnClose"/> handler
        /// </summary>
        public virtual void OnCloseInternal() {
            IsPageVisible = false;
            OnClose();
        }

        /// <summary>
        /// Page show handler
        /// </summary>
        protected virtual void OnShow() {
        }

        /// <summary>
        /// Page close handler
        /// </summary>
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

        /// <summary>
        /// <see cref="IProfileManager.ProfileChanged"/> event handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        protected abstract void OnProfileChanged(object sender, BaseEventArgs e);
    }
}