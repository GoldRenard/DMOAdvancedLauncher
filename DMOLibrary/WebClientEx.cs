// ======================================================================
// DMOLibrary
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

namespace DMOLibrary {

    public class WebClientEx : WebClient {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(WebClientEx));

        public const int DEFAULT_TIMEOUT = 3000;

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

        public WebClientEx()
            : this(DEFAULT_TIMEOUT) {
        }

        public WebClientEx(int timeout) {
            this._timeout = timeout;
            this.Encoding = System.Text.Encoding.UTF8;
            this.Proxy = (IWebProxy)null;
        }

        protected override WebRequest GetWebRequest(Uri address) {
            var result = base.GetWebRequest(address);
            result.Timeout = this._timeout;
            return result;
        }

        public static WebRequest CreateHTTPRequest(Uri url) {
            WebRequest req = HttpWebRequest.Create(url);
            req.Timeout = DEFAULT_TIMEOUT;
            return req;
        }

        public static string DownloadContent(string url) {
            return DownloadContent(url, 100);
        }

        public static string DownloadContent(string url, int tryAttempts) {
            string html = null;
            for (int i = 0; i < tryAttempts; i++) {
                using (WebClientEx webClient = new WebClientEx()) {
                    try {
                        html = webClient.DownloadString(url);
                    } catch (WebException e) {
                        LOGGER.WarnFormat("Web request for \"{0}\" caused the error: {1}", url, e.Message);
                    };
                    if (html != string.Empty && html != null) {
                        return html;
                    }
                }
            }
            return html;
        }
    }
}