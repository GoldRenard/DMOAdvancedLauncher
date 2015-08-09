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
using System.Windows.Controls;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Events.Proxy;

namespace AdvancedLauncher.SDK.UI {

    /// <summary>
    /// Base <see cref="UserControl"/> implementation for <see cref="IRemoteControl"/>
    /// </summary>
    public abstract class AbstractUserControl : UserControl {

        /// <summary>
        /// Gets <see cref="ILanguageManager"/> API
        /// </summary>
        protected ILanguageManager LanguageManager {
            get;
            private set;
        }

        /// <summary>
        /// Gets <see cref="IWindowManager"/> API
        /// </summary>
        protected IWindowManager WindowManager {
            get;
            private set;
        }

        /// <summary>
        /// Gets <see cref="IRemoteControl"/> container of this control
        /// </summary>
        public IRemoteControl Container {
            get;
            protected set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AbstractUserControl"/> for specified <see cref="ILanguageManager"/>
        /// and <see cref="IWindowManager"/>.
        /// </summary>
        /// <param name="LanguageManager">LanguageManager API</param>
        /// <param name="WindowManager">WindowManager API</param>
        public AbstractUserControl(ILanguageManager LanguageManager, IWindowManager WindowManager) {
            if (LanguageManager == null) {
                throw new ArgumentException("LanguageManager cannot be null");
            }
            this.WindowManager = WindowManager;
            this.LanguageManager = LanguageManager;
            this.LanguageManager.LanguageChangedProxy(new BaseEventProxy(OnLanguageChanged));
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
    }
}