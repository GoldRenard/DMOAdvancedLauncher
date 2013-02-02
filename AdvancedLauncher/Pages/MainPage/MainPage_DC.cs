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

namespace AdvancedLauncher
{
    public class MainPage_DC : INotifyPropertyChanged
    {
        public string Button_StartGame { set; get; }
        public string ShowNewsTab { set; get; }

        public MainPage_DC()
        {
            Update();
            LanguageProvider.Languagechanged += () => { Update(); };
        }

        public void Update()
        {
            Button_StartGame = LanguageProvider.strings.MAIN_START_GAME;
            ShowNewsTab = LanguageProvider.strings.MAIN_SHOW_TAB;

            NotifyPropertyChanged("Button_StartGame");
            NotifyPropertyChanged("ShowNewsTab");
        }

        public void SetButtonText(string text)
        {
            Button_StartGame = text;
            NotifyPropertyChanged("Button_StartGame");
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
