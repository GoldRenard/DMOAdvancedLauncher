// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Windows.Media.Animation;
using AdvancedLauncher.Environment;
using DMOLibrary;

namespace AdvancedLauncher.Controls {

    public partial class TDBlock : UserControl {
        private TamerViewModel TamerModel = new TamerViewModel();
        private DigimonViewModel DigimonModel = new DigimonViewModel();
        private bool isFullDList = false;
        private byte CurrentTab = 1;

        public delegate void ChangedEventHandler(object sender, int tab_num);

        public event ChangedEventHandler TabChanged;

        protected virtual void OnChanged(int tab_num) {
            if (TabChanged != null)
                TabChanged(this, tab_num);
        }

        public TDBlock() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                Tamers.DataContext = TamerModel;
                Digimons.DataContext = DigimonModel;
                LanguageEnv.Languagechanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
            }
        }

        public void ClearAll() {
            TamerModel.UnLoadData();
            DigimonModel.UnLoadData();

            Tamers.Visibility = Visibility.Collapsed;
            Digimons.Visibility = Visibility.Collapsed;
            Tamers.Opacity = 0;
            Digimons.Opacity = 0;

            isFullDList = false;
        }

        #region Showing tabs

        private void OnTamersShow(object sender, MouseButtonEventArgs e) {
            if (TamerModel.IsDataLoaded && Tamers.SelectedIndex >= 0) {
                TamerItemViewModel selected_item = (TamerItemViewModel)Tamers.SelectedItem;
                Tamers.SelectedIndex = -1;
                ShowDigimons(selected_item.Tamer);
            }
        }

        public void ShowTamers() {
            OnChanged(1);
            //Скрываем старую панель
            Storyboard sb = new Storyboard();
            DoubleAnimation dbl_anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(150)));
            Storyboard.SetTarget(dbl_anim, (CurrentTab == 1) ? Tamers : Digimons);
            Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
            sb.Children.Add(dbl_anim);
            sb.Completed += (s, e) => {
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

        public void ShowTamers(List<TamerOld> tamers) {
            OnChanged(1);
            //Скрываем старую панель
            Storyboard sb = new Storyboard();
            DoubleAnimation dbl_anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(150)));
            Storyboard.SetTarget(dbl_anim, (CurrentTab == 1) ? Tamers : Digimons);
            Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
            sb.Children.Add(dbl_anim);
            sb.Completed += (s, e) => {
                Tamers.Visibility = Visibility.Visible;
                Digimons.Visibility = Visibility.Collapsed;
                CurrentTab = 1;
                TamerModel.UnLoadData();
                TamerModel.LoadData(tamers);
                sb = new Storyboard();
                dbl_anim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(150)));
                Storyboard.SetTarget(dbl_anim, Tamers);
                Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
                sb.Children.Add(dbl_anim);
                sb.Begin();
            };
            sb.Begin();
        }

        public void ShowDigimons(TamerOld tamer) {
            OnChanged(2);
            //Скрываем старую панель
            Storyboard sb = new Storyboard();
            DoubleAnimation dbl_anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(150)));
            Storyboard.SetTarget(dbl_anim, (CurrentTab == 1) ? Tamers : Digimons);
            Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
            sb.Children.Add(dbl_anim);
            sb.Completed += (s, e) => {
                Tamers.Visibility = Visibility.Collapsed;
                Digimons.Visibility = Visibility.Visible;

                DigimonModel.UnLoadData();
                DigimonModel.LoadData(tamer);
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

        public void ShowDigimons(List<TamerOld> tamers) {
            OnChanged(2);
            //Скрываем старую панель
            Storyboard sb = new Storyboard();
            DoubleAnimation dbl_anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(150)));
            Storyboard.SetTarget(dbl_anim, (CurrentTab == 1) ? Tamers : Digimons);
            Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
            sb.Children.Add(dbl_anim);
            sb.Completed += (s, e) => {
                Tamers.Visibility = Visibility.Collapsed;
                Digimons.Visibility = Visibility.Visible;

                if (!isFullDList) {
                    DigimonModel.UnLoadData();
                    DigimonModel.LoadData(tamers);
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

        #endregion Showing tabs

        #region List Header Processing and translation

        private void OnTamerHeaderLoaded(object sender, RoutedEventArgs e) {
            ((TextBlock)sender).MouseLeftButtonUp += OnTamerHeaderClick;
        }

        private void OnDigimonHeaderLoaded(object sender, RoutedEventArgs e) {
            ((TextBlock)sender).MouseLeftButtonUp += OnDigimonHeaderClick;
        }

        private void OnTamerHeaderClick(object sender, RoutedEventArgs e) {
            if (sender != null) {
                if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Type) {
                    TamerModel.Sort(i => i.Tamer.TypeId);
                } else if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Name) {
                    TamerModel.Sort(i => i.TName);
                } else if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Level) {
                    TamerModel.Sort(i => i.Level);
                } else if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Ranking) {
                    TamerModel.Sort(i => i.Rank);
                } else if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Partner) {
                    TamerModel.Sort(i => i.PName);
                } else if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Mercenary) {
                    TamerModel.Sort(i => i.DCnt);
                }
            }
        }

        private void OnDigimonHeaderClick(object sender, RoutedEventArgs e) {
            if (sender != null) {
                if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Type) {
                    DigimonModel.Sort(i => i.DType);
                } else if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Name) {
                    DigimonModel.Sort(i => i.DName);
                } else if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Level) {
                    DigimonModel.Sort(i => i.Level);
                } else if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Ranking) {
                    DigimonModel.Sort(i => i.Rank);
                } else if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Tamer) {
                    DigimonModel.Sort(i => i.TName);
                } else if (((TextBlock)sender).Text == LanguageEnv.Strings.CommHeader_Size) {
                    DigimonModel.Sort(i => i.SizePC);
                }
            }
        }

        #endregion List Header Processing and translation
    }
}