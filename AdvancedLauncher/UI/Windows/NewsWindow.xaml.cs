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
using System.Windows.Media;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.UI.Pages;
using Ninject;

namespace AdvancedLauncher.UI.Windows {

    public partial class NewsWindow : AbstractWindow {
        private AbstractPage currentTab;

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get; set;
        }

        [Inject]
        public IConfigurationManager ConfigurationManager {
            get; set;
        }

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        public NewsWindow() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
                EnvironmentManager.FileSystemLocked += OnFileSystemLocked;
                ProfileManager.ProfileChanged += OnProfileChanged;
                OnProfileChanged(this, EventArgs.Empty);
            }
        }

        private void OnFileSystemLocked(object sender, LockedEventArgs e) {
            if (e.IsLocked) {
                //Выбираем первую вкладку и отключаем персонализацию (на всякий случай)
                NavControl.SelectedIndex = 0;
                NavPersonalization.IsEnabled = false;
            } else {
                //Включаем персонализации обратно если игра определена
                if (ConfigurationManager.CheckGame(ProfileManager.CurrentProfile.GameModel)) {
                    NavPersonalization.IsEnabled = true;
                }
            }
        }

        private void OnTabChanged(object sender, SelectionChangedEventArgs e) {
            TabItem selectedTab = (TabItem)NavControl.SelectedValue;
            AbstractPage selectedPage = (AbstractPage)selectedTab.Content;
            //Prevent handling over changing inside tab item
            if (currentTab == selectedPage) {
                return;
            }
            if (currentTab != null) {
                currentTab.PageClose();
            }
            currentTab = selectedPage;
            currentTab.PageActivate();
            selectedTab.Focus();
        }

        private void OnProfileChanged(object sender, EventArgs e) {
            //Выбираем первую вкладку и отключаем все модули
            NavControl.SelectedIndex = 0;
            NavGallery.IsEnabled = false;
            NavCommunity.IsEnabled = false;
            NavPersonalization.IsEnabled = false;

            IGameModel model = ProfileManager.CurrentProfile.GameModel;

            //Если доступен веб-профиль, включаем вкладку сообщества
            if (ConfigurationManager.GetConfiguration(model).IsWebAvailable) {
                NavCommunity.IsEnabled = true;
            }

            //Если путь до игры верен, включаем вкладку галереи и персонализации
            if (ConfigurationManager.CheckGame(model)) {
                NavGallery.IsEnabled = true;
                NavPersonalization.IsEnabled = true;
            }
        }
    }
}