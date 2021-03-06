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
using System.ComponentModel;
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.Model {

    public abstract class AbstractItemViewModel<T> : INotifyPropertyChanged, IDisposable {

        protected ILanguageManager LanguageManager {
            get; private set;
        }

        protected virtual void OnLanguageChanged(object sender, SDK.Model.Events.BaseEventArgs e) {
            // nothing to do here
        }

        public AbstractItemViewModel(ILanguageManager LanguageManager) {
            if (LanguageManager != null) {
                this.LanguageManager = LanguageManager;
                this.LanguageManager.LanguageChanged += OnLanguageChanged;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    if (LanguageManager != null) {
                        LanguageManager.LanguageChanged -= OnLanguageChanged;
                    }
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