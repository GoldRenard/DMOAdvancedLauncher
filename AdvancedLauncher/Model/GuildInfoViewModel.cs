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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;
using AdvancedLauncher.SDK.Model.Entity;

namespace AdvancedLauncher.Model {

    public class GuildInfoViewModel : AbstractContainerViewModel<Guild, GuildInfoItemViewModel>, IDisposable {
        private static object NO_DATA_CHAR = "-";

        private ILanguageManager LanguageManager {
            get; set;
        }

        public GuildInfoViewModel(ILanguageManager LanguageManager)
            : base(null) {
            this.LanguageManager = LanguageManager;
            this.LanguageManager.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, SDK.Model.Events.BaseEventArgs e) {
            NotifyPropertyChanged("Items");
        }

        public override void UnLoadData() {
            base.UnLoadData();
            LoadGuildData(null);
        }

        private void LoadGuildData(Guild item) {
            LoadItem<LanguageModel>(e => e.CommGMaster, item != null ? item.Master.Name : NO_DATA_CHAR);
            LoadItem<LanguageModel>(e => e.CommGBest, item != null ? item.Tamers.Aggregate((t1, t2) => (t1.Rank > t2.Rank ? t2 : t1)).Name : NO_DATA_CHAR);
            LoadItem<LanguageModel>(e => e.CommGRank, item != null ? item.Rank : NO_DATA_CHAR);
            LoadItem<LanguageModel>(e => e.CommGRep, item != null ? item.Rep : NO_DATA_CHAR);
            LoadItem<LanguageModel>(e => e.CommGTCnt, item != null ? item.Tamers.Count : NO_DATA_CHAR);
            LoadItem<LanguageModel>(e => e.CommGDCnt, item != null ? item.Tamers.Select(o => o.Digimons.Count).Aggregate((x, y) => x + y) : NO_DATA_CHAR);
            IsDataLoaded = true;
        }

        public override void LoadData(Guild item) {
            this.Items.Clear();
            LoadGuildData(item);
        }

        public override void LoadData(ICollection<Guild> List) {
            // deprecated
        }

        private void LoadItem<T>(Expression<Func<T, object>> expression, object value) {
            var body = expression.Body as MemberExpression;
            if (body == null) {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            this.Items.Add(new GuildInfoItemViewModel(LanguageManager) {
                Name = body.Member.Name,
                Value = value
            });
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    this.LanguageManager.LanguageChanged -= OnLanguageChanged;
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