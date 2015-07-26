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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Threading;
using DMOLibrary.Database.Context;
using DMOLibrary.Database.Entity;

namespace AdvancedLauncher.UI.Controls {

    public class DigimonViewModel : AbstractContainerViewModel<Tamer, DigimonItemViewModel> {
        private static string SIZE_FORMAT = "{0}cm ({1}%)";

        public DigimonViewModel(Dispatcher OwnerDispatcher)
            : base(OwnerDispatcher) {
        }

        private ICollection<DigimonItemViewModel> LoadDigimonList(Tamer tamer) {
            string typeName;
            DigimonType dtype;
            ICollection<DigimonItemViewModel> items = null;
            if (ItemsCache.TryGetValue(tamer, out items)) {
                return items;
            }
            items = new List<DigimonItemViewModel>();
            BindingOperations.EnableCollectionSynchronization(items, _stocksLock);
            using (MainContext context = new MainContext()) {
                foreach (Digimon item in tamer.Digimons) {
                    dtype = context.FindDigimonTypeByCode(item.Type.Code);
                    typeName = dtype.Name;
                    if (dtype.NameAlt != null) {
                        typeName += " (" + dtype.NameAlt + ")";
                    }
                    items.Add(new DigimonItemViewModel {
                        DName = item.Name,
                        DType = typeName,
                        Image = IconHolder.GetImage(item.Type.Code),
                        TName = tamer.Name,
                        Level = item.Level,
                        SizePC = item.SizePc,
                        Size = string.Format(SIZE_FORMAT, item.SizeCm, item.SizePc),
                        Rank = item.Rank
                    });
                }
            }
            ItemsCache.TryAdd(tamer, items);
            return items;
        }

        public override void LoadData(Tamer tamer) {
            this.IsDataLoaded = true;
            this.Items = new ObservableCollection<DigimonItemViewModel>(LoadDigimonList(tamer));
        }

        public override void LoadData(ICollection<Tamer> tamers) {
            this.IsDataLoaded = true;
            List<DigimonItemViewModel> items = new List<DigimonItemViewModel>();
            foreach (Tamer tamer in tamers) {
                items.AddRange(LoadDigimonList(tamer));
            }
            this.Items = new ObservableCollection<DigimonItemViewModel>(items);
        }
    }
}