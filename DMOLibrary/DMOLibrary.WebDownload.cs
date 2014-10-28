// ======================================================================
// DMOLibrary
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
using System.Net;

namespace DMOLibrary {

    public class WebDownload : WebClient {
        private int _timeout;

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout {
            get {
                return _timeout;
            }
            set {
                _timeout = value;
            }
        }

        public WebDownload() {
            this._timeout = 60000;
        }

        public WebDownload(int timeout) {
            this._timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address) {
            var result = base.GetWebRequest(address);
            result.Timeout = this._timeout;
            return result;
        }

        public static string GetHTML(string url) {
            string html = string.Empty;
            for (int i = 1; i < 100; i++) {
                html = string.Empty;
                WebDownload wd = new WebDownload();
                wd.Encoding = System.Text.Encoding.UTF8;
                wd.Proxy = (IWebProxy)null;
                wd.Timeout = 3000;
                try {
                    html = wd.DownloadString(url);
                } catch {
                };
                if (html != string.Empty && html != null) {
                    return html;
                }
            }
            return string.Empty;
        }
    }
}