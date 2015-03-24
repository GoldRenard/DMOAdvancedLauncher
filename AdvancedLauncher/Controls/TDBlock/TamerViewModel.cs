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
using DMOLibrary.Database.Entity;

namespace AdvancedLauncher.Controls {

    public class TamerViewModel : AbstractContainerViewModel<Tamer, TamerItemViewModel> {

        public TamerViewModel(Dispatcher OwnerDispatcher)
            : base(OwnerDispatcher) {
        }

        public override void LoadData(ICollection<Tamer> List) {
            this.IsDataLoaded = true;
            foreach (Tamer item in List) {
                this.Items.Add(new TamerItemViewModel {
                    TName = item.Name,
                    TType = item.Type != null ? item.Type.Name : "N/A",
                    Level = item.Level,
                    PName = item.Partner.Name,
                    Rank = item.Rank,
                    DCnt = item.Digimons.Count,
                    Tamer = item,
                    Image = IconHolder.GetImage(item.Type.Code, false)
                });
            }
        }
    }
}