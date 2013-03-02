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
using System.Windows.Media.Animation;

namespace AdvancedLauncher
{
    public partial class LoginWindow : UserControl
    {
        LoginWindow_DC DContext = new LoginWindow_DC();
        Storyboard ShowWindow, HideWindow;

        public LoginWindow()
        {
            InitializeComponent();
            LayoutRoot.DataContext = DContext;
            ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
            HideWindow = ((Storyboard)this.FindResource("HideWindow"));
            App.DMOProfile.GameStartCompleted += DMOProfile_GameStartCompleted;
            App.DMOProfile.LoginStateChanged += DMOProfile_LoginStateChanged;
        }

        void DMOProfile_LoginStateChanged(object sender, DMOLibrary.LoginState state, int try_num, int last_error)
        {
            if (state == DMOLibrary.LoginState.LOGINNING)
                tb_status.Text = LanguageProvider.strings.LOGIN_LOGINNING;
            else if (state == DMOLibrary.LoginState.GETTING_DATA)
                tb_status.Text = LanguageProvider.strings.LOGIN_GETTING_DATA;
            tb_try.Text = string.Format(LanguageProvider.strings.LOGIN_TRY_TEXT, try_num);
            if (last_error != -1)
                tb_try.Text += string.Format(" " + LanguageProvider.strings.LOGIN_WAS_ERROR, last_error);
        }

        void DMOProfile_GameStartCompleted(object sender, DMOLibrary.LoginCode code, string result)
        {
            if (code != DMOLibrary.LoginCode.SUCCESS)
                Block(false);
            switch (code)
            {
                case DMOLibrary.LoginCode.SUCCESS:
                    {
                        App.DMOProfile.WriteSettings();
                        break;
                    }
                case DMOLibrary.LoginCode.WRONG_USER:
                    {
                        ShowInfoBlock(LanguageProvider.strings.LOGIN_BAD_LOGIN_PW);
                        break;
                    }
                case DMOLibrary.LoginCode.EXECUTE_ERROR:
                    {
                        Utils.MSG_ERROR(LanguageProvider.strings.APPLAUNCHER_CANT_EXECURE);
                        break;
                    }
                case DMOLibrary.LoginCode.UNKNOWN_URL:
                    {
                        ShowInfoBlock(LanguageProvider.strings.LOGIN_CANT_LOGIN + " [1]");
                        break;
                    }
                case DMOLibrary.LoginCode.WRONG_PAGE:
                    {
                        ShowInfoBlock(LanguageProvider.strings.LOGIN_CANT_LOGIN + " [2]");
                        break;
                    }
            }
        }

        public bool GameStart()
        {
            if ((bool)cb_lastsession.IsChecked)
            {
                App.DMOProfile.LastSessionStart();
                return true;
            }

            if (tb_login.Text.Length == 0)
            {
                ShowInfoBlock(LanguageProvider.strings.LOGIN_EMPTY_LOGIN);
                return false;
            }

            if (pb_password.Password.Length == 0)
            {
                ShowInfoBlock(LanguageProvider.strings.LOGIN_EMPTY_PASSWORD);
                return false;
            }

            Block(true);
            App.DMOProfile.USER_ID = tb_login.Text;
            App.DMOProfile.USER_PASSWORD = pb_password.Password;
            App.DMOProfile.RememberPassword = (bool)cb_save.IsChecked;
            App.DMOProfile.GameStart();
            return true;
        }

        private void pb_password_PasswordChanged_1(object sender, RoutedEventArgs e)
        {
            if (pb_password.Password.Length == 0)
                PW_Watermark.Visibility = System.Windows.Visibility.Visible;
            else
                PW_Watermark.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Update()
        {
            tb_try.Text = tb_status.Text = string.Empty;
            tb_login.Text = App.DMOProfile.USER_ID;
            pb_password.Password = App.DMOProfile.USER_PASSWORD;
            cb_save.IsChecked = App.DMOProfile.RememberPassword;
            cb_lastsession.IsEnabled = (App.DMOProfile.LastSessionArgs.Length > 0);
        }

        private void ShowInfoBlock(string text)
        {
            InfoBorder.Visibility = System.Windows.Visibility.Visible;
            InfoBorderText.Text = text;
        }

        private void Block(bool state)
        {
            LoaderIcon.IsEnabled = state;
            if (state)
                ((Storyboard)LayoutRoot.FindResource("ShowWaitingFrame")).Begin();
            else
                ((Storyboard)LayoutRoot.FindResource("HideWaitingFrame")).Begin();
        }
    }
}
