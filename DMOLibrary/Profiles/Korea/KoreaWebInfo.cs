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
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DMOLibrary.Profiles.Korea {
    public class KoreaWebInfo : DMOWebProfile {
        static string STR_GUILD_ID_REGEX = "(main_sub\\.aspx\\?v=)(\\d+)(&o=)(\\d+)";
        static string STR_TAMER_LVL_REGEX = "(\\/images\\/comm\\/icon\\/lv_)(\\d+)(\\.gif)";
        static string STR_TAMER_TYPE_REGEX = "(\\/images\\/ranking\\/icon\\/)(\\d+)(\\.gif)";
        static string STR_TAMER_ID_REGEX = "(\\/ranking\\/main_pop\\.aspx\\?o=)(\\d+)(&type=)";
        static string STR_DIGIMON_SIZE = "(\\d+)( cm \\()(\\d+)(%\\))";

        static string STR_URL_TAMER_POPPAGE = "http://www.digimonmasters.com/ranking/main_pop.aspx?o={0}&type={1}00";
        static string STR_URL_GUILD_RANK = "http://www.digimonmasters.com/guild/main.aspx?type={1}00&searchtype=GUILDNAME&search={0}";
        static string STR_URL_GUILD_PAGE = "http://www.digimonmasters.com/guild/main_sub.aspx?v={1}00&o={0}";
        static string STR_URL_STARTER_RANK = "http://www.digimonmasters.com/ranking/partner.aspx?type={1}00&search={0}";
        static string STR_URL_MERC_SIZE_RANK = "http://www.digimonmasters.com/ranking/size.aspx?type={1}00&search={0}&digimon={2}";

        public KoreaWebInfo(DMODatabase Database) {
            this.Database = Database;
        }

        public override guild GetGuild(string g_name, server serv, bool isDetailed, int ActualDays) {
            if (IsBusy) DispatcherHelper.DoEvents();
            OnStarted();
            if (Database.OpenConnection()) {
                //Check actual guild in database
                guild g_db = Database.ReadGuild(g_name, serv, ActualDays);
                if (g_db.Id != -1) {
                    if (!(isDetailed && !g_db.isDetailed)) {
                        //and return it
                        Database.CloseConnection();
                        OnCompleted(DMODownloadResultCode.OK, g_db);
                        return g_db;
                    }
                }
                Database.CloseConnection();
                //else get database from web
                guild g_info = new guild();
                g_info.Id = -1;
                HtmlDocument doc = new HtmlDocument();

                OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, g_name, 0, 50);

                string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_RANK, g_name, serv.Id));
                if (html == string.Empty) {
                    OnCompleted(DMODownloadResultCode.WEB_ACCESS_ERROR, g_info);
                    return g_info;
                }
                doc.LoadHtml(html);

                bool isFound = false;
                HtmlNode ranking = doc.DocumentNode;
                try {
                    ranking = doc.DocumentNode.SelectNodes("//div[@id='body']//table[@class='forum_list'][1]//tbody//tr[not(@onmouseover)]")[4];

                    g_info.Serv_id = serv.Id;
                    g_info.Rank = CheckRankNode(ranking.SelectSingleNode(".//td[1]"));

                    g_info.Name = ClearStr(ranking.SelectSingleNode(".//td[2]").InnerText);
                    g_info.Rep = Convert.ToInt32(ClearStr(ranking.SelectSingleNode(".//td[4]").InnerText));
                    g_info.Master_name = ClearStr(ranking.SelectSingleNode(".//td[5]").InnerText);

                    Regex r1 = new Regex(STR_GUILD_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    string link = ranking.SelectSingleNode(".//td[7]").InnerHtml;
                    Match m1 = r1.Match(link);
                    if (m1.Success) {
                        g_info.Id = Convert.ToInt32(m1.Groups[4].ToString());
                        isFound = true;
                    }
                } catch { isFound = false; }

                if (!isFound) {
                    OnCompleted(DMODownloadResultCode.NOT_FOUND, g_info); // guild not found
                    return g_info;
                }

                if (GetGuildInfo(ref g_info, isDetailed)) {
                    //write new guild into database and read back with detailed data (if not)
                    g_info.Update_time = DateTime.Now;
                    if (Database.OpenConnection()) {
                        Database.WriteGuild(g_info, isDetailed);
                        g_db = Database.ReadGuild(g_name, serv, ActualDays);
                        Database.CloseConnection();
                    }
                    OnCompleted(DMODownloadResultCode.OK, g_db);
                    return g_db;
                } else {
                    OnCompleted(DMODownloadResultCode.CANT_GET, g_info); // can't get guild info
                    return g_info;
                }
            }
            guild empty = new guild { Id = -1 };
            OnCompleted(DMODownloadResultCode.DB_CONNECT_ERROR, empty); //can't connect to database*/
            return empty;
        }

        protected override bool GetGuildInfo(ref guild g, bool isDetailed) {
            List<tamer> tamer_list = new List<tamer>();
            HtmlDocument doc = new HtmlDocument();

            string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_PAGE, g.Id.ToString(), g.Serv_id));
            if (html == string.Empty)
                return false;
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes("//table[@class='forum_list']//tbody")[1];
            HtmlNodeCollection tlist = ranking.SelectNodes(".//tr");

            if (tlist != null) {
                for (int i = 0; i < tlist.Count; i++) {
                    try {
                        tamer t_info = new tamer();
                        t_info.Serv_id = g.Serv_id;
                        t_info.Guild_id = g.Id;
                        t_info.Name = ClearStr(tlist[i].SelectSingleNode(".//td[3]").InnerText);

                        OnStatusChanged(DMODownloadStatusCode.GETTING_TAMER, t_info.Name, i, tlist.Count);

                        Regex regex = new Regex(STR_TAMER_TYPE_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        Match match = regex.Match(tlist[i].SelectSingleNode(".//td[3]").InnerHtml);
                        if (match.Success)
                            t_info.Type_id = Convert.ToInt32(match.Groups[2].ToString());

                        regex = new Regex(STR_TAMER_LVL_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        match = regex.Match(tlist[i].SelectSingleNode(".//td[4]").InnerHtml);
                        if (match.Success)
                            t_info.Lvl = Convert.ToInt32(match.Groups[2].ToString());
                        t_info.Partner_name = ClearStr(tlist[i].SelectSingleNode(".//td[5]").InnerText);
                        t_info.Rank = CheckRankNode(tlist[i].SelectSingleNode(".//td[2]"));

                        regex = new Regex(STR_TAMER_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        match = regex.Match(tlist[i].SelectSingleNode(".//td[7]").InnerHtml);
                        if (match.Success)
                            t_info.Id = Convert.ToInt32(match.Groups[2].ToString());

                        if (t_info.Type_id != 0 && t_info.Lvl != 0 && t_info.Id != 0) {
                            if (t_info.Name == g.Master_name)
                                g.Master_id = t_info.Id;
                            t_info.Digimons = GetDigimons(t_info, isDetailed);
                            if (t_info.Digimons.Count == 0)
                                return false;
                            tamer_list.Add(t_info);
                        }
                    } catch { }
                }
            }

            if (tamer_list.Count == 0)
                return false;
            g.Members = tamer_list;
            return true;
        }

        protected override List<digimon> GetDigimons(tamer tamer, bool isDetailed) {
            List<digimon> digi_list = new List<digimon>();
            HtmlDocument doc = new HtmlDocument();

            string html = WebDownload.GetHTML(string.Format(STR_URL_TAMER_POPPAGE, tamer.Id.ToString(), tamer.Serv_id.ToString()));
            if (html == string.Empty)
                return digi_list;
            doc.LoadHtml(html);

            //getting starter
            digimon partner_info;
            try {
                partner_info = new digimon();
                partner_info.Tamer_id = tamer.Id;
                partner_info.Serv_id = tamer.Serv_id;
                partner_info.Name = ClearStr(doc.DocumentNode.SelectSingleNode("//div[1]//div[2]//div[2]//table[1]//tr[1]//td[2]//table[1]//tr[3]//td[2]//b").InnerText);
            } catch { return digi_list; }
            if (!StarterInfo(ref partner_info, tamer.Name))
                return digi_list;
            digi_list.Add(partner_info);

            HtmlNode merc_list = doc.DocumentNode.SelectNodes("//table[@class='list']")[1];
            HtmlNodeCollection dlist = merc_list.SelectNodes(".//tr");

            if (dlist != null) {
                for (int i = 1; i < dlist.Count; i++) {
                    digimon d_info = new digimon();
                    d_info.Tamer_id = tamer.Id;
                    d_info.Serv_id = tamer.Serv_id;

                    d_info.Name = ClearStr(dlist[i].SelectSingleNode(".//td[1]").InnerText);

                    d_info.Lvl = Convert.ToInt32(ClearStr(dlist[i].SelectSingleNode(".//td[2]//label").InnerText));
                    string rank = string.Empty;
                    foreach (char c in ClearStr(dlist[i].SelectSingleNode(".//td[3]//label").InnerText))
                        if (Char.IsDigit(c))
                            rank += c;
                        else
                            break;
                    d_info.Rank = Convert.ToInt32(rank);

                    List<digimon_type> types = null;
                    if (Database.OpenConnection()) {
                        types = Database.Digimon_GetTypesByKoreanName(d_info.Name);
                        Database.CloseConnection();
                    }
                    if (types == null)
                        continue;
                    d_info.Name = types[0].Name;
                    d_info.Type_id = types[0].Id;

                    if (isDetailed)
                        DigimonInfo(ref d_info, tamer.Name);
                    digi_list.Add(d_info);
                }
            }
            return digi_list;
        }

        protected override bool StarterInfo(ref digimon digimon, string tamer_name) {
            HtmlDocument doc = new HtmlDocument();

            digimon.Size_pc = 100;
            if (digimon.Name == "아구몬") //Agumon
                digimon.Size_cm = 117;
            else if (digimon.Name == "가오몬") //Gaomon
                digimon.Size_cm = 137;
            else if (digimon.Name == "라라몬") //Lalamon
                digimon.Size_cm = 154;
            else if (digimon.Name == "팰코몬") //Falcomon
                digimon.Size_cm = 127;

            string html = WebDownload.GetHTML(string.Format(STR_URL_STARTER_RANK, tamer_name, digimon.Serv_id));
            if (html == string.Empty)
                return false;
            doc.LoadHtml(html);

            HtmlNode partner_node;
            try { partner_node = doc.DocumentNode.SelectNodes("//table[@class='forum_list']")[1].SelectSingleNode(".//tbody//tr[not(@onmouseover)]"); } catch { return false; }

            if (partner_node != null) {
                if (Database.OpenConnection()) {
                    digimon_type dtype = Database.Digimon_GetTypesByKoreanName(digimon.Name)[0];
                    digimon.Type_id = dtype.Id;
                    digimon.Name = dtype.Name;
                    Database.CloseConnection();
                }

                digimon.Rank = CheckRankNode(partner_node.SelectSingleNode(".//td[1]"));
                digimon.Name = ClearStr(partner_node.SelectSingleNode(".//td[2]//label").InnerText);
                digimon.Lvl = Convert.ToInt32(ClearStr(partner_node.SelectSingleNode(".//td[3]").InnerText));
                return true;
            }
            return false;
        }

        protected override bool DigimonInfo(ref digimon digimon, string tamer_name) {
            //we don't need starters info
            if (digimon.Type_id == 31003 || digimon.Type_id == 31002 || digimon.Type_id == 31004 || digimon.Type_id == 31001)
                return false;

            HtmlDocument doc = new HtmlDocument();
            List<digimon_type> d_types = new List<digimon_type>();
            if (Database.OpenConnection()) {
                if (Database.Digimon_GetTypeById(digimon.Type_id).Id == -1) {
                    Database.CloseConnection();
                    return false;
                }
                d_types = Database.Digimon_GetTypesByName(digimon.Name);
                Database.CloseConnection();
            }

            foreach (digimon_type d_type in d_types) {
                string html = WebDownload.GetHTML(string.Format(STR_URL_MERC_SIZE_RANK, tamer_name, digimon.Serv_id.ToString(), d_type.Id.ToString()));
                if (html == string.Empty)
                    continue;
                doc.LoadHtml(html);

                HtmlNode partner_node;
                try { partner_node = doc.DocumentNode.SelectNodes("//table[@class='forum_list']")[1].SelectSingleNode(".//tbody//tr[not(@onmouseover)]"); } catch { return false; }

                if (partner_node != null) {
                    digimon.Type_id = d_type.Id;
                    digimon.Size_rank = Convert.ToInt32(ClearStr(partner_node.SelectSingleNode(".//td[1]").InnerText));
                    digimon.Name = ClearStr(partner_node.SelectSingleNode(".//td[2]//label").InnerText);
                    digimon.Lvl = Convert.ToInt32(ClearStr(partner_node.SelectSingleNode(".//td[4]").InnerText));
                    Regex r = new Regex(STR_DIGIMON_SIZE, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match m = r.Match(partner_node.SelectSingleNode(".//td[3]").InnerHtml);
                    if (m.Success) {
                        digimon.Size_cm = Convert.ToInt32(m.Groups[1].ToString());
                        digimon.Size_pc = Convert.ToInt32(m.Groups[3].ToString());
                        return true;
                    }
                }
            }
            return false;
        }

        static string ClearStr(string str) {
            return str.Replace(",", string.Empty).Replace(" ", string.Empty).Replace(Environment.NewLine, string.Empty);
        }

        private long CheckRankNode(HtmlNode node) {
            string rank = ClearStr(node.InnerText);
            long rank_num = 0;
            if (rank == string.Empty) {
                string html_rank = node.InnerHtml;
                if (html_rank.Contains("no_01"))
                    rank_num = 1;
                else if (html_rank.Contains("no_02"))
                    rank_num = 2;
                else if (html_rank.Contains("no_03"))
                    rank_num = 3;
            } else
                rank_num = Convert.ToInt64(rank);
            return rank_num;
        }
    }
}
