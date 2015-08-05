// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2015 Ilya Egorov (goldrenard@gmail.com)

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

using System.Net;
using AdvancedLauncher.Model.Protected;
using AdvancedLauncher.Providers;

namespace AdvancedLauncher.Management.Internal {

    internal sealed class ProxyManager {

        public ProxySetting Settings {
            get;
            private set;
        }

        public void Initialize(ProxySetting Settings) {
            this.Settings = new ProxySetting(Settings);
            WebClientEx.GlobalProxy = GetProxy();
        }

        public IWebProxy GetProxy() {
            IWebProxy proxy = null;
            if (Settings != null) {
                if (Settings.IsEnabled) {
                    switch ((ProxyMode)Settings.Mode) {
                        case ProxyMode.Default:
                            proxy = WebRequest.GetSystemWebProxy();
                            break;

                        case ProxyMode.HTTP:
                            proxy = new WebProxy(string.Format("http://{0}:{1}/", Settings.Host, Settings.Port), true);
                            break;

                        case ProxyMode.HTTPS:
                            proxy = new WebProxy(Settings.Host, Settings.Port);
                            break;
                    }
                    if ((ProxyMode)Settings.Mode != ProxyMode.Default &&
                        Settings.Authentication && Settings.Credentials.IsCorrect) {
                        proxy.Credentials = new NetworkCredential(Settings.Credentials.User, Settings.Credentials.SecurePassword);
                    }
                }
            } else {
                proxy = WebRequest.GetSystemWebProxy();
            }
            return proxy;
        }
    }
}