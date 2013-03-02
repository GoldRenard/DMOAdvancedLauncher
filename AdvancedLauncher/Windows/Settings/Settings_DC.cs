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
    public class Settings_DC : INotifyPropertyChanged
    {
        public string Language { set; get; }
        public string RotationGuild { set; get; }
        public string RotUpdateFreq { set; get; }

        public string NewsSettings { set; get; }
        public string TwitterUser { set; get; }
        public string StartNewsTab { set; get; }

        public string GameSettings { set; get; }
        public string GamePath { set; get; }
        public string LauncherPath { set; get; }
        public string UseGameUpdater { set; get; }
        public string UseAL { set; get; }
        public string ALHelp { set; get; }

        public string Apply { set; get; }
        public string Cancel { set; get; }
        public string Browse { set; get; }

        Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public Settings_DC()
        {
            Update();
            LanguageProvider.Languagechanged += () => { Update(); };
        }

        public void Update()
        {

            Language = LanguageProvider.strings.SETTINGS_LANG;
            RotationGuild = LanguageProvider.strings.SETTINGS_ROTATION_GUILD;
            RotUpdateFreq = LanguageProvider.strings.SETTINGS_ROT_UPD_FREQ;

            NewsSettings = LanguageProvider.strings.SETTINGS_NEWS;
            TwitterUser = LanguageProvider.strings.SETTINGS_TWITTER_USER;
            StartNewsTab = LanguageProvider.strings.SETTINGS_START_NEWS;

            GameSettings = LanguageProvider.strings.SETTINGS_GAME;
            GamePath = LanguageProvider.strings.SETTINGS_GAME_PATH;
            LauncherPath = LanguageProvider.strings.SETTINGS_LAUNCHER_PATH;
            UseGameUpdater = LanguageProvider.strings.SETTINGS_USE_UPDATER + " (beta)";
            UseAL = LanguageProvider.strings.SETTINGS_USE_APPLOCALE;
            ALHelp = LanguageProvider.strings.SETTINGS_APPLOCALE_HELP;

            Apply = LanguageProvider.strings.APPLY;
            Cancel = LanguageProvider.strings.CANCEL;
            Browse = LanguageProvider.strings.BROWSE;

            NotifyPropertyChanged("Language");
            NotifyPropertyChanged("RotationGuild");
            NotifyPropertyChanged("RotUpdateFreq");

            NotifyPropertyChanged("NewsSettings");
            NotifyPropertyChanged("TwitterUser");
            NotifyPropertyChanged("StartNewsTab");

            NotifyPropertyChanged("GameSettings");
            NotifyPropertyChanged("GamePath");
            NotifyPropertyChanged("LauncherPath");
            NotifyPropertyChanged("UseGameUpdater");
            NotifyPropertyChanged("UseAL");
            NotifyPropertyChanged("ALHelp");

            NotifyPropertyChanged("Browse");
            NotifyPropertyChanged("Apply");
            NotifyPropertyChanged("Cancel");
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
