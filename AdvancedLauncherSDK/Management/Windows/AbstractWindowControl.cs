﻿// ======================================================================
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
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Events.Proxy;

namespace AdvancedLauncher.SDK.Management.Windows {

    public abstract class AbstractWindowControl : UserControl {

        protected ILanguageManager LanguageManager {
            get;
            private set;
        }

        protected IWindowManager WindowManager {
            get;
            private set;
        }

        public IWindow Container {
            get;
            private set;
        }

        public AbstractWindowControl(ILanguageManager LanguageManager, IWindowManager WindowManager) {
            if (LanguageManager == null) {
                throw new ArgumentException("LanguageManager cannot be null");
            }
            if (WindowManager == null) {
                throw new ArgumentException("WindowManager cannot be null");
            }
            this.WindowManager = WindowManager;
            this.LanguageManager = LanguageManager;
            this.LanguageManager.LanguageChangedProxy(new BaseEventProxy(OnLanguageChanged));
            this.Container = new WindowContainer(this, WindowManager);
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

        protected void OnCloseClick(object sender, System.Windows.RoutedEventArgs e) {
            Close();
        }

        public virtual void OnShow() {
            // nothing to do here
        }

        /// <summary>
        /// Returns to last opened window. You're free to override this method.
        /// </summary>
        public virtual void Close() {
            WindowManager.GoBack(Container);
        }
    }
}