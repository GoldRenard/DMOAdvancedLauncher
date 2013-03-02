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
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;


namespace AdvancedLauncher
{
    public class TwitterViewModel : INotifyPropertyChanged
    {
        public TwitterViewModel()
        {
            this.Items = new ObservableCollection<TwitterItemViewModel>();
        }

        public ObservableCollection<TwitterItemViewModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData(List<TwitterItemViewModel> List)
        {
            this.IsDataLoaded = true;
            foreach (TwitterItemViewModel item in List)
            {
                this.Items.Add(new TwitterItemViewModel { Title = item.Title, Date = item.Date, Image = item.Image });
            }
        }

        public void UnLoadData()
        {
            this.IsDataLoaded = false;
            this.Items.Clear();
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
    }
}