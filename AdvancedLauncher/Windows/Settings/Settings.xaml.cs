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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Globalization;
using System.IO;
using Microsoft.Win32;

namespace AdvancedLauncher
{
    public partial class Settings : UserControl
    {
        Settings_DC DContext = new Settings_DC();
        Storyboard ShowWindow, HideWindow;
        string current_lang;
        ServerViewModel SERVER_DC = new ServerViewModel();
        string LINK_EAL_INSTALLING_RUS = "http://www.bolden.ru/index.php?option=com_content&task=view&id=76";
        string LINK_EAL_INSTALLING = "http://www.voom.net/install-files-for-east-asian-languages-windows-xp";
        string LINK_MS_APPLOCALE = "http://www.microsoft.com/en-us/download/details.aspx?id=2043";

        bool isAllowAL = false;
        bool isALInstalled = false;
        bool isKoreanSupported = false;

        public Settings()
        {
            InitializeComponent();
            LayoutRoot.DataContext = DContext;
            SERVER_DC.LoadData(App.DMOProfile.ServerList);
            ComboBoxServer.ItemsSource = SERVER_DC.Items;

            current_lang = SettingsProvider.TRANSLATION_FILE;
            string[] langs = LanguageProvider.GetTranslations();

            ComboBoxLanguage.Items.Add(LanguageProvider.Translation.DEF_LANG_NAME);
            foreach (string lang in langs)
                ComboBoxLanguage.Items.Add(Path.GetFileNameWithoutExtension(lang));

            ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
            HideWindow = ((Storyboard)this.FindResource("HideWindow"));
        }

        #region Check AppLocale

        public bool CheckAppLocale()
        {
            string apploc_dir = Environment.GetEnvironmentVariable("windir") + "\\apppatch\\AppLoc.exe";
            isALInstalled = File.Exists(apploc_dir);

            isKoreanSupported = false;
            CultureInfo[] cis = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures);
            foreach (CultureInfo ci in cis)
                if (ci.TwoLetterISOLanguageName == "ko")
                {
                    isKoreanSupported = true;
                    break;
                }
            if (!isALInstalled || !isKoreanSupported)
                return false;
            return true;
        }

        private void HLWhyAL_Click(object sender, RoutedEventArgs e)
        {
            string message = LanguageProvider.strings.SETTINGS_CANT_USE_APPLOC + Environment.NewLine;
            if (!isALInstalled)
                message += Environment.NewLine + LanguageProvider.strings.SETTINGS_APPLOC_ISNT_INSTALLED;

            if (!isKoreanSupported)
                message += Environment.NewLine + LanguageProvider.strings.SETTINGS_ALP_ISNT_INSTALLED;
            message += Environment.NewLine + Environment.NewLine + LanguageProvider.strings.SETTINGS_QUESTION_DOWNLOAD_NOW;

            if (MessageBoxResult.Yes == MessageBox.Show(message, LanguageProvider.strings.SETTINGS_CANT_USE_APPLOC_CAPTION, MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                if (!isALInstalled)
                    System.Diagnostics.Process.Start(LINK_MS_APPLOCALE);
                if (!isKoreanSupported)
                {
                    if (CultureInfo.CurrentCulture.Name == "ru-RU")
                        System.Diagnostics.Process.Start(LINK_EAL_INSTALLING_RUS);
                    else
                        System.Diagnostics.Process.Start(LINK_EAL_INSTALLING);
                }
            }
        }

        #endregion

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

        public void Show(bool state)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
            {
                if (state)
                {
                    LoadSettings();
                    isAllowAL = CheckAppLocale();
                    Chk_UseAppLoc.IsEnabled = isAllowAL;
                    if (!isAllowAL)
                        Chk_UseAppLoc.IsChecked = false;
                    AL_Helper.Visibility = isAllowAL ? Visibility.Collapsed : Visibility.Visible;
                    ShowWindow.Begin();
                }
                else
                    HideWindow.Begin();
            }));
        }

        public void LoadSettings()
        {
            textBox_game_path.Text = App.DMOProfile.GetGamePath();
            textBox_t_user.Text = App.DMOProfile.S_TWITTER_USER;
            Chk_UseAppLoc.IsChecked = App.DMOProfile.S_USE_APPLOC;
            Chk_UseUpdateEngine.IsChecked = App.DMOProfile.S_USE_UPDATE_ENGINE;
            textBox_g_name.Text = App.DMOProfile.S_ROTATION_GNAME;
            ComboBoxServer.SelectedIndex = App.DMOProfile.S_ROTATION_GSERV.Id - 1;
            ComboBoxURate.SelectedIndex = App.DMOProfile.S_ROTATION_URATE - 1;

            for (int i = 0; i < ComboBoxLanguage.Items.Count; i++)
            {
                if (ComboBoxLanguage.Items[i].ToString() == SettingsProvider.TRANSLATION_FILE)
                {
                    ComboBoxLanguage.SelectedIndex = i;
                    break;
                }
            }

            if (App.DMOProfile.S_FIRST_TAB == 1)
                RB_News_Twitter.IsChecked = true;
            else
                RB_News_Joymax.IsChecked = true;

            if (!App.DMOProfile.IsWebSupported)
                GuildGroupBox.Visibility = Visibility.Collapsed;

            if (!App.DMOProfile.IsNewsSupported)
            {
                RB_News_Twitter.IsChecked = true;
                RB_News_Joymax.IsChecked = false;
                RB_News_Joymax.IsEnabled = false;
            }

            if (!App.DMOProfile.IsUpdateSupported)
            {
                Chk_UseUpdateEngine.IsChecked = false;
                Chk_UseUpdateEngine.IsEnabled = false;
            }

            if (App.DMOProfile.IsSeparateLauncher)
                textBox_launcher_path.Text = App.DMOProfile.GetLauncherPath();
            else
                LauncherPathBlock.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            if (isValidName(textBox_g_name.Text))
            {
                current_lang = SettingsProvider.TRANSLATION_FILE;
                App.DMOProfile.S_ROTATION_GNAME = textBox_g_name.Text;
                App.DMOProfile.S_ROTATION_GSERV.Id = (byte)(ComboBoxServer.SelectedIndex + 1);
                App.DMOProfile.S_ROTATION_URATE = (byte)(ComboBoxURate.SelectedIndex + 1);
                App.DMOProfile.S_TWITTER_USER = textBox_t_user.Text;
                App.DMOProfile.S_USE_APPLOC = (bool)Chk_UseAppLoc.IsChecked;
                App.DMOProfile.S_USE_UPDATE_ENGINE = (bool)Chk_UseUpdateEngine.IsChecked;

                if (RB_News_Twitter.IsChecked == true)
                    App.DMOProfile.S_FIRST_TAB = 1;
                else
                    App.DMOProfile.S_FIRST_TAB = 2;

                SettingsProvider.SaveSettings();
                App.DMOProfile.WriteSettings();

                MessageBox.Show(LanguageProvider.strings.SETTINGS_NEED_RESTART, LanguageProvider.strings.SETTINGS_NEED_RESTART_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);
                Show(false);
            }
        }

        private void BtnGameBrowse_Click_1(object sender, RoutedEventArgs e)
        {
            string new_path = App.DMOProfile.SelectGameDir(true);
            if (new_path != string.Empty)
                textBox_game_path.Text = new_path;
        }

        private void BtnLauncherBrowse_Click_1(object sender, RoutedEventArgs e)
        {
            string new_path = App.DMOProfile.SelectLauncherDir(true);
            if (new_path != string.Empty)
                textBox_launcher_path.Text = new_path;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            LanguageProvider.LoadTranslation(current_lang);
            Show(false);
        }

        private void ComboBoxLanguage_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
                LanguageProvider.LoadTranslation(ComboBoxLanguage.SelectedValue.ToString());
        }

        private static RegistryKey CheckKey(RegistryKey rootKey, string keyName, bool createKey)
        {
            RegistryKey currentKey = rootKey.OpenSubKey(keyName, true);
            if (currentKey == null && createKey)
                currentKey = rootKey.CreateSubKey(keyName);
            return currentKey;
        }
    }

    public class BorderHeightConverter : System.Windows.Data.IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
                return ((double)value + 10);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
