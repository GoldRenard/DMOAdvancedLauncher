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
using System.Windows.Threading;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.Tools;
using Ninject;

namespace AdvancedLauncher.Model {

    public class TamerViewModel : AbstractContainerViewModel<Tamer, TamerItemViewModel> {

        public TamerViewModel(Dispatcher OwnerDispatcher)
            : base(OwnerDispatcher) {
        }

        public override void LoadData(ICollection<Tamer> List) {
            this.IsDataLoaded = true;
            foreach (Tamer item in List) {
                TamerItemViewModel newItem = App.Kernel.Get<TamerItemViewModel>();
                newItem.TName = item.Name;
                newItem.TType = item.Type != null ? item.Type.Name : "N/A";
                newItem.Level = item.Level;
                newItem.PName = item.Partner.Name;
                newItem.Rank = item.Rank;
                newItem.DCnt = item.Digimons.Count;
                newItem.Tamer = item;
                newItem.Image = IconHolder.GetImage(item.Type.Code, false);
                this.Items.Add(newItem);
            }
        }
    }
}