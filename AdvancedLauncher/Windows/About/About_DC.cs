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
    public class About_DC : INotifyPropertyChanged
    {
        public string Version { set; get; }
        public string Developer { set; get; }
        public string Designer { set; get; }
        public string Projects { set; get; }
        public string CloseBtn { set; get; }

        Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public About_DC()
        {
            Update();
            LanguageProvider.Languagechanged += () => { Update(); };
        }

        static string separator = ": ";
        public void Update()
        {

            Version = LanguageProvider.strings.ABOUT_VERSION + separator + version.Major.ToString() + "." + version.Minor.ToString() + " (build " + version.Build.ToString() + ")";
            Developer = LanguageProvider.strings.ABOUT_DEV + separator;
            Designer = LanguageProvider.strings.ABOUT_DES + separator;
            Projects = LanguageProvider.strings.ABOUT_PROJECTS + separator;
            CloseBtn = LanguageProvider.strings.CLOSE;

            NotifyPropertyChanged("Version");
            NotifyPropertyChanged("Developer");
            NotifyPropertyChanged("Designer");
            NotifyPropertyChanged("Projects");
            NotifyPropertyChanged("CloseBtn");
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
