﻿// ======================================================================
// GLOBAL DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using DMOLibrary.DMOWebInfo;

namespace AdvancedLauncher
{
    public partial class TDBlock : UserControl
    {
        TDBlock_DC DContext = new TDBlock_DC();
        TamerViewModel Tamer_DC = new TamerViewModel();
        DigimonViewModel Digimon_DC = new DigimonViewModel();

        bool isFullDList = false;
        byte CurrentTab = 1;

        public delegate void ChangedEventHandler(object sender, int tab_num);
        public event ChangedEventHandler TabChanged;
        protected virtual void OnChanged(int tab_num)
        {
            if (TabChanged != null)
                TabChanged(this, tab_num);
        }

        public TDBlock()
        {
            InitializeComponent();
            Tamers.DataContext = Tamer_DC;
            Digimons.DataContext = Digimon_DC;
        }

        public void ClearAll()
        {
            Tamer_DC.UnLoadData();
            Digimon_DC.UnLoadData();
            isFullDList = false;
        }


        #region Showing tabs

        private void Tamers_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (Tamer_DC.IsDataLoaded && Tamers.SelectedIndex >= 0)
            {
                TamerItemViewModel selected_item = (TamerItemViewModel)Tamers.SelectedItem;
                Tamers.SelectedIndex = -1;
                ShowDigimons(selected_item.Tamer);
            }
        }

        public void ShowTamers()
        {
            OnChanged(1);
            //Скрываем старую панель
            Storyboard sb = new Storyboard();
            DoubleAnimation dbl_anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(150)));
            Storyboard.SetTarget(dbl_anim, (CurrentTab == 1) ? Tamers : Digimons);
            Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
            sb.Children.Add(dbl_anim);
            sb.Completed += (s, e) =>
            {
                Tamers.Visibility = Visibility.Visible;
                Digimons.Visibility = Visibility.Collapsed;
                CurrentTab = 1;
                sb = new Storyboard();
                dbl_anim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(150)));
                Storyboard.SetTarget(dbl_anim, Tamers);
                Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
                sb.Children.Add(dbl_anim);
                sb.Begin();
            };
            sb.Begin();
        }

        public void ShowTamers(List<tamer> tamers)
        {
            OnChanged(1);
            //Скрываем старую панель
            Storyboard sb = new Storyboard();
            DoubleAnimation dbl_anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(150)));
            Storyboard.SetTarget(dbl_anim, (CurrentTab == 1) ? Tamers : Digimons);
            Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
            sb.Children.Add(dbl_anim);
            sb.Completed += (s, e) =>
            {
                Tamers.Visibility = Visibility.Visible;
                Digimons.Visibility = Visibility.Collapsed;
                CurrentTab = 1;
                Tamer_DC.UnLoadData();
                Tamer_DC.LoadData(tamers);
                sb = new Storyboard();
                dbl_anim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(150)));
                Storyboard.SetTarget(dbl_anim, Tamers);
                Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
                sb.Children.Add(dbl_anim);
                sb.Begin();
            };
            sb.Begin();
        }

        public void ShowDigimons(tamer tamer)
        {
            OnChanged(2);
            //Скрываем старую панель
            Storyboard sb = new Storyboard();
            DoubleAnimation dbl_anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(150)));
            Storyboard.SetTarget(dbl_anim, (CurrentTab == 1) ? Tamers : Digimons);
            Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
            sb.Children.Add(dbl_anim);
            sb.Completed += (s, e) =>
            {
                Tamers.Visibility = Visibility.Collapsed;
                Digimons.Visibility = Visibility.Visible;

                Digimon_DC.UnLoadData();
                Digimon_DC.LoadData(tamer);
                isFullDList = false;

                CurrentTab = 2;
                sb = new Storyboard();
                dbl_anim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(150)));
                Storyboard.SetTarget(dbl_anim, Digimons);
                Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
                sb.Children.Add(dbl_anim);
                sb.Begin();
            };
            sb.Begin();
        }

        public void ShowDigimons(List<tamer> tamers)
        {
            OnChanged(2);
            //Скрываем старую панель
            Storyboard sb = new Storyboard();
            DoubleAnimation dbl_anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(150)));
            Storyboard.SetTarget(dbl_anim, (CurrentTab == 1) ? Tamers : Digimons);
            Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
            sb.Children.Add(dbl_anim);
            sb.Completed += (s, e) =>
            {
                Tamers.Visibility = Visibility.Collapsed;
                Digimons.Visibility = Visibility.Visible;

                if (!isFullDList)
                {
                    Digimon_DC.UnLoadData();
                    Digimon_DC.LoadData(tamers);
                    isFullDList = true;
                }
                CurrentTab = 2;
                sb = new Storyboard();
                dbl_anim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(150)));
                Storyboard.SetTarget(dbl_anim, Digimons);
                Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
                sb.Children.Add(dbl_anim);
                sb.Begin();
            };
            sb.Begin();
        }

        #endregion

        #region List Header Processing and translation

        private void TamerHeader_Loaded_1(object sender, RoutedEventArgs e)
        {
            ((TextBlock)sender).MouseLeftButtonUp += TamerHeader_Click_1;
            ((TextBlock)sender).DataContext = DContext;
        }

        private void DigimonHeader_Loaded_1(object sender, RoutedEventArgs e)
        {
            ((TextBlock)sender).MouseLeftButtonUp += DigimonHeader_Click_1;
            ((TextBlock)sender).DataContext = DContext;
        }

        private void TamerHeader_Click_1(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_TYPE)
                    Tamer_DC.Sort(i => i.Tamer.Type_id);
                else if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_NAME)
                    Tamer_DC.Sort(i => i.TName);
                else if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_LEVEL)
                    Tamer_DC.Sort(i => i.Level);
                else if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_RANKING)
                    Tamer_DC.Sort(i => i.Rank);
                else if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_PARTNER)
                    Tamer_DC.Sort(i => i.PName);
                else if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_MERCENARY)
                    Tamer_DC.Sort(i => i.DCnt);
            }
        }

        private void DigimonHeader_Click_1(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_TYPE)
                    Digimon_DC.Sort(i => i.DType);
                else if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_NAME)
                    Digimon_DC.Sort(i => i.DName);
                else if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_LEVEL)
                    Digimon_DC.Sort(i => i.Level);
                else if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_RANKING)
                    Digimon_DC.Sort(i => i.Rank);
                else if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_TAMER)
                    Digimon_DC.Sort(i => i.TName);
                else if (((TextBlock)sender).Text == LanguageProvider.strings.COMM_LHEADER_SIZE)
                    Digimon_DC.Sort(i => i.SizePC);
            }
        }

        #endregion
    }
}
