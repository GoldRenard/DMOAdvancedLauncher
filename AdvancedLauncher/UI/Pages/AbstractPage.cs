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
using AdvancedLauncher.UI.Controls;

namespace AdvancedLauncher.UI.Pages {

    public abstract class AbstractPage : AbstractUserControl {
        protected bool IsPageActivated = false;

        protected bool IsPageVisible = false;

        public AbstractPage() : base() {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                ProfileManager.ProfileChanged += ProfileChanged;
                LanguageManager.LanguageChanged += (s, e) => {
                    this.DataContext = LanguageManager.Model;
                };
            }
        }

        public virtual void PageActivate() {
            if (!IsPageActivated) {
                ProfileChanged(this, SDK.Model.Events.EventArgs.Empty);
            }
            IsPageActivated = true;
            IsPageVisible = true;
        }

        public virtual void PageClose() {
            IsPageVisible = false;
        }

        protected abstract void ProfileChanged(object sender, SDK.Model.Events.EventArgs e);
    }
}