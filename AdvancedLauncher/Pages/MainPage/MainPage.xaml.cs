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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AdvancedLauncher
{
    public partial class MainPage : UserControl
    {
        MainPage_DC DContext = new MainPage_DC();
        bool isPageLoaded = false;
        Storyboard ShowWindow;
        private delegate void DoChangeTextNBool(string text, bool bool_);

        public MainPage()
        {
            InitializeComponent();
            LayoutRoot.DataContext = DContext;
            ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
        }

        public void Activate()
        {
            ShowWindow.Begin();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            //Prevent secondary handle on tab changing
            if (isPageLoaded)
                return;
            isPageLoaded = true;

            App.DMOProfile.GameStartCompleted += DMOProfile_GameStartCompleted;
            Updater.UpdateCompleted += Updater_UpdateCompleted;
            Updater.UpdateStarted += Updater_UpdateStarted;
            Updater.UpdateFailed += Updater_UpdateFailed;
            Updater.DefaultUpdateRequired += Updater_DefaultUpdateRequired;
            Updater.CheckUpdates(this.Dispatcher);

            NewsBlock_.twitter_json_url = App.DMOProfile.S_TWITTER_JSON;
            NewsBlock_.TabChanged += NewsTabCnagned;
            if (App.DMOProfile.IsNewsSupported)
            {
                NewsBlock_.ShowTab(App.DMOProfile.S_FIRST_TAB, false);
                if (App.DMOProfile.S_FIRST_TAB == 1)
                    Twitter.IsEnabled = false;
                else
                    Joymax.IsEnabled = false;
            }
            else
            {
                NewsBlock_.ShowTab(1, false);
                Twitter.IsEnabled = false;
                Joymax.IsEnabled = false;
                Joymax.Visibility = Visibility.Collapsed;
            }

            if (App.DMOProfile.IsWebSupported)
                DigiRotator.InitializeRotation();
            else
                DigiRotator.InitializeStaticRotation();
        }

        public void CloseApp()
        {
            if (LoginForm.Visibility == System.Windows.Visibility.Visible)
                ((Storyboard)LayoutRoot.FindResource("HideLoginForm")).Begin();
            StartButton.Content = LanguageProvider.strings.MAIN_START_WAITING;
            StartButton.IsEnabled = false;
            BackgroundWorker queue_worker = new BackgroundWorker();
            queue_worker.DoWork += (s, e) =>
            {
                while (!DigiRotator.isLoaded) { System.Threading.Thread.Sleep(1000); };
            };
            queue_worker.RunWorkerCompleted += (s, e) =>
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() { Application.Current.Shutdown(); }));
                else
                    Application.Current.Shutdown();
            };
            queue_worker.RunWorkerAsync();
        }

        #region Обработка интерфейса

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.DMOProfile.IsLoginRequired)
                App.DMOProfile.GameStart();
            else
            {
                if (LoginForm.Visibility == System.Windows.Visibility.Collapsed)
                {
                    LoginForm.Update();
                    ((Storyboard)LayoutRoot.FindResource("ShowLoginForm")).Begin();
                }
                else
                {
                    if (LoginForm.GameStart())
                        StartButton.IsEnabled = false;
                }
            }
        }

        void DMOProfile_GameStartCompleted(object sender, DMOLibrary.LoginCode code, string result)
        {
            StartButton.IsEnabled = true;
            switch (code)
            {
                case DMOLibrary.LoginCode.SUCCESS:
                    {
                        CloseApp();
                        break;
                    }
            }
        }

        private void NewsTabCnagned(object sender, int tab)
        {
            Twitter.IsEnabled = tab == 1 ? false : true;
            Joymax.IsEnabled = tab == 1 ? true : false;
        }

        private void Twitter_Click(object sender, RoutedEventArgs e)
        {
            NewsBlock_.ShowTab(1, true);
        }

        private void Joymax_Click(object sender, RoutedEventArgs e)
        {
            NewsBlock_.ShowTab(2, true);
        }

        void Updater_DefaultUpdateRequired(object sender)
        {
            App.DMOProfile.IsUpdateNeeded = true;
            StartButton.IsEnabled = true;
            DContext.SetButtonText(LanguageProvider.strings.MAIN_UPDATE_GAME);
        }

        void Updater_UpdateFailed(object sender)
        {
            StartButton.Visibility = System.Windows.Visibility.Visible;
            Updater.Visibility = System.Windows.Visibility.Collapsed;
        }

        void Updater_UpdateStarted(object sender)
        {
            StartButton.Visibility = System.Windows.Visibility.Collapsed;
            Updater.Visibility = System.Windows.Visibility.Visible;
        }

        void Updater_UpdateCompleted(object sender)
        {
            StartButton.Visibility = System.Windows.Visibility.Visible;
            Updater.Visibility = System.Windows.Visibility.Collapsed;
            StartButton.IsEnabled = true;
        }



        #endregion Обработка интерфейса
    }
}