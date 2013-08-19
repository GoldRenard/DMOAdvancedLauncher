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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using AdvancedLauncher.Service;
using AdvancedLauncher.Environment;
using System.IO;
using DMOLibrary;
using System.Globalization;
using AdvancedLauncher.Environment.Containers;

namespace AdvancedLauncher.Windows
{
    public partial class Settings : UserControl, INotifyPropertyChanged
    {
        string LINK_EAL_INSTALLING_RUS = "http://www.bolden.ru/index.php?option=com_content&task=view&id=76";
        string LINK_EAL_INSTALLING = "http://www.voom.net/install-files-for-east-asian-languages-windows-xp";
        string LINK_MS_APPLOCALE = "http://www.microsoft.com/en-us/download/details.aspx?id=2043";

        Microsoft.Win32.OpenFileDialog FileDialog = new Microsoft.Win32.OpenFileDialog() { Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png" };
        System.Windows.Forms.FolderBrowserDialog Folderdialog = new System.Windows.Forms.FolderBrowserDialog() { ShowNewFolderButton = false };

        AdvancedLauncher.Environment.Containers.Settings sContainer;
        Storyboard ShowWindow, HideWindow;
        int cLangInd;
        public Settings()
        {
            InitializeComponent();
            ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
            HideWindow = ((Storyboard)this.FindResource("HideWindow"));

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

                //Copying settings object and set it as DataContext
                ProfileList.DataContext = sContainer = new AdvancedLauncher.Environment.Containers.Settings(LauncherEnv.Settings);
                //Search and set current profile
                foreach (Profile p in ((AdvancedLauncher.Environment.Containers.Settings)ProfileList.DataContext).pCollection)
                    if (p.pId == LauncherEnv.Settings.pCurrent.pId)
                    {
                        ProfileList.SelectedItem = p;
                        break;
                    }

                //Load language list
                ComboBoxLanguage.Items.Add(LanguageEnv.DefaultName);
                foreach (string lang in LanguageEnv.GetTranslations())
                    ComboBoxLanguage.Items.Add(Path.GetFileNameWithoutExtension(lang));
                for (int i = 0; i < ComboBoxLanguage.Items.Count; i++)
                {
                    if (ComboBoxLanguage.Items[i].ToString() == LauncherEnv.Settings.LangFile)
                    {
                        ComboBoxLanguage.SelectedIndex = cLangInd = i;
                        break;
                    }
                }
            }
        }

        #region Profile Section

        public static Profile SelectedProfile;
        private void ProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProfile = (Profile)ProfileList.SelectedItem;
            ValidatePaths();
            NotifyPropertyChanged("IsSelectedNotDefault");
        }

        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidatePaths();
        }

        private void BtnSetDef_Click(object sender, RoutedEventArgs e)
        {
            sContainer.pDefault = SelectedProfile.pId;
            NotifyPropertyChanged("IsSelectedNotDefault");
        }

        public bool IsSelectedNotDefault
        {
            set { }
            get
            {
                return SelectedProfile.pId != sContainer.pDefault;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            sContainer.AddNewProfile();
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileList.Items.Count == 1)
            {
                Utils.MSG_ERROR(LanguageEnv.Strings.Settings_LastProfile);
                return;
            }
            Profile pToDel = SelectedProfile;

            if (ProfileList.SelectedIndex != ProfileList.Items.Count - 1)
                ProfileList.SelectedIndex++;
            else
                ProfileList.SelectedIndex--;

            sContainer.DeleteProfile(pToDel);
        }

        private void ImageBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Nullable<bool> result = FileDialog.ShowDialog();
            if (result == true)
                SelectedProfile.ImagePath = FileDialog.FileName;
        }

        #endregion

        #region Path Browse Section

        private void BtnGameBrowse_Click_1(object sender, RoutedEventArgs e)
        {
            Folderdialog.Description = LanguageEnv.Strings.Settings_SelectGameDir;
            while (true)
            {
                if (Folderdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (SelectedProfile.GameEnv.CheckGame(Folderdialog.SelectedPath))
                    {
                        SelectedProfile.GameEnv.GamePath = Folderdialog.SelectedPath;
                        break;
                    }
                    else
                        MessageBox.Show(LanguageEnv.Strings.Settings_SelectGameDirError, LanguageEnv.Strings.Settings_GamePath, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                    break;
            }
        }

        private void BtnLauncherBrowse_Click_1(object sender, RoutedEventArgs e)
        {
            Folderdialog.Description = LanguageEnv.Strings.Settings_SelectLauncherDir;
            while (true)
            {
                if (Folderdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (SelectedProfile.GameEnv.CheckDefLauncher(Folderdialog.SelectedPath))
                    {
                        SelectedProfile.GameEnv.DefLauncherPath = Folderdialog.SelectedPath;
                        break;
                    }
                    else
                        MessageBox.Show(LanguageEnv.Strings.Settings_SelectLauncherDirError, LanguageEnv.Strings.Settings_LauncherPath, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                    break;
            }
        }

        #endregion

        #region AppLocale Section

        public bool IsALSupported
        {
            set { }
            get { return Service.ApplicationLauncher.IsALSupported; }
        }

        public bool IsALNotSupported
        {
            set { }
            get { return !Service.ApplicationLauncher.IsALSupported; }
        }

        private void HLWhyAL_Click(object sender, RoutedEventArgs e)
        {
            if (Service.ApplicationLauncher.IsALSupported)
                return;
            string message = LanguageEnv.Strings.AppLocale_FailReasons + System.Environment.NewLine;
            if (!Service.ApplicationLauncher.IsALInstalled)
                message += System.Environment.NewLine + LanguageEnv.Strings.AppLocale_NotInstalled;

            if (!Service.ApplicationLauncher.IsKoreanSupported)
                message += System.Environment.NewLine + LanguageEnv.Strings.AppLocale_EALNotInstalled;
            message += System.Environment.NewLine + System.Environment.NewLine + LanguageEnv.Strings.AppLocale_FixQuestion;

            if (MessageBoxResult.Yes == MessageBox.Show(message, LanguageEnv.Strings.AppLocale_Error, MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                if (!Service.ApplicationLauncher.IsALInstalled)
                    System.Diagnostics.Process.Start(LINK_MS_APPLOCALE);
                if (!Service.ApplicationLauncher.IsKoreanSupported)
                {
                    if (CultureInfo.CurrentCulture.Name == "ru-RU")
                        System.Diagnostics.Process.Start(LINK_EAL_INSTALLING_RUS);
                    else
                        System.Diagnostics.Process.Start(LINK_EAL_INSTALLING);
                }
            }
        }

        #endregion

        #region Global Actions Section

        public void Show(bool state)
        {
            if (state)
                ShowWindow.Begin();
            else
                HideWindow.Begin();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxLanguage.SelectedIndex = cLangInd;
            Show(false);
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            cLangInd = ComboBoxLanguage.SelectedIndex;
            sContainer.LangFile = ComboBoxLanguage.SelectedValue.ToString();
            LauncherEnv.Settings.Merge(sContainer);
            LauncherEnv.Save();
            Show(false);
        }

        private void ComboBoxLanguage_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
                LanguageEnv.Load(ComboBoxLanguage.SelectedValue.ToString());
        }

        #endregion

        #region Service

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Utils.OpenSite(e.Uri.AbsoluteUri);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Validation

        private void ValidatePaths()
        {
            tbGamePath.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbLauncherPath.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        #endregion
    }

}