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

namespace AdvancedLauncher
{
    public partial class Community : UserControl
    {
        Community_DC DContext = new Community_DC();
        Storyboard ShowWindow, HideWindow;
        private delegate void DoOneText(string text);
        ServerViewModel SERVER_DC = new ServerViewModel();
        DMOWebProfile dmo_web;
        guild CURRENT_GUILD = new guild() { Id = -1 };

        public Community()
        {
            InitializeComponent();
            LayoutRoot.DataContext = DContext;
            textBox_g_name.Text = LanguageProvider.strings.COMM_TB_GUILD_NAME;

            TDBlock_.TabChanged += TDBlock_TabChanged;
            SERVER_DC.LoadData(App.DMOProfile.ServerList);
            ComboBoxServer.ItemsSource = SERVER_DC.Items;

            ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
            HideWindow = ((Storyboard)this.FindResource("HideWindow"));

            dmo_web = App.DMOProfile.GetWebProfile();
            dmo_web.DownloadStarted += dmo_web_DownloadStarted;
            dmo_web.DownloadCompleted += dmo_web_DownloadCompleted;
            dmo_web.StatusChanged += dmo_web_StatusChanged;
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
                        LoadProgressStatus.Text = LanguageProvider.strings.COMM_STATUS_GUILD_SEARCHING;
                        break;
                    }
                case DMODownloadStatusCode.GETTING_TAMER:
                    {
                        LoadProgressStatus.Text = string.Format(LanguageProvider.strings.COMM_STATUS_TAMER_GETTING, status.info);
                        break;
                    }
            }
            LoadProgressBar.Maximum = status.max_progress;
            LoadProgressBar.Value = status.progress;
        }

        void dmo_web_DownloadCompleted(object sender, DMODownloadResultCode code, guild result)
        {
            BlockControls(false);
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
                        Utils.MSG_ERROR(LanguageProvider.strings.COMM_CANT_GET_GUILD);
                        break;
                    }
                case DMODownloadResultCode.DB_CONNECT_ERROR:
                    {
                        Utils.MSG_ERROR(LanguageProvider.strings.COMM_CANT_CONNECT_TO_DB);
                        break;
                    }
                case DMODownloadResultCode.NOT_FOUND:
                    {
                        Utils.MSG_ERROR(LanguageProvider.strings.COMM_GUILD_NOT_FOUND);
                        break;
                    }
                case DMODownloadResultCode.WEB_ACCESS_ERROR:
                    {
                        Utils.MSG_ERROR(LanguageProvider.strings.COMM_GUILD_NO_CONNECTION);
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
                dmo_web.GetGuildAsync(this.Dispatcher, textBox_g_name.Text, (server)ComboBoxServer.SelectedValue, (bool)chkbox_IsDetailed.IsChecked, 1);
        }

        public void Activate()
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
            {
                    ShowWindow.Begin();
            }));
        }

        public void BlockControls(bool block)
        {
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
            if (textBox_g_name.Text == LanguageProvider.strings.COMM_TB_GUILD_NAME)
            {
                textBox_g_name.Foreground = Brushes.Black;
                textBox_g_name.Text = string.Empty;
            }
        }

        private void GuildName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBox_g_name.Text == string.Empty)
            {
                textBox_g_name.Foreground = Brushes.Gray;
                textBox_g_name.Text = LanguageProvider.strings.COMM_TB_GUILD_NAME;
            }
        }

        public bool isValidName(string str)
        {
            if (str == LanguageProvider.strings.COMM_TB_GUILD_NAME)
            {
                Utils.MSG_ERROR(LanguageProvider.strings.COMM_TB_EMPTY_MSG);
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


    class GuildNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value.ToString() == LanguageProvider.strings.COMM_TB_GUILD_NAME)
                return new ValidationResult(true, null);
            int code = 0;

            if (value.ToString().IndexOfAny("(*^%@)&^@#><>!.,$|`~?:\":\\/';=-+_".ToCharArray()) != -1)
                return new ValidationResult(false, LanguageProvider.strings.COMM_TB_INCORRECT);

            foreach (char chr in value.ToString())
            {
                code = Convert.ToInt32(chr);
                //if (!((code > 96 && code < 123) || (code > 64 && code < 91) || Char.IsDigit(chr)))
                if (Char.IsWhiteSpace(chr) || Char.IsControl(chr) )
                    return new ValidationResult(false, LanguageProvider.strings.COMM_TB_INCORRECT);
            }
            return new ValidationResult(true, null);
        }
    }
}
