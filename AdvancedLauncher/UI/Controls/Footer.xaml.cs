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
using System.Windows.Input;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.Tools;
using AdvancedLauncher.UI.Commands;
using Ninject;

namespace AdvancedLauncher.UI.Controls {

    public partial class Footer : AbstractUserControl {

        [Inject]
        public WikiProvider Provider {
            get; set;
        }

        public Footer() {
            InitializeComponent();
            this.DataContext = this;
            Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string ver = v.Major.ToString() + "." + v.Minor.ToString();
            ver += " (build " + v.Build.ToString() + ")";
            VersionBlock.Text = string.Format(VersionBlock.Text, ver);
            SearchBox.SuggestionCommit = new ModelCommand(Search);
            LanguageManager.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, BaseEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new BaseEventHandler((s, e2) => {
                    OnLanguageChanged(sender, e2);
                }), sender, e);
                return;
            }
            SearchBox.DataContext = LanguageManager.Model;
        }

        public void Search(object obj) {
            if (obj == null) {
                return;
            }
            string url = null;
            if (typeof(WikiProvider.Suggestion).IsAssignableFrom(obj.GetType())) {
                url = Provider.URL + (obj as WikiProvider.Suggestion).Value;
            }
            if (typeof(string).IsAssignableFrom(obj.GetType())) {
                if (!string.IsNullOrEmpty((string)obj)) {
                    url = string.Format(Provider.Search, obj);
                }
            }
            if (url != null) {
                URLUtils.OpenSite(url);
            }
        }

        private void AutoCompleteTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                Search(SearchBox.Text);
            }
        }
    }
}