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
    public class Personalization_DC : INotifyPropertyChanged
    {
        public string Select { set; get; }
        public string SelectMessage { set; get; }
        public string Current { set; get; }
        public string Save { set; get; }
        public string Apply { set; get; }
        public string Close { set; get; }

        public Personalization_DC()
        {
            Update();
            LanguageProvider.Languagechanged += () => { Update(); };
        }

        public void Update()
        {
            Select = LanguageProvider.strings.CUST_SELECT_PICTURE;
            SelectMessage = LanguageProvider.strings.CUST_SELECT_MESSAGE;
            Current = LanguageProvider.strings.CUST_CURRENT_PICTURE;
            Save = LanguageProvider.strings.CUST_SAVE_PICTURE;
            Apply = LanguageProvider.strings.APPLY;
            Close = LanguageProvider.strings.CLOSE;

            NotifyPropertyChanged("Select");
            NotifyPropertyChanged("SelectMessage");
            NotifyPropertyChanged("Current");
            NotifyPropertyChanged("Save");
            NotifyPropertyChanged("Apply");
            NotifyPropertyChanged("Close");
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
