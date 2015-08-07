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
using AdvancedLauncher.Model.Proxy;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.Model {

    public class NamedItemViewModel<T> : IDisposable, INotifyPropertyChanged, IPropertyChangedEventAccessor
        where T : NamedItem {
        private PropertyChangedEventAccessor PropertyChangedAccessor;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ILanguageManager LanguageManager;

        public T Item {
            get;
            private set;
        }

        public string Name {
            get {
                if (LanguageManager == null || !IsBinding) {
                    if (Item.Name != null) {
                        return Item.Name;
                    }
                    return "N/A";
                }
                if (Item.Name == null) {
                    return "N/A";
                }
                return (string)LanguageManager.Model.GetType().GetProperty(Item.Name).GetValue(LanguageManager.Model, null);
            }
        }

        public bool IsBinding {
            get {
                return Item.IsBinding;
            }
        }

        public bool IsEnabled {
            get {
                return Item.IsEnabled;
            }
        }

        public NamedItemViewModel(T Item, ILanguageManager LanguageManager) {
            this.PropertyChangedAccessor = new PropertyChangedEventAccessor(this);
            this.Item = Item;
            this.Item.PropertyChanged += PropertyChangedAccessor.OnPropertyChanged;
            this.LanguageManager = LanguageManager;
            this.LanguageManager.LanguageChanged += OnLanguageChanged;
        }

        #region Events

        protected virtual void NotifyPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnLanguageChanged(object sender, BaseEventArgs e) {
            NotifyPropertyChanged("Name");
        }

        public void OnPropertyChanged(object sender, RemotePropertyChangedEventArgs e) {
            NotifyPropertyChanged(e.PropertyName);
        }

        #endregion Events

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    LanguageManager.LanguageChanged -= OnLanguageChanged;
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}