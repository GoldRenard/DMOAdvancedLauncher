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

namespace AdvancedLauncher
{
    public class ResourceItemViewModel : INotifyPropertyChanged
    {
        private string _RName;
        public string  RName
        {
            get
            {
                return _RName;
            }
            set
            {
                if (value != _RName)
                {
                    _RName = value;
                    NotifyPropertyChanged("RName");
                }
            }
        }

        private uint _RID;
        public uint RID
        {
            get
            {
                return _RID;
            }
            set
            {
                if (value != _RID)
                {
                    _RID = value;
                    NotifyPropertyChanged("RID");
                }
            }
        }

        private bool _IsRID;
        public bool IsRID
        {
            get
            {
                return _IsRID;
            }
            set
            {
                if (value != _IsRID)
                {
                    _IsRID = value;
                    NotifyPropertyChanged("IsRID");
                }
            }
        }

        public ResourceItemViewModel Item
        {
            get
            {
                return this;
            }
            set {  }
        }

        private string _RPath;
        public string RPath
        {
            get
            {
                return _RPath;
            }
            set
            {
                _RPath = value;
                NotifyPropertyChanged("RPath");
            }
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