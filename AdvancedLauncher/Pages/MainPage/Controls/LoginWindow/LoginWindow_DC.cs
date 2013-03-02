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
  public class LoginWindow_DC : INotifyPropertyChanged
  {
    public string Button_LastSession { set; get; }
    public string TB_Login { set; get; }
    public string TB_Password { set; get; }
    public string RB_SavePass { set; get; }

    public LoginWindow_DC()
    {
      Update();
      LanguageProvider.Languagechanged += () => { Update(); };
    }

    public void Update()
    {
      TB_Login = LanguageProvider.strings.LOGIN_LOGIN;
      TB_Password = LanguageProvider.strings.LOGIN_PASSWORD;
      RB_SavePass = LanguageProvider.strings.LOGIN_SAVEPASS;
      Button_LastSession = LanguageProvider.strings.LOGIN_START_LAST_SESSION;

      NotifyPropertyChanged("TB_Login");
      NotifyPropertyChanged("TB_Password");
      NotifyPropertyChanged("RB_SavePass");
      NotifyPropertyChanged("Button_LastSession");
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
