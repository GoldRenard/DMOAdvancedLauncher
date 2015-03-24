// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Net;
using System.Security;

namespace DMOLibrary {

    public class ProxyConfiguration {
        private const string HTTP_FORMAT = "http://{0}:{1}/";

        private readonly ProxyMode Mode;
        private readonly string Host;
        private readonly ushort Port;
        private readonly string Login;
        private readonly SecureString Password;
        private readonly bool Authentication;

        public ProxyConfiguration(ProxyMode mode, string host, ushort port)
            : this(mode, host, port, null, null, false) {
        }

        public ProxyConfiguration(ProxyMode mode, string host, ushort port, string login, SecureString password, bool Authentication = true) {
            this.Mode = mode;
            this.Host = host;
            this.Port = port;
            this.Login = login;
            this.Password = password;
            this.Authentication = Authentication;
        }

        public bool IsDefault {
            get {
                return Mode == ProxyMode.Default;
            }
        }

        public WebProxy GetProxy() {
            WebProxy proxy;
            if (Mode == ProxyMode.HTTP) {
                proxy = new WebProxy(string.Format(HTTP_FORMAT, Host, Port), true);
            } else if (Mode == ProxyMode.HTTPS) {
                proxy = new WebProxy(Host, Port);
            } else {
                throw new NotImplementedException("Unknown proxy type");
            }
            if (Authentication) {
                proxy.Credentials = new NetworkCredential(Login, Password);
            }
            return proxy;
        }
    }
}