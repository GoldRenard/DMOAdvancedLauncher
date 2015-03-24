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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;

namespace AdvancedLauncher.Controls {

    public abstract class AbstractContainerViewModel<SourceType, ItemViewModel> : INotifyPropertyChanged {
        private readonly Dispatcher OwnerDispatcher;

        protected object _stocksLock = new object();

        public delegate void LoadEventHandler(object sender, EventArgs e);

        public event LoadEventHandler LoadStarted;

        public event LoadEventHandler LoadCompleted;

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ItemViewModel> _Items;

        public ObservableCollection<ItemViewModel> Items {
            get {
                return _Items;
            }
            protected set {
                if (_Items != value) {
                    _Items = value;
                    NotifyPropertyChanged("Items");
                }
            }
        }

        protected ConcurrentDictionary<SourceType, ICollection<ItemViewModel>> ItemsCache {
            get;
            set;
        }

        public AbstractContainerViewModel(Dispatcher OwnerDispatcher) {
            this.OwnerDispatcher = OwnerDispatcher;
            this.Items = new ObservableCollection<ItemViewModel>();
            this.ItemsCache = new ConcurrentDictionary<SourceType, ICollection<ItemViewModel>>();
            BindingOperations.EnableCollectionSynchronization(Items, _stocksLock);
        }

        public bool IsDataLoaded {
            get;
            protected set;
        }

        public virtual void UnLoadData() {
            this.IsDataLoaded = false;
            this.Items.Clear();
        }

        public void RemoveAt(int index) {
            if (this.IsDataLoaded && index < this.Items.Count) {
                this.Items.RemoveAt(index);
            }
        }

        public abstract void LoadData(ICollection<SourceType> items);

        public virtual void LoadData(SourceType item) {
            LoadData(new List<SourceType>() { item });
        }

        public void LoadDataAsync(SourceType item) {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, e) => {
                LoadData(item);
            };
            worker.RunWorkerCompleted += RunWorkerCompleted;
            if (LoadStarted != null) {
                LoadStarted(this, null);
            }
            worker.RunWorkerAsync();
        }

        public void LoadDataAsync(ICollection<SourceType> items) {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, e) => {
                LoadData(items);
            };
            worker.RunWorkerCompleted += RunWorkerCompleted;
            if (LoadStarted != null) {
                LoadStarted(this, null);
            }
            worker.RunWorkerAsync();
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            this.OwnerDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => {
                if (LoadCompleted != null) {
                    LoadCompleted(this, null);
                }
            }));
        }

        protected void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}