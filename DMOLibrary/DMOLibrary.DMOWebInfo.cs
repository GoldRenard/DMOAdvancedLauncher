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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DMOLibrary.DMOWebInfo
{
    #region Structs
    public struct server
    {
        public int Id;
        public string Name;
    }

    public struct guild
    {
        public int Key;
        public int Id;
        public int Serv_id;
        public string Name;
        public long Rep;
        public long Master_id;
        public string Master_name;
        public long Rank;
        public DateTime Update_time;
        public bool isDetailed;
        public List<tamer> Members;
    }

    public struct digimon_type
    {
        public int Id;
        public string Name;
        public string Name_alt;
    }

    public struct digimon
    {
        public int Key;
        public int Type_id;
        public int Serv_id;
        public int Tamer_id;
        public string Custom_Tamer_Name;
        public int Custom_Tamer_lvl;
        public string Name;
        public long Rank;
        public int Lvl;
        public double Size_cm;
        public int Size_pc;
        public int Size_rank;
    }

    public struct tamer_type
    {
        public int Id;
        public string Name;
    }

    public struct tamer
    {
        public int Key;
        public int Id;
        public int Type_id;
        public int Serv_id;
        public int Guild_id;
        public int Partner_key;
        public string Partner_name;
        public string Name;
        public long Rank;
        public int Lvl;
        public List<digimon> Digimons;
    }

    public struct NewsItem
    {
        public string mode;
        public string subj;
        public string date;
        public string content;
        public string url;
    }

    public struct DownloadStatus
    {
        public DMODownloadStatusCode code;
        public string info;
        public int progress;
        public int max_progress;
    }

    public enum DMODownloadStatusCode
    {
        GETTING_GUILD = 0,
        GETTING_TAMER = 1
    }

    public enum DMODownloadResultCode
    {
        OK = 0,
        DB_CONNECT_ERROR = 1,
        WEB_ACCESS_ERROR = 2,
        NOT_FOUND = 404,
        CANT_GET = 3
    }
    #endregion

    public class DMOWebInfo
    {
        DMODatabase db = null;
        DownloadStatus download_status = new DownloadStatus();

        static string STR_RANKING_NODE = "//div[@class='list bbs-ranking']";
        static string STR_GUILD_ID_REGEX = "(\\/Ranking\\/GuildRankingDetail\\.aspx\\?gid=)(\\d+)(&srvn=)";
        static string STR_TAMER_ID_REGEX = "(\\/Ranking\\/MainPop\\.aspx\\?tid=)(\\d+)(&srvn=)";
        static string STR_DATE_FORMAT_REGEX = "(\\d\\d)(-)(\\d\\d)(-)(\\d\\d)";

        static string STR_URL_JM_MAINPAGE = "http://dmocp.joymax.com/Main/HomeMain.aspx";
        static string STR_URL_NEW_PAGE = "http://www.joymax.com/dmo/Property_Front.aspx?workurl=http://dmocp.joymax.com{0}";
        static string STR_URL_GUILD_PAGE = "http://dmocp.joymax.com/Ranking/GuildRankingDetail.aspx?gid={0}&srvn={1}";
        static string STR_URL_TAMER_POPPAGE = "http://dmocp.joymax.com/us/Ranking/MainPop.aspx?tid={0}&srvn={1}";
        static string STR_URL_GUILD_RANK = "http://dmocp.joymax.com/Ranking/GuildRankingList.aspx?st=0&sw={0}&srvn={1}";
        static string STR_URL_MERC_SIZE_RANK = "http://dmocp.joymax.com/Ranking/SizeRankingList.aspx?sw={0}&srvn={1}&dtype={2}";
        static string STR_URL_STARTER_RANK = "http://dmocp.joymax.com/Ranking/PartnerRankingList.aspx?sw={0}&srvn={1}";

        #region EVENTS
        /* Complete results:
         * 0 - all good
         * 1 - can't connect to database
         * 2 - web access error
         * 404 - guild not found
         * 3 - can't get guild info
         * 4 - web page is not supported or guild not found
         * */

        System.Windows.Threading.Dispatcher owner_dispatcher;

        public delegate void DownloadHandler(object sender);
        public delegate void DownloadCompleteHandler(object sender, DMODownloadResultCode code, guild result);
        public delegate void DownloadStatusChangedHandler(object sender, DownloadStatus status);
        public event DownloadHandler DownloadStarted;
        public event DownloadCompleteHandler DownloadCompleted;
        public event DownloadStatusChangedHandler StatusChanged;
        protected virtual void OnStarted()
        {
            if (DownloadStarted != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadHandler((sender) =>
                    {
                        DownloadStarted(sender);
                    }), this);
                }
                else
                    DownloadStarted(this);
            }
        }
        protected virtual void OnCompleted(DMODownloadResultCode code, guild result)
        {
            if (DownloadCompleted != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadCompleteHandler((sender, code_, result_) =>
                    {
                        DownloadCompleted(sender, code_, result_);
                    }), this, code, result);
                }
                else
                    DownloadCompleted(this, code, result);
            }
        }
        protected virtual void OnStatusChanged(DMODownloadStatusCode code, string info, int p, int pm)
        {
            download_status.code = code;
            download_status.info = info;
            download_status.progress = p;
            download_status.max_progress = pm;

            if (StatusChanged != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadStatusChangedHandler((sender, status_) =>
                    {
                        StatusChanged(sender, status_);
                    }), this, download_status);
                }
                else
                    StatusChanged(this, download_status);
            }
        }
        #endregion

        public DMOWebInfo(string DB_FILE)
        {
            db = new DMODatabase(DB_FILE);
        }

        public void GetGuildAsync(System.Windows.Threading.Dispatcher owner_dispatcher, string g_name, server serv, bool isDetailed, int ActualDays)
        {
            this.owner_dispatcher = owner_dispatcher;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s1, e2) => { GetGuild(g_name, serv, isDetailed, ActualDays); };
            bw.RunWorkerAsync();
        }

        public digimon GetRandomDigimon(server serv, string g_name, int minlvl)
        {
            digimon d = new digimon();
            if (db.OpenConnection())
            {
                d = db.RandomDigimon(serv, g_name, minlvl);
                db.CloseConnection();
            }
            return d;
        }

        public guild GetGuild(string g_name, server serv, bool isDetailed, int ActualDays)
        {
            OnStarted();
            if (db.OpenConnection())
            {
                //Check actual guild in database
                guild g_db = db.ReadGuild(g_name, serv, ActualDays);
                if (g_db.Id != -1)
                {
                    if (!(isDetailed && !g_db.isDetailed))
                    {
                        //and return it
                        db.CloseConnection();
                        OnCompleted(DMODownloadResultCode.OK, g_db);
                        return g_db;
                    }
                }
                db.CloseConnection();
                //else get database from web
                guild g_info = new guild();
                g_info.Id = -1;
                HtmlDocument doc = new HtmlDocument();

                OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, g_name, 0, 50);

                string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_RANK, g_name, "srv" + serv.Id));
                if (html == string.Empty)
                {
                    OnCompleted(DMODownloadResultCode.WEB_ACCESS_ERROR, g_info);
                    return g_info;
                }
                doc.LoadHtml(html);

                HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
                HtmlNodeCollection tlist = ranking.SelectNodes("//tr/td[@class='guild']");
                bool isFound = false;
                if (tlist != null)
                {
                    HtmlNode e = null;
                    for (int i = 0; i <= tlist.Count - 2; i++)
                    {
                        try { e = ranking.SelectNodes("//td[@class='guild']")[i]; }
                        catch { };
                        if (e != null)
                            if (ClearStr(e.InnerText) == g_name)
                            {
                                Regex r = new Regex(STR_GUILD_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                Match m = r.Match(ranking.SelectNodes("//td[@class='detail']")[i].InnerHtml);
                                if (m.Success)
                                {
                                    g_info.Id = Convert.ToInt32(m.Groups[2].ToString());
                                    string master = ranking.SelectNodes("//td[@class='master']")[i].InnerText;
                                    master = master.Substring(0, master.IndexOf(' '));
                                    g_info.Master_name = master;
                                    g_info.Serv_id = serv.Id;
                                    g_info.Name = g_name;
                                    g_info.Rank = Convert.ToInt32(ranking.SelectNodes("//td[@class='ranking']")[i].InnerText);
                                    g_info.Rep = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='reputation']")[i].InnerText));
                                    isFound = true;
                                }
                            }
                    }
                    if (!isFound)
                    {
                        OnCompleted(DMODownloadResultCode.NOT_FOUND, g_info); // guild not found
                        return g_info;
                    }

                    if (GetGuildInfo(ref g_info, isDetailed))
                    {
                        //write new guild into database and read back with detailed data (if not)
                        g_info.Update_time = DateTime.Now;
                        if (db.OpenConnection())
                        {
                            db.WriteGuild(g_info, isDetailed);
                            g_db = db.ReadGuild(g_name, serv, ActualDays);
                            db.CloseConnection();
                        }
                        OnCompleted(DMODownloadResultCode.OK, g_db);
                        return g_db;
                    }
                    else
                    {
                        OnCompleted(DMODownloadResultCode.CANT_GET, g_info); // can't get guild info
                        return g_info;
                    }
                }
                else
                {
                    OnCompleted(DMODownloadResultCode.NOT_FOUND, g_info);//wrong web page
                    return g_info;
                }
            }
            guild empty = new guild { Id = -1 };
            OnCompleted(DMODownloadResultCode.DB_CONNECT_ERROR, empty); //can't connect to database
            return empty;
        }

        public bool GetGuildInfo(ref guild g, bool isDetailed)
        {
            List<tamer> tamer_list = new List<tamer>();
            HtmlDocument doc = new HtmlDocument();

            string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_PAGE, g.Id.ToString(), "srv" + g.Serv_id));
            if (html == string.Empty)
                return false;
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection tlist = ranking.SelectNodes("//tr/td[@class='level']");
            for (int i = 0; i <= tlist.Count - 1; i++)
            {
                tamer t_info = new tamer();
                t_info.Name = ClearStr(ranking.SelectNodes("//td[@class='guild']")[i].InnerText);
                OnStatusChanged(DMODownloadStatusCode.GETTING_TAMER, t_info.Name, i, tlist.Count - 1);
                t_info.Serv_id = g.Serv_id;
                t_info.Guild_id = g.Id;
                t_info.Lvl = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='level']")[i].InnerText));
                string t_type = ranking.SelectNodes("//td[@class='guild']//img")[i].GetAttributeValue("src", null);
                t_type = t_type.Substring(t_type.LastIndexOf('/') + 1, t_type.LastIndexOf('.') - t_type.LastIndexOf('/') - 1);
                t_info.Type_id = Convert.ToInt32(t_type);

                t_info.Partner_name = ClearStr(ranking.SelectNodes("//td[@class='partner']")[i].InnerText);
                t_info.Rank = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='ranking']")[i].InnerText));

                Regex r = new Regex(STR_TAMER_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match m = r.Match(ranking.SelectNodes("//td[@class='detail']")[i].InnerHtml);

                if (m.Success)
                {
                    t_info.Id = Convert.ToInt32(m.Groups[2].ToString());
                    t_info.Digimons = GetDigimons(t_info, isDetailed);
                    if (t_info.Digimons.Count == 0)
                        return false;
                    tamer_list.Add(t_info);
                    if (t_info.Name == g.Master_name)
                        g.Master_id = t_info.Id;
                }
            }
            if (tamer_list.Count == 0)
                return false;
            g.Members = tamer_list;
            return true;
        }

        public List<digimon> GetDigimons(tamer tamer, bool isDetailed)
        {
            List<digimon> digi_list = new List<digimon>();
            HtmlDocument doc = new HtmlDocument();

            string html = WebDownload.GetHTML(string.Format(STR_URL_TAMER_POPPAGE, tamer.Id.ToString(), "srv" + tamer.Serv_id.ToString()));
            if (html == string.Empty)
                return digi_list;
            doc.LoadHtml(html);

            //getting starter
            HtmlNode tamer_info = doc.DocumentNode.SelectNodes("//div[@class='tamer-area']")[0];
            digimon partner_info = new digimon();
            partner_info.Tamer_id = tamer.Id;
            partner_info.Serv_id = tamer.Serv_id;
            partner_info.Name = ClearStr(tamer_info.SelectNodes("//ul/li[@class='partner']/span")[0].InnerText);
            if (!StarterInfo(ref partner_info, tamer.Name))
                return digi_list;
            digi_list.Add(partner_info);

            HtmlNode merc_list = doc.DocumentNode.SelectNodes("//div[@id='rankingscroll']")[0];
            HtmlNodeCollection dlist = merc_list.SelectNodes("//li/em[@class='partner']");

            if (dlist != null)
            {
                for (int i = 0; i <= dlist.Count - 1; i++)
                {
                    digimon d_info = new digimon();
                    d_info.Tamer_id = tamer.Id;
                    d_info.Serv_id = tamer.Serv_id;
                    d_info.Name = ClearStr(merc_list.SelectNodes("//em[@class='partner']")[i].InnerText);
                    List<digimon_type> types = null;
                    if (db.OpenConnection())
                    {
                        types = db.Digimon_GetTypesByName(d_info.Name);
                        db.CloseConnection();
                    }
                    if (types == null)
                        continue;
                    d_info.Type_id = types[0].Id;
                    d_info.Lvl = Convert.ToInt32(ClearStr(merc_list.SelectNodes("//span[@class='level']")[i].InnerText));
                    d_info.Rank = Convert.ToInt32(ClearStr(merc_list.SelectNodes("//span[@class='ranking']")[i].InnerText));
                    if (isDetailed)
                        DigimonInfo(ref d_info, tamer.Name);
                    digi_list.Add(d_info);
                }
            }
            return digi_list;
        }

        public bool StarterInfo(ref digimon digimon, string tamer_name)
        {
            HtmlDocument doc = new HtmlDocument();

            digimon.Size_pc = 100;
            if (digimon.Name == "Agumon")
                digimon.Size_cm = 117;
            else if (digimon.Name == "Gaomon")
                digimon.Size_cm = 137;
            else if (digimon.Name == "Lalamon")
                digimon.Size_cm = 154;
            else if (digimon.Name == "Falcomon")
                digimon.Size_cm = 127;

            string html = WebDownload.GetHTML(string.Format(STR_URL_STARTER_RANK, tamer_name, "srv" + digimon.Serv_id));
            if (html == string.Empty)
                return false;
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection dlist = ranking.SelectNodes("//tr/td[@class='tamer2']");

            if (dlist != null)
                for (int i = 0; i <= dlist.Count - 1; i++)
                {
                    if (ClearStr(ranking.SelectNodes("//td[@class='tamer2']")[i].InnerText) == tamer_name)
                    {
                        if (db.OpenConnection())
                        {
                            digimon.Type_id = db.Digimon_GetTypesByName(digimon.Name)[0].Id;
                            db.CloseConnection();
                        }
                        digimon.Rank = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='ranking']")[i + 3].InnerText));
                        digimon.Name = ClearStr(ranking.SelectNodes("//td[@class='name']")[i + 3].InnerText);
                        digimon.Lvl = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='level']")[i + 3].InnerText));
                        return true;
                    }
                }

            return false;
        }

        public bool DigimonInfo(ref digimon digimon, string tamer_name)
        {
            //we don't need starters info
            if (digimon.Type_id == 31003 || digimon.Type_id == 31002 || digimon.Type_id == 31004 || digimon.Type_id == 31001)
                return false;

            HtmlDocument doc = new HtmlDocument();
            List<digimon_type> d_types = new List<digimon_type>();
            if (db.OpenConnection())
            {
                if (db.Digimon_GetTypeById(digimon.Type_id).Id == -1)
                {
                    db.CloseConnection();
                    return false;
                }
                d_types = db.Digimon_GetTypesByName(digimon.Name);
                db.CloseConnection();
            }

            foreach (digimon_type d_type in d_types)
            {
                string html = WebDownload.GetHTML(string.Format(STR_URL_MERC_SIZE_RANK, tamer_name, "srv" + digimon.Serv_id.ToString(), d_type.Id.ToString()));
                if (html == string.Empty)
                    continue;
                doc.LoadHtml(html);

                HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
                HtmlNodeCollection dlist = ranking.SelectNodes("//tr/td[@class='tamer2']");
                string size;

                if (dlist != null)
                    for (int i = 0; i <= dlist.Count - 1; i++)
                    {
                        if (ClearStr(ranking.SelectNodes("//td[@class='tamer2']")[i].InnerText) == tamer_name)
                        {
                            digimon.Type_id = d_type.Id;
                            size = ranking.SelectNodes("//td[@class='size']")[i + 3].InnerText.Replace("cm", "");
                            string size_cm = size.Substring(0, size.IndexOf(' '));
                            double.TryParse(size_cm.Replace('.', ','), out digimon.Size_cm);
                            digimon.Size_pc = Convert.ToInt32(size.Substring(size.LastIndexOf('(') + 1, size.LastIndexOf('%') - size.LastIndexOf('(') - 1));
                            digimon.Size_rank = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='ranking']")[i + 3].InnerText));
                            digimon.Name = ClearStr(ranking.SelectNodes("//td[@class='name']")[i + 3].InnerText);
                            return true;
                        }
                    }
            }
            return false;
        }

        public static List<NewsItem> GetJoymaxNews()
        {
            HtmlDocument doc = new HtmlDocument();
            List<NewsItem> news = new List<NewsItem>();

            string html = WebDownload.GetHTML(STR_URL_JM_MAINPAGE);
            if (html == string.Empty)
                return null;
            doc.LoadHtml(html);

            HtmlNode news_wrap = doc.DocumentNode.SelectNodes("//div[@class='news-list']/ul/li")[0];
            HtmlNodeCollection news_list = doc.DocumentNode.SelectNodes("//div[@class='news-list']/ul/li");
            NewsItem ni;

            if (news_list != null)
            {
                for (int i = 0; i <= news_list.Count - 1; i++)
                {
                    ni = new NewsItem();
                    ni.mode = news_wrap.SelectNodes("//div[@class='lead']/span[contains(@class, 'mode')]")[i].InnerText;
                    ni.subj = System.Web.HttpUtility.HtmlDecode(news_wrap.SelectNodes("//div[@class='lead']/span[@class='subj']")[i].InnerText);
                    ni.date = news_wrap.SelectNodes("//div[@class='lead']/span[@class='date']")[i].InnerText;

                    Regex r = new Regex(STR_DATE_FORMAT_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match m = r.Match(ni.date);
                    if (m.Success)
                        ni.date = m.Groups[3].ToString() + "." + m.Groups[1].ToString() + "." + m.Groups[5].ToString();

                    foreach (HtmlAttribute atr in news_wrap.SelectNodes("//div[@class='view']/div[@class='btn-right']/span[@class='read-more']/a")[i].Attributes)
                        if (atr.Name == "href")
                        {
                            ni.url = string.Format(STR_URL_NEW_PAGE, atr.Value.Replace("&", "^"));
                            break;
                        }
                    ni.content = System.Web.HttpUtility.HtmlDecode(news_wrap.SelectNodes("//div[@class='view']/div[@class='memo']")[i].InnerText);
                    ni.content = ni.content.Trim().Replace("\r\n\r\n", "\r\n").Replace("\t", "");
                    news.Add(ni);
                }
            }

            if (news.Count == 0)
                return null;
            return news;
        }

        static string ClearStr(string str)
        {
            return str.Replace(",", string.Empty).Replace(" ", string.Empty);
        }
    }
}
