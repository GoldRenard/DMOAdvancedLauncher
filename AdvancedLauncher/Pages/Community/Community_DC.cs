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
    public class Community_DC : INotifyPropertyChanged
    {
        public string ShowTab { set; get; }
        public string TamersTab { set; get; }
        public string DigimonsTab { set; get; }

        public string Select_Guild { set; get; }
        public string CheckBox_Detailed { set; get; }
        public string Button_Search { set; get; }

        public string Information { set; get; }
        public string GMaster { set; get; }
        public string GBest { set; get; }
        public string GRank { set; get; }
        public string GRep { set; get; }
        public string GTCnt { set; get; }
        public string GDCnt { set; get; }

        public Community_DC()
        {
            Update();
            LanguageProvider.Languagechanged += () => { Update(); };
        }

        public void Update()
        {

            ShowTab = LanguageProvider.strings.COMM_SHOWTAB;
            TamersTab = LanguageProvider.strings.COMM_TAMERS_TAB;
            DigimonsTab = LanguageProvider.strings.COMM_DIGIMONS_TAB;

            Select_Guild = LanguageProvider.strings.COMM_SELECT_GUILD;
            CheckBox_Detailed = LanguageProvider.strings.COMM_CB_DETAILED;
            Button_Search = LanguageProvider.strings.COMM_BTN_SEARCH;

            Information = LanguageProvider.strings.COMM_INFO;
            GMaster = LanguageProvider.strings.COMM_GMASTER;
            GBest = LanguageProvider.strings.COMM_GBEST;
            GRank = LanguageProvider.strings.COMM_GRANK;
            GRep = LanguageProvider.strings.COMM_GREP;
            GTCnt = LanguageProvider.strings.COMM_TAMER_COUNT;
            GDCnt = LanguageProvider.strings.COMM_DIGIMON_COUNT;

            NotifyPropertyChanged("ShowTab");
            NotifyPropertyChanged("TamersTab");
            NotifyPropertyChanged("DigimonsTab");

            NotifyPropertyChanged("Select_Guild");
            NotifyPropertyChanged("CheckBox_Detailed");
            NotifyPropertyChanged("Button_Search");

            NotifyPropertyChanged("Information");
            NotifyPropertyChanged("GMaster");
            NotifyPropertyChanged("GBest");
            NotifyPropertyChanged("GRank");
            NotifyPropertyChanged("GRep");
            NotifyPropertyChanged("GTCnt");
            NotifyPropertyChanged("GDCnt");
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
