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
using AdvancedLauncher.Controls;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Service;
using AdvancedLauncher.Validators;
using DMOLibrary.Database;
using DMOLibrary.Database.Entity;
using DMOLibrary.Events;
using DMOLibrary.Profiles;

namespace AdvancedLauncher.Pages {

    public partial class Community : AbstractPage {

        private delegate void DoOneText(string text);

        private AbstractWebProfile webProfile;

        private GuildInfoViewModel GuildInfoModel = new GuildInfoViewModel();

        private Guild CURRENT_GUILD = new Guild() {
            Id = -1
        };

        protected override void InitializeAbstractPage() {
            InitializeComponent();
            GuildInfo.DataContext = GuildInfoModel;
        }

        protected override void ProfileChanged() {
            GuildInfoModel.UnLoadData();
            TDBlock_.ClearAll();
            IsDetailedCheckbox.IsChecked = false;
            webProfile = LauncherEnv.Settings.CurrentProfile.DMOProfile.GetWebProfile();
            if (webProfile != null) {
                webProfile.SetDispatcher(this.Dispatcher);
            }
            // use lazy ServerList initialization to prevent first long EF6 database
            // init causes the long app start time
            if (IsPageActivated) {
                LoadServerList();
            }
        }

        public override void PageActivate() {
            base.PageActivate();
            LoadServerList();
        }

        private void LoadServerList() {
            //Загружаем новый список серверов
            ComboBoxServer.ItemsSource = LauncherEnv.Settings.CurrentProfile.DMOProfile.ServerList;
            //Если есть название гильдии в ротации, вводим его и сервер
            if (!string.IsNullOrEmpty(LauncherEnv.Settings.CurrentProfile.Rotation.Guild)) {
                foreach (Server serv in ComboBoxServer.Items) {
                    //Ищем сервер с нужным идентификатором и выбираем его
                    if (serv.Identifier == LauncherEnv.Settings.CurrentProfile.Rotation.ServerId + 1) {
                        ComboBoxServer.SelectedValue = serv;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(GuildNameTextBox.Text)) {
                    GuildNameTextBox.Text = LauncherEnv.Settings.CurrentProfile.Rotation.Guild;
                }
            } else {
                GuildNameTextBox.Clear();
                if (ComboBoxServer.Items.Count > 0) {
                    ComboBoxServer.SelectedIndex = 0;
                }
            }
        }

        private void OnStatusChanged(object sender, DownloadStatusEventArgs e) {
            switch (e.Code) {
                case DMODownloadStatusCode.GETTING_GUILD: {
                        LoadProgressStatus.Text = LanguageEnv.Strings.CommSearchingGuild;
                        break;
                    }
                case DMODownloadStatusCode.GETTING_TAMER: {
                        LoadProgressStatus.Text = string.Format(LanguageEnv.Strings.CommGettingTamer, e.Info);
                        break;
                    }
            }
            LoadProgressBar.Maximum = e.MaxProgress;
            LoadProgressBar.Value = e.Progress;
        }

        private void OnDownloadCompleted(object sender, DownloadCompleteEventArgs e) {
            BlockControls(false);

            webProfile.DownloadStarted -= OnDownloadStarted;
            webProfile.DownloadCompleted -= OnDownloadCompleted;
            webProfile.StatusChanged -= OnStatusChanged;

            ProgressBlock.Visibility = System.Windows.Visibility.Collapsed;
            switch (e.Code) {
                case DMODownloadResultCode.OK: {
                        CURRENT_GUILD = MergeHelper.Merge(e.Guild);
                        GuildInfoModel.LoadData(CURRENT_GUILD);
                        TDBlock_.SetGuild(CURRENT_GUILD);
                        break;
                    }
                case DMODownloadResultCode.CANT_GET: {
                        Utils.ShowErrorDialog(LanguageEnv.Strings.CantGetError);
                        break;
                    }
                case DMODownloadResultCode.DB_CONNECT_ERROR: {
                        Utils.ShowErrorDialog(LanguageEnv.Strings.DBConnectionError);
                        break;
                    }
                case DMODownloadResultCode.NOT_FOUND: {
                        Utils.ShowErrorDialog(LanguageEnv.Strings.GuildNotFoundError);
                        break;
                    }
                case DMODownloadResultCode.WEB_ACCESS_ERROR: {
                        Utils.ShowErrorDialog(LanguageEnv.Strings.ConnectionError);
                        break;
                    }
            }
        }

        private void OnDownloadStarted(object sender, EventArgs e) {
            BlockControls(true);
            LoadProgressBar.Value = 0;
            LoadProgressBar.Maximum = 100;
            LoadProgressStatus.Text = string.Empty;
            ProgressBlock.Visibility = System.Windows.Visibility.Visible;
        }

        private void OnGetInfoClick(object sender, RoutedEventArgs e) {
            if (IsValidName(GuildNameTextBox.Text)) {
                webProfile.DownloadStarted += OnDownloadStarted;
                webProfile.DownloadCompleted += OnDownloadCompleted;
                webProfile.StatusChanged += OnStatusChanged;

                AbstractWebProfile.GetActualGuildAsync(this.Dispatcher,
                    webProfile,
                    (Server)ComboBoxServer.SelectedValue,
                    GuildNameTextBox.Text,
                    (bool)IsDetailedCheckbox.IsChecked,
                    1);
            }
        }

        public void BlockControls(bool block) {
            LauncherEnv.Settings.OnProfileLocked(block);
            GuildNameTextBox.IsEnabled = !block;
            ComboBoxServer.IsEnabled = !block;
            SearchButton.IsEnabled = !block;
            IsDetailedCheckbox.IsEnabled = !block;
        }

        #region Обработка поля ввода имени гильдии

        public static bool IsValidName(string name) {
            if (name == LanguageEnv.Strings.CommGuildName) {
                Utils.ShowErrorDialog(LanguageEnv.Strings.CommGuildNameEmpty);
                return false;
            }
            GuildNameValidationRule validationRule = new GuildNameValidationRule();
            ValidationResult result = validationRule.Validate(name, new System.Globalization.CultureInfo(1, false));
            if (!result.IsValid) {
                Utils.ShowErrorDialog(result.ErrorContent.ToString());
            }
            return result.IsValid;
        }

        #endregion Обработка поля ввода имени гильдии
    }
}