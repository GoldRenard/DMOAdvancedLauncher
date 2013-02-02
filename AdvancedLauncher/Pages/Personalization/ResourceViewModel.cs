// ======================================================================
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using DMOLibrary.DMOWebInfo;


namespace AdvancedLauncher
{
    public class ResourceViewModel : INotifyPropertyChanged
    {
        public ResourceViewModel()
        {
            this.Items = new ObservableCollection<ResourceItemViewModel>();
        }

        public ObservableCollection<ResourceItemViewModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData(List<ResourceItemViewModel> List)
        {
            this.IsDataLoaded = true;
            foreach (ResourceItemViewModel item in List)
                this.Items.Add(item);
        }

        public void AddData(ResourceItemViewModel item)
        {
            this.IsDataLoaded = true;
            this.Items.Add(item);
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