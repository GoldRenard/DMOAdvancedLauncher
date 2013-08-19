// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using DMOLibrary;
using DMOLibrary.Profiles;
using System.Linq;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Service;
using AdvancedLauncher.Validators;

namespace AdvancedLauncher.Pages
{
    public partial class Community : UserControl
    {
        Storyboard ShowWindow;
        private delegate void DoOneText(string text);
        DMOWebProfile dmo_web;
        guild CURRENT_GUILD = new guild() { Id = -1 };

        public Community()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
                LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
                LauncherEnv.Settings.ProfileChanged += ProfileChanged;
                TDBlock_.TabChanged += TDBlock_TabChanged;
                ProfileChanged();
            }
        }

        public void Activate()
        {
            ShowWindow.Begin();
        }

        void ProfileChanged()
        {
            //Очищаем все поля и списки
            GMaster.Text = GRank.Text = GRep.Text = GTop.Text = GDCnt.Text = GTCnt.Text = "-";
            TDBlock_.ClearAll();
            chkbox_IsDetailed.IsChecked = false;

            //Загружаем новый список серверов
            ComboBoxServer.ItemsSource = LauncherEnv.Settings.pCurrent.DMOProfile.ServerList;

            //Активируем новый профиль
            dmo_web = LauncherEnv.Settings.pCurrent.DMOProfile.WebProfile;

            //Если есть название гильдии в ротации, вводим его и сервер
            if (!string.IsNullOrEmpty(LauncherEnv.Settings.pCurrent.Rotation.Guild))
            {
                foreach (server serv in ComboBoxServer.Items)
                {
                    //Ищем сервер с нужным идентификатором и выбираем его
                    if (serv.Id == LauncherEnv.Settings.pCurrent.Rotation.ServerId + 1)
                    {
                        ComboBoxServer.SelectedValue = serv;
                        break;
                    }
                }
                textBox_g_name.Text = LauncherEnv.Settings.pCurrent.Rotation.Guild;
            }
            else
            {
                textBox_g_name.Text = LanguageEnv.Strings.CommGuildName;
                if (ComboBoxServer.Items.Count > 0)
                    ComboBoxServer.SelectedIndex = 0;
            }
        }

        void TDBlock_TabChanged(object sender, int tab_num)
        {
            if (tab_num == 1)
            {
                btn_ttab.IsEnabled = false;
                btn_dtab.IsEnabled = true;
            }
            else
            {
                btn_ttab.IsEnabled = true;
                btn_dtab.IsEnabled = false;
            }
        }

        private void btn_ttab_Click(object sender, RoutedEventArgs e)
        {
            if (CURRENT_GUILD.Id != -1)
                TDBlock_.ShowTamers();
        }

        private void btn_dtab_Click(object sender, RoutedEventArgs e)
        {
            if (CURRENT_GUILD.Id != -1)
                TDBlock_.ShowDigimons(CURRENT_GUILD.Members);
        }

        void dmo_web_StatusChanged(object sender, DownloadStatus status)
        {
            switch (status.code)
            {
                case DMODownloadStatusCode.GETTING_GUILD:
                    {
                        LoadProgressStatus.Text = LanguageEnv.Strings.CommSearchingGuild;
                        break;
                    }
                case DMODownloadStatusCode.GETTING_TAMER:
                    {
                        LoadProgressStatus.Text = string.Format(LanguageEnv.Strings.CommGettingTamer, status.info);
                        break;
                    }
            }
            LoadProgressBar.Maximum = status.max_progress;
            LoadProgressBar.Value = status.progress;
        }

        void dmo_web_DownloadCompleted(object sender, DMODownloadResultCode code, guild result)
        {
            BlockControls(false);

            dmo_web.DownloadStarted -= dmo_web_DownloadStarted;
            dmo_web.DownloadCompleted -= dmo_web_DownloadCompleted;
            dmo_web.StatusChanged -= dmo_web_StatusChanged;

            ProgressBlock.Opacity = 0;
            switch (code)
            {
                case DMODownloadResultCode.OK:
                    {
                        CURRENT_GUILD = result;
                        UpdateInfo(CURRENT_GUILD);
                        TDBlock_.ShowTamers(CURRENT_GUILD.Members);
                        break;
                    }
                case DMODownloadResultCode.CANT_GET:
                    {
                        Utils.MSG_ERROR(LanguageEnv.Strings.CantGetError);
                        break;
                    }
                case DMODownloadResultCode.DB_CONNECT_ERROR:
                    {
                        Utils.MSG_ERROR(LanguageEnv.Strings.DBConnectionError);
                        break;
                    }
                case DMODownloadResultCode.NOT_FOUND:
                    {
                        Utils.MSG_ERROR(LanguageEnv.Strings.GuildNotFoundError);
                        break;
                    }
                case DMODownloadResultCode.WEB_ACCESS_ERROR:
                    {
                        Utils.MSG_ERROR(LanguageEnv.Strings.ConnectionError);
                        break;
                    }
            }
        }

        void dmo_web_DownloadStarted(object sender)
        {
            BlockControls(true);
            LoadProgressBar.Value = 0;
            LoadProgressBar.Maximum = 100;
            LoadProgressStatus.Text = string.Empty;
            ProgressBlock.Opacity = 1;
        }

        private void GetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (isValidName(textBox_g_name.Text))
            {
                dmo_web.DownloadStarted += dmo_web_DownloadStarted;
                dmo_web.DownloadCompleted += dmo_web_DownloadCompleted;
                dmo_web.StatusChanged += dmo_web_StatusChanged;
                dmo_web.GetGuildAsync(this.Dispatcher, textBox_g_name.Text, (server)ComboBoxServer.SelectedValue, (bool)chkbox_IsDetailed.IsChecked, 1);
            }
        }

        public void BlockControls(bool block)
        {
            LauncherEnv.Settings.OnProfileLocked(block);
            textBox_g_name.IsEnabled = !block;
            ComboBoxServer.IsEnabled = !block;
            button_getinfo.IsEnabled = !block;
            chkbox_IsDetailed.IsEnabled = !block;
        }

        public void UpdateInfo(guild g)
        {
            GMaster.Text = g.Master_name;
            GRank.Text = g.Rank.ToString();
            GRep.Text = g.Rep.ToString();
            //calculating top tamer in guild
            long max = long.MaxValue;
            int index = -1;
            for (int i = 0; i < g.Members.Count; i++)
            {
                if (g.Members[i].Rank < max)
                {
                    max = g.Members[i].Rank;
                    index = i;
                }
            }
            GTop.Text = g.Members[index].Name;
            //calc total digimons
            int digi_count = 0;
            foreach (tamer t in g.Members)
                digi_count += t.Digimons.Count;
            GDCnt.Text = digi_count.ToString();
            GTCnt.Text = g.Members.Count.ToString();
        }

        #region Обработка поля ввода имени гильдии
        private void GuildName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox_g_name.Text == LanguageEnv.Strings.CommGuildName)
            {
                textBox_g_name.Foreground = Brushes.Black;
                textBox_g_name.Text = string.Empty;
            }
        }

        private void GuildName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_g_name.Text))
            {
                textBox_g_name.Foreground = Brushes.Gray;
                textBox_g_name.Text = LanguageEnv.Strings.CommGuildName;
            }
        }

        public static bool isValidName(string str)
        {
            if (str == LanguageEnv.Strings.CommGuildName)
            {
                Utils.MSG_ERROR(LanguageEnv.Strings.CommGuildNameEmpty);
                return false;
            }
            GuildNameValidationRule gvr = new GuildNameValidationRule();
            ValidationResult vr = gvr.Validate(str, new System.Globalization.CultureInfo(1, false));
            if (!vr.IsValid)
                Utils.MSG_ERROR(vr.ErrorContent.ToString());
            return vr.IsValid;
        }
        #endregion
    }
}
