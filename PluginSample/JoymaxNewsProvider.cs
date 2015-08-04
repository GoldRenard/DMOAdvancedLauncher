using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;
using AdvancedLauncher.SDK.Model.Web;
using HtmlAgilityPack;

namespace PluginSample {

    public class JoymaxNewsProvider : AbstractNewsProvider {
        private static string STR_URL_NEW_PAGE = "http://www.joymax.com/dmo/Property_Front.aspx?workurl=http://dmocp.joymax.com{0}";
        private static string STR_DATE_FORMAT_REGEX = "(\\d\\d)(-)(\\d\\d)(-)(\\d\\d)";

        public JoymaxNewsProvider(ILogManager logManager) : base(logManager) {
        }

        public override List<NewsItem> GetNews() {
            LogManager.Info("Getting JoyMax news...");

            HtmlDocument doc = new HtmlDocument();
            List<NewsItem> news = new List<NewsItem>();

            HtmlNodeCollection newsNode = null;
            int tryCount = 5;
            while (newsNode == null && tryCount > 0) {
                string html = WebClientEx.DownloadContent(LogManager, "http://dmocp.joymax.com/Main/HomeMain.aspx", 5000);
                doc.LoadHtml(html);
                newsNode = doc.DocumentNode.SelectNodes("//div[@class='news-list']/ul/li");
                tryCount--;
            }

            if (newsNode == null) {
                return null;
            }

            HtmlNode newsWrap = newsNode[0];
            HtmlNodeCollection newsList = doc.DocumentNode.SelectNodes("//div[@class='news-list']/ul/li");
            NewsItem ni;

            if (newsList != null) {
                for (int i = 0; i <= newsList.Count - 1; i++) {
                    ni = new NewsItem();
                    ni.Mode = newsWrap.SelectNodes("//div[@class='lead']/span[contains(@class, 'mode')]")[i].InnerText;
                    ni.Subject = newsWrap.SelectNodes("//div[@class='lead']/span[@class='subj']")[i].InnerText;
                    ni.Date = newsWrap.SelectNodes("//div[@class='lead']/span[@class='date']")[i].InnerText;

                    Regex r = new Regex(STR_DATE_FORMAT_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match m = r.Match(ni.Date);
                    if (m.Success) {
                        ni.Date = m.Groups[3].ToString() + "." + m.Groups[1].ToString() + "." + m.Groups[5].ToString();
                    }

                    foreach (HtmlAttribute atr in newsWrap.SelectNodes("//div[@class='view']/div[@class='btn-right']/span[@class='read-more']/a")[i].Attributes) {
                        if (atr.Name == "href") {
                            ni.Url = string.Format(STR_URL_NEW_PAGE, atr.Value.Replace("&", "^"));
                            break;
                        }
                    }
                    ni.Content = newsWrap.SelectNodes("//div[@class='view']/div[@class='memo']")[i].InnerText;
                    ni.Content = ni.Content.Trim().Replace("\r\n\r\n", "\r\n").Replace("\t", "");
                    news.Add(ni);
                }
            }

            if (news.Count == 0) {
                return null;
            }
            return news;
        }
    }
}