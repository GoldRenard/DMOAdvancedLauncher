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
using System.ComponentModel;

namespace AdvancedLauncher.SDK.Management.Windows {

    public abstract class NamedItem : INotifyPropertyChanged {
        protected ILanguageManager LanguageManager;

        protected readonly string BindingName;

        protected readonly string Label;

        public event PropertyChangedEventHandler PropertyChanged;

        public NamedItem(string Label)
            : this(Label, null, null) {
        }

        public NamedItem(ILanguageManager LanguageManager, string bindingName)
            : this(null, LanguageManager, bindingName) {
        }

        public NamedItem(string Label, ILanguageManager LanguageManager, string BindingName) {
            this.Label = Label;
            this.LanguageManager = LanguageManager;
            this.BindingName = BindingName;
            if (LanguageManager != null) {
                LanguageManager.LanguageChanged += (s, e) => {
                    this.NotifyPropertyChanged("Name");
                };
            }
        }

        public string Name {
            get {
                if (LanguageManager == null || BindingName == null) {
                    if (Label != null) {
                        return Label;
                    }
                    return "N/A";
                }
                return (string)LanguageManager.Model.GetType().GetProperty(BindingName).GetValue(LanguageManager.Model, null);
            }
        }

        protected void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}