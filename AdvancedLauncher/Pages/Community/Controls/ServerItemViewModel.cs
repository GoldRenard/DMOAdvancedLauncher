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
using System.Windows;
using DMOLibrary.DMOWebInfo;

namespace AdvancedLauncher
{
    public class ServerItemViewModel : INotifyPropertyChanged
    {
        private string _SName;
        public string  SName
        {
            get
            {
                return _SName;
            }
            set
            {
                if (value != _SName)
                {
                    _SName = value;
                    NotifyPropertyChanged("SName");
                }
            }
        }

        private server _Server;
        public server Server
        {
            get
            {
                return _Server;
            }
            set
            {
                _Server = value;
                NotifyPropertyChanged("Server");
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