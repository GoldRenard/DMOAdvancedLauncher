using System;
using System.Net;
using AdvancedLauncher.SDK.Management;

namespace PluginSample {

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