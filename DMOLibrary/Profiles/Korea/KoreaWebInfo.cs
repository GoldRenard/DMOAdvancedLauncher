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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DMOLibrary.Profiles.Korea {
    public class KoreaWebInfo : DMOWebProfile {
        private static string STR_GUILD_ID_REGEX = "(main_sub\\.aspx\\?v=)(\\d+)(&o=)(\\d+)";
        private static string STR_TAMER_LVL_REGEX = "(\\/images\\/comm\\/icon\\/lv_)(\\d+)(\\.gif)";
        private static string STR_TAMER_TYPE_REGEX = "(\\/images\\/ranking\\/icon\\/)(\\d+)(\\.gif)";
        private static string STR_TAMER_ID_REGEX = "(\\/ranking\\/main_pop\\.aspx\\?o=)(\\d+)(&type=)";
        private static string STR_DIGIMON_SIZE = "(\\d+)( cm \\()(\\d+)(%\\))";

        private static string STR_URL_TAMER_POPPAGE = "http://www.digimonmasters.com/ranking/main_pop.aspx?o={0}&type={1}00";
        private static string STR_URL_GUILD_RANK = "http://www.digimonmasters.com/guild/main.aspx?type={1}00&searchtype=GUILDNAME&search={0}";
        private static string STR_URL_GUILD_PAGE = "http://www.digimonmasters.com/guild/main_sub.aspx?v={1}00&o={0}";
        private static string STR_URL_STARTER_RANK = "http://www.digimonmasters.com/ranking/partner.aspx?type={1}00&search={0}";
        private static string STR_URL_MERC_SIZE_RANK = "http://www.digimonmasters.com/ranking/size.aspx?type={1}00&search={0}&digimon={2}";

        public KoreaWebInfo(DMODatabase Database) {
            this.Database = Database;
        }

        public override Guild GetGuild(string guildName, Server serv, bool isDetailed, int actualDays) {
            if (IsBusy) DispatcherHelper.DoEvents();
            OnStarted();
            if (Database.OpenConnection()) {
                //Check actual guild in database
                Guild storedGuild = Database.ReadGuild(guildName, serv, actualDays);
                if (storedGuild.Id != -1) {
                    if (!(isDetailed && !storedGuild.IsDetailed)) {
                        //and return it
                        Database.CloseConnection();
                        OnCompleted(DMODownloadResultCode.OK, storedGuild);
                        return storedGuild;
                    }
                }
                Database.CloseConnection();
                //else get database from web
                Guild guildInfo = new Guild();
                guildInfo.Id = -1;
                HtmlDocument doc = new HtmlDocument();

                OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, guildName, 0, 50);

                string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_RANK, guildName, serv.Id));
                if (html == string.Empty) {
                    OnCompleted(DMODownloadResultCode.WEB_ACCESS_ERROR, guildInfo);
                    return guildInfo;
                }
                doc.LoadHtml(html);

                bool isFound = false;
                HtmlNode ranking = doc.DocumentNode;
                try {
                    ranking = doc.DocumentNode.SelectNodes("//div[@id='body']//table[@class='forum_list'][1]//tbody//tr[not(@onmouseover)]")[4];

                    guildInfo.ServId = serv.Id;
                    guildInfo.Rank = CheckRankNode(ranking.SelectSingleNode(".//td[1]"));

                    guildInfo.Name = ClearStr(ranking.SelectSingleNode(".//td[2]").InnerText);
                    guildInfo.Rep = Convert.ToInt32(ClearStr(ranking.SelectSingleNode(".//td[4]").InnerText));
                    guildInfo.MasterName = ClearStr(ranking.SelectSingleNode(".//td[5]").InnerText);

                    Regex r1 = new Regex(STR_GUILD_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    string link = ranking.SelectSingleNode(".//td[7]").InnerHtml;
                    Match m1 = r1.Match(link);
                    if (m1.Success) {
                        guildInfo.Id = Convert.ToInt32(m1.Groups[4].ToString());
                        isFound = true;
                    }
                } catch {
                    isFound = false;
                }

                if (!isFound) {
                    OnCompleted(DMODownloadResultCode.NOT_FOUND, guildInfo); // guild not found
                    return guildInfo;
                }

                if (GetGuildInfo(ref guildInfo, isDetailed)) {
                    //write new guild into database and read back with detailed data (if not)
                    guildInfo.UpdateTime = DateTime.Now;
                    if (Database.OpenConnection()) {
                        Database.WriteGuild(guildInfo, isDetailed);
                        storedGuild = Database.ReadGuild(guildName, serv, actualDays);
                        Database.CloseConnection();
                    }
                    OnCompleted(DMODownloadResultCode.OK, storedGuild);
                    return storedGuild;
                } else {
                    OnCompleted(DMODownloadResultCode.CANT_GET, guildInfo); // can't get guild info
                    return guildInfo;
                }
            }
            Guild empty = new Guild {
                Id = -1
            };
            OnCompleted(DMODownloadResultCode.DB_CONNECT_ERROR, empty); //can't connect to database*/
            return empty;
        }

        protected override bool GetGuildInfo(ref Guild g, bool isDetailed) {
            List<Tamer> tamerList = new List<Tamer>();
            HtmlDocument doc = new HtmlDocument();

            string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_PAGE, g.Id.ToString(), g.ServId));
            if (html == string.Empty) {
                return false;
            }
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes("//table[@class='forum_list']//tbody")[1];
            HtmlNodeCollection tlist = ranking.SelectNodes(".//tr");

            if (tlist != null) {
                for (int i = 0; i < tlist.Count; i++) {
                    try {
                        Tamer tamerInfo = new Tamer();
                        tamerInfo.ServId = g.ServId;
                        tamerInfo.GuildId = g.Id;
                        tamerInfo.Name = ClearStr(tlist[i].SelectSingleNode(".//td[3]").InnerText);

                        OnStatusChanged(DMODownloadStatusCode.GETTING_TAMER, tamerInfo.Name, i, tlist.Count);

                        Regex regex = new Regex(STR_TAMER_TYPE_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        Match match = regex.Match(tlist[i].SelectSingleNode(".//td[3]").InnerHtml);
                        if (match.Success) {
                            tamerInfo.TypeId = Convert.ToInt32(match.Groups[2].ToString());
                        }

                        regex = new Regex(STR_TAMER_LVL_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        match = regex.Match(tlist[i].SelectSingleNode(".//td[4]").InnerHtml);
                        if (match.Success) {
                            tamerInfo.Lvl = Convert.ToInt32(match.Groups[2].ToString());
                        }
                        tamerInfo.PartnerName = ClearStr(tlist[i].SelectSingleNode(".//td[5]").InnerText);
                        tamerInfo.Rank = CheckRankNode(tlist[i].SelectSingleNode(".//td[2]"));

                        regex = new Regex(STR_TAMER_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        match = regex.Match(tlist[i].SelectSingleNode(".//td[7]").InnerHtml);
                        if (match.Success) {
                            tamerInfo.Id = Convert.ToInt32(match.Groups[2].ToString());
                        }

                        if (tamerInfo.TypeId != 0 && tamerInfo.Lvl != 0 && tamerInfo.Id != 0) {
                            if (tamerInfo.Name == g.MasterName) {
                                g.MasterId = tamerInfo.Id;
                            }
                            tamerInfo.Digimons = GetDigimons(tamerInfo, isDetailed);
                            if (tamerInfo.Digimons.Count == 0) {
                                return false;
                            }
                            tamerList.Add(tamerInfo);
                        }
                    } catch {
                    }
                }
            }

            if (tamerList.Count == 0) {
                return false;
            }
            g.Members = tamerList;
            return true;
        }

        protected override List<Digimon> GetDigimons(Tamer tamer, bool isDetailed) {
            List<Digimon> digimonList = new List<Digimon>();
            HtmlDocument doc = new HtmlDocument();

            string html = WebDownload.GetHTML(string.Format(STR_URL_TAMER_POPPAGE, tamer.Id.ToString(), tamer.ServId.ToString()));
            if (html == string.Empty) {
                return digimonList;
            }
            doc.LoadHtml(html);

            //getting starter
            Digimon partnerInfo;
            try {
                partnerInfo = new Digimon();
                partnerInfo.TamerId = tamer.Id;
                partnerInfo.ServId = tamer.ServId;
                partnerInfo.Name = ClearStr(doc.DocumentNode.SelectSingleNode("//div[1]//div[2]//div[2]//table[1]//tr[1]//td[2]//table[1]//tr[3]//td[2]//b").InnerText);
            } catch {
                return digimonList;
            }
            if (!StarterInfo(ref partnerInfo, tamer.Name)) {
                return digimonList;
            }
            digimonList.Add(partnerInfo);

            HtmlNode merc_list = doc.DocumentNode.SelectNodes("//table[@class='list']")[1];
            HtmlNodeCollection dlist = merc_list.SelectNodes(".//tr");

            if (dlist != null) {
                for (int i = 1; i < dlist.Count; i++) {
                    Digimon digimonInfo = new Digimon();
                    digimonInfo.TamerId = tamer.Id;
                    digimonInfo.ServId = tamer.ServId;

                    digimonInfo.Name = ClearStr(dlist[i].SelectSingleNode(".//td[1]").InnerText);

                    digimonInfo.Lvl = Convert.ToInt32(ClearStr(dlist[i].SelectSingleNode(".//td[2]//label").InnerText));
                    string rank = string.Empty;
                    foreach (char c in ClearStr(dlist[i].SelectSingleNode(".//td[3]//label").InnerText)) {
                        if (Char.IsDigit(c)) {
                            rank += c;
                        } else {
                            break;
                        }
                    }
                    digimonInfo.Rank = Convert.ToInt32(rank);

                    List<DigimonType> types = null;
                    if (Database.OpenConnection()) {
                        types = Database.GetDigimonTypesByKoreanName(digimonInfo.Name);
                        Database.CloseConnection();
                    }
                    if (types == null) {
                        continue;
                    }
                    digimonInfo.Name = types[0].Name;
                    digimonInfo.TypeId = types[0].Id;

                    if (digimonList.Count(d => d.TypeId.Equals(digimonInfo.TypeId)) == 0) {
                        if (isDetailed) {
                            DigimonInfo(ref digimonInfo, tamer.Name);
                        }
                        digimonList.Add(digimonInfo);
                    }
                }
            }
            return digimonList;
        }

        protected override bool StarterInfo(ref Digimon digimon, string tamer_name) {
            HtmlDocument doc = new HtmlDocument();

            digimon.SizePc = 100;
            digimon.SizeCm = ResolveStartedSize(digimon.Name);

            string html = WebDownload.GetHTML(string.Format(STR_URL_STARTER_RANK, tamer_name, digimon.ServId));
            if (html == string.Empty) {
                return false;
            }
            doc.LoadHtml(html);

            HtmlNode partnerNode;
            try {
                partnerNode = doc.DocumentNode.SelectNodes("//table[@class='forum_list']")[1].SelectSingleNode(".//tbody//tr[not(@onmouseover)]");
            } catch {
                return false;
            }

            if (partnerNode != null) {
                if (Database.OpenConnection()) {
                    DigimonType dType = Database.GetDigimonTypesByKoreanName(digimon.Name)[0];
                    digimon.TypeId = dType.Id;
                    digimon.Name = dType.Name;
                    Database.CloseConnection();
                }

                digimon.Rank = CheckRankNode(partnerNode.SelectSingleNode(".//td[1]"));
                digimon.Name = ClearStr(partnerNode.SelectSingleNode(".//td[2]//label").InnerText);
                digimon.Lvl = Convert.ToInt32(ClearStr(partnerNode.SelectSingleNode(".//td[3]").InnerText));
                return true;
            }
            return false;
        }

        protected override bool DigimonInfo(ref Digimon digimon, string tamer_name) {
            //we don't need starters info
            foreach (int id in STARTER_IDS) {
                if (digimon.TypeId == id) {
                    return false;
                }
            }

            HtmlDocument doc = new HtmlDocument();
            List<DigimonType> d_types = new List<DigimonType>();
            if (Database.OpenConnection()) {
                if (Database.GetDigimonTypeById(digimon.TypeId).Id == -1) {
                    Database.CloseConnection();
                    return false;
                }
                d_types = Database.GetDigimonTypesByName(digimon.Name);
                Database.CloseConnection();
            }

            foreach (DigimonType d_type in d_types) {
                string html = WebDownload.GetHTML(string.Format(STR_URL_MERC_SIZE_RANK, tamer_name, digimon.ServId.ToString(), d_type.Id.ToString()));
                if (html == string.Empty) {
                    continue;
                }
                doc.LoadHtml(html);

                HtmlNode partner_node;
                try {
                    partner_node = doc.DocumentNode.SelectNodes("//table[@class='forum_list']")[1].SelectSingleNode(".//tbody//tr[not(@onmouseover)]");
                } catch {
                    return false;
                }

                if (partner_node != null) {
                    digimon.TypeId = d_type.Id;
                    digimon.SizeRank = Convert.ToInt32(ClearStr(partner_node.SelectSingleNode(".//td[1]").InnerText));
                    digimon.Name = ClearStr(partner_node.SelectSingleNode(".//td[2]//label").InnerText);
                    digimon.Lvl = Convert.ToInt32(ClearStr(partner_node.SelectSingleNode(".//td[4]").InnerText));
                    Regex r = new Regex(STR_DIGIMON_SIZE, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match m = r.Match(partner_node.SelectSingleNode(".//td[3]").InnerHtml);
                    if (m.Success) {
                        digimon.SizeCm = Convert.ToInt32(m.Groups[1].ToString());
                        digimon.SizePc = Convert.ToInt32(m.Groups[3].ToString());
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
            long rankNum = 0;
            if (rank == string.Empty) {
                string htmlRank = node.InnerHtml;
                if (htmlRank.Contains("no_01")) {
                    rankNum = 1;
                } else if (htmlRank.Contains("no_02")) {
                    rankNum = 2;
                } else if (htmlRank.Contains("no_03")) {
                    rankNum = 3;
                }
            } else {
                rankNum = Convert.ToInt64(rank);
            }
            return rankNum;
        }

        private static double ResolveStartedSize(string digimonName) {
            if ("아구몬".Equals(digimonName)) {             // Agumon
                return 117;
            } else if ("가오몬".Equals(digimonName)) {      // Gaomon
                return 137;
            } else if ("라라몬".Equals(digimonName)) {      // Lalamon
                return 154;
            } else if ("팰코몬".Equals(digimonName)) {      // Falcomon
                return 127;
            }
            return 100;
        }
    }
}
