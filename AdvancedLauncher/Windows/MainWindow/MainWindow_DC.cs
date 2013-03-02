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
using System.Text;

namespace AdvancedLauncher
{
    public class MainWindow_DC : INotifyPropertyChanged
    {
        public string NewsTab { set; get; }
        public string GalleryTab { set; get; }
        public string CommunityTab { set; get; }
        public string CustTab { set; get; }

        public MainWindow_DC()
        {
            Update();
            LanguageProvider.Languagechanged += () => { Update(); };
        }

        public void Update()
        {

            NewsTab = LanguageProvider.strings.MAIN_NEWS_TAB;
            GalleryTab = LanguageProvider.strings.MAIN_GALLERY_TAB;
            CommunityTab = LanguageProvider.strings.MAIN_COMMUNITY_TAB;
            CustTab = LanguageProvider.strings.MAIN_CUSTOMIZATION_TAB;

            NotifyPropertyChanged("NewsTab");
            NotifyPropertyChanged("GalleryTab");
            NotifyPropertyChanged("CommunityTab");
            NotifyPropertyChanged("CustTab");
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
