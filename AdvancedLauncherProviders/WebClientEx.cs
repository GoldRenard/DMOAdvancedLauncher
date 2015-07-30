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

using System;
using System.Net;
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.Providers {

    public class WebClientEx : WebClient {
        public const int DEFAULT_TIMEOUT = 3000;

        private int? _timeout;

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int? Timeout {
            get {
                return _timeout;
            }
            set {
                _timeout = value;
            }
        }

        public WebClientEx()
            : this(null) {
        }

        public static IWebProxy GlobalProxy {
            get;
            set;
        }

        public WebClientEx(int? timeout) {
            this._timeout = timeout;
            this.Encoding = System.Text.Encoding.UTF8;
            this.Proxy = GlobalProxy;
        }

        protected override WebRequest GetWebRequest(Uri address) {
            var result = base.GetWebRequest(address);
            if (_timeout != null) {
                result.Timeout = this._timeout.Value;
            }
            return result;
        }

        public static WebRequest CreateHTTPRequest(Uri url, int? timeOut = null) {
            WebRequest req = HttpWebRequest.Create(url);
            if (timeOut != null) {
                req.Timeout = timeOut.Value;
            }
            req.Proxy = GlobalProxy;
            return req;
        }

        public static string DownloadContent(string url, int? timeOut = null) {
            return DownloadContent(url, 100, timeOut);
        }

        public static string DownloadContent(string url, int tryAttempts, int? timeOut) {
            return DownloadContent(null, url, tryAttempts, timeOut);
        }

        public static string DownloadContent(ILogManager logManager, string url, int? timeOut = null) {
            return DownloadContent(logManager, url, 100, timeOut);
        }

        public static string DownloadContent(ILogManager logManager, string url, int tryAttempts, int? timeOut) {
            Exception exception = null;
            for (int i = 0; i < tryAttempts; i++) {
                using (WebClientEx webClient = timeOut != null ? new WebClientEx(timeOut.Value) : new WebClientEx()) {
                    try {
                        return webClient.DownloadString(url);
                    } catch (WebException e) {
                        exception = e;
                        if (logManager != null) {
                            logManager.WarnFormat("Web request for \"{0}\" caused the error: {1}", url, e.Message);
                        }
                    };
                }
            }
            throw exception;
        }
    }
}