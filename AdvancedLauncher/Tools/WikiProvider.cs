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

using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvancedLauncher.Providers;
using Newtonsoft.Json.Linq;

namespace AdvancedLauncher.Tools {

    public class WikiProvider {
        private const string WIKI_URL = "http://dmowiki.com/";

        private const string WIKI_API_URL = "api.php?action={0}";

        private const string WIKI_SEARCH_URL = "index.php?title=Special:Search&search={0}&go=Go";

        public class Suggestion {

            public string Value {
                get; set;
            }
        }

        public string URL {
            get {
                return WIKI_URL;
            }
        }

        public string API {
            get {
                return URL + WIKI_API_URL;
            }
        }

        public string Search {
            get {
                return URL + WIKI_SEARCH_URL;
            }
        }

        public string InvokeAPI(string action, Dictionary<string, string> parameters) {
            var url = string.Format(API + "&{1}", action,
            string.Join("&",
                parameters.Select(kvp =>
                    string.Format("{0}={1}", HttpUtility.UrlEncode(kvp.Key), HttpUtility.UrlEncode(kvp.Value)))));
            return WebClientEx.DownloadContent(url, 1, 1500);
        }

        public List<string> OpenSearch(string value) {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("search", value);
            parameters.Add("format", "json");
            try {
                string result = InvokeAPI("opensearch", parameters);
                JArray content = JArray.Parse(System.Web.HttpUtility.HtmlDecode(result));
                if (content != null) {
                    if (content.Count > 1) {
                        var suggestions = content[1];
                        if (suggestions.HasValues) {
                            return suggestions.Values().Select(p => p.ToObject<string>()).ToList();
                        }
                    }
                }
            } catch { }
            return null;
        }
    }
}