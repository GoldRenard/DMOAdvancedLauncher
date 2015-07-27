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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AdvancedLauncher.Model;
using DMOLibrary.Database.Entity;

namespace AdvancedLauncher.UI.Controls {

    public partial class TDBlock : AbstractUserControl {
        private TamerViewModel TamerModel;
        private DigimonViewModel DigimonModel;
        private bool IsFullDigimonList = true;
        private int CurrentTabIndex = 0;

        private Guild CURRENT_GUILD = new Guild() {
            Id = -1
        };

        public TDBlock() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                TamerModel = new TamerViewModel(this.Dispatcher);
                DigimonModel = new DigimonViewModel(this.Dispatcher);
                Tamers.DataContext = TamerModel;
                Digimons.DataContext = DigimonModel;
                TamerModel.LoadStarted += TamersLoadStarted;
                TamerModel.LoadCompleted += TamersLoadCompleted;
                DigimonModel.LoadStarted += DigimonsLoadStarted;
                DigimonModel.LoadCompleted += DigimonsLoadCompleted;
                LanguageManager.LanguageChanged += (s, e) => {
                    this.DataContext = LanguageManager.Model;
                };
            }
        }

        private void TamersLoadStarted(object sender, EventArgs e) {
            LoaderRing.IsActive = true;
            MainTabControl.IsEnabled = false;
            TamerModel.UnLoadData();
        }

        private void TamersLoadCompleted(object sender, EventArgs e) {
            MainTabControl.IsEnabled = true;
            LoaderRing.IsActive = false;
            MainTabControl.SelectedIndex = 0;
        }

        private void DigimonsLoadStarted(object sender, EventArgs e) {
            LoaderRing.IsActive = true;
            MainTabControl.IsEnabled = false;
            DigimonModel.UnLoadData();
        }

        private void DigimonsLoadCompleted(object sender, EventArgs e) {
            MainTabControl.IsEnabled = true;
            LoaderRing.IsActive = false;
            IsFullDigimonList = false;
            MainTabControl.SelectedIndex = 1;
            IsFullDigimonList = true;
        }

        public void ClearAll() {
            TamerModel.UnLoadData();
            DigimonModel.UnLoadData();
            IsFullDigimonList = true;
            MainTabControl.SelectedIndex = 0;
            MainTabControl.IsEnabled = false;
        }

        #region Showing tabs

        private void OnTamersShow(object sender, MouseButtonEventArgs e) {
            if (TamerModel.IsDataLoaded && Tamers.SelectedIndex >= 0) {
                TamerItemViewModel selectedItem = (TamerItemViewModel)Tamers.SelectedItem;
                if (selectedItem != null) {
                    DigimonModel.LoadDataAsync(selectedItem.Tamer);
                }
            }
        }

        public void SetGuild(Guild guild) {
            CURRENT_GUILD = guild;
            DigimonModel.ClearCache();
            TamerModel.ClearCache();
            TamerModel.LoadDataAsync(guild.Tamers);
        }

        #endregion Showing tabs

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TabControl control = (TabControl)sender;
            if (CURRENT_GUILD.Id == -1 || control.SelectedIndex == CurrentTabIndex) {
                return;
            }
            CurrentTabIndex = control.SelectedIndex;
            switch (CurrentTabIndex) {
                case 1:
                    if (IsFullDigimonList) {
                        DigimonModel.LoadDataAsync(CURRENT_GUILD.Tamers);
                        MainTabControl.SelectedIndex = 0;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}