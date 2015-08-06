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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AdvancedLauncher.Model.Proxy;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;

namespace AdvancedLauncher.UI.Extension {

    public static class ObservableCollectionExtension {

        public static ObservableCollection<P> GetLinkedProxy<T, P>(this ObservableCollection<T> Collection, ILanguageManager LanguageManager)
            where T : NamedItem
            where P : NamedItemViewModel<T> {
            ObservableCollection<P> ProxyCollection = new ObservableCollection<P>();
            foreach (T Item in Collection) {
                ProxyCollection.Add((P)Activator.CreateInstance(typeof(P), new object[] { Item, LanguageManager }));
            }

            Collection.CollectionChanged += (s, e) => {
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add:
                        foreach (T Item in e.NewItems) {
                            ProxyCollection.Add((P)Activator.CreateInstance(typeof(P), new object[] { Item, LanguageManager }));
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (T Item in e.NewItems) {
                            P proxy = ProxyCollection.FirstOrDefault(p => p.Item.Equals(Item));
                            if (proxy != null) {
                                ProxyCollection.Remove(proxy);
                            }
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        ProxyCollection.Clear();
                        break;

                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Move:
                        throw new NotImplementedException();
                }
            };
            return ProxyCollection;
        }
    }
}