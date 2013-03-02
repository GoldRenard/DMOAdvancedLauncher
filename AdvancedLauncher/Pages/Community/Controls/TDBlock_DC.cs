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

namespace AdvancedLauncher
{
    public class TDBlock_DC : INotifyPropertyChanged
    {
        public string Type { set; get; }
        public string Level { set; get; }
        public string Ranking { set; get; }
        public string Tamer { set; get; }
        public string Partner { set; get; }
        public string Mercenary { set; get; }
        public string Name { set; get; }
        public string Size { set; get; }

        public TDBlock_DC()
        {
            Update();
            LanguageProvider.Languagechanged += () => { Update(); };
        }

        public void Update()
        {
            Type = LanguageProvider.strings.COMM_LHEADER_TYPE;
            Level = LanguageProvider.strings.COMM_LHEADER_LEVEL;
            Ranking = LanguageProvider.strings.COMM_LHEADER_RANKING;
            Tamer = LanguageProvider.strings.COMM_LHEADER_TAMER;
            Partner = LanguageProvider.strings.COMM_LHEADER_PARTNER;
            Mercenary = LanguageProvider.strings.COMM_LHEADER_MERCENARY;
            Name = LanguageProvider.strings.COMM_LHEADER_NAME;
            Size = LanguageProvider.strings.COMM_LHEADER_SIZE;

            NotifyPropertyChanged("Type");
            NotifyPropertyChanged("Level");
            NotifyPropertyChanged("Ranking");
            NotifyPropertyChanged("Tamer");
            NotifyPropertyChanged("Partner");
            NotifyPropertyChanged("Mercenary");
            NotifyPropertyChanged("Name");
            NotifyPropertyChanged("Size");
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
