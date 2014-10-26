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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DMOLibrary.Profiles.Joymax {

    public class JMWebInfo : DMOWebProfile {
        private static string STR_RANKING_NODE = "//div[@class='list bbs-ranking']";
        private static string STR_GUILD_ID_REGEX = "(\\/Ranking\\/GuildRankingDetail\\.aspx\\?gid=)(\\d+)(&srvn=)";
        private static string STR_TAMER_ID_REGEX = "(\\/Ranking\\/MainPop\\.aspx\\?tid=)(\\d+)(&srvn=)";

        private static string STR_URL_GUILD_PAGE = "http://dmocp.joymax.com/Ranking/GuildRankingDetail.aspx?gid={0}&srvn={1}";
        private static string STR_URL_TAMER_POPPAGE = "http://dmocp.joymax.com/us/Ranking/MainPop.aspx?tid={0}&srvn={1}";
        private static string STR_URL_GUILD_RANK = "http://dmocp.joymax.com/Ranking/GuildRankingList.aspx?st=0&sw={0}&srvn={1}";
        private static string STR_URL_MERC_SIZE_RANK = "http://dmocp.joymax.com/Ranking/SizeRankingList.aspx?sw={0}&srvn={1}&dtype={2}";
        private static string STR_URL_STARTER_RANK = "http://dmocp.joymax.com/Ranking/PartnerRankingList.aspx?sw={0}&srvn={1}";

        public JMWebInfo(DMODatabase Database) {
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

                string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_RANK, guildName, "srv" + serv.Id));
                if (html == string.Empty) {
                    OnCompleted(DMODownloadResultCode.WEB_ACCESS_ERROR, guildInfo);
                    return guildInfo;
                }
                doc.LoadHtml(html);

                HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
                HtmlNodeCollection tlist = ranking.SelectNodes("//tr/td[@class='guild']");
                bool isFound = false;
                if (tlist != null) {
                    HtmlNode e = null;
                    for (int i = 0; i <= tlist.Count - 2; i++) {
                        try { e = ranking.SelectNodes("//td[@class='guild']")[i]; } catch { };
                        if (e != null)
                            if (ClearStr(e.InnerText) == guildName) {
                                Regex r = new Regex(STR_GUILD_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                Match m = r.Match(ranking.SelectNodes("//td[@class='detail']")[i].InnerHtml);
                                if (m.Success) {
                                    guildInfo.Id = Convert.ToInt32(m.Groups[2].ToString());
                                    string master = ranking.SelectNodes("//td[@class='master']")[i].InnerText;
                                    master = master.Substring(0, master.IndexOf(' '));
                                    guildInfo.MasterName = master;
                                    guildInfo.ServId = serv.Id;
                                    guildInfo.Name = guildName;
                                    guildInfo.Rank = Convert.ToInt32(ranking.SelectNodes("//td[@class='ranking']")[i].InnerText);
                                    guildInfo.Rep = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='reputation']")[i].InnerText));
                                    isFound = true;
                                }
                            }
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
                } else {
                    OnCompleted(DMODownloadResultCode.NOT_FOUND, guildInfo);//wrong web page
                    return guildInfo;
                }
            }
            Guild empty = new Guild { Id = -1 };
            OnCompleted(DMODownloadResultCode.DB_CONNECT_ERROR, empty); //can't connect to database
            return empty;
        }

        protected override bool GetGuildInfo(ref Guild guild, bool isDetailed) {
            List<Tamer> tamerList = new List<Tamer>();
            HtmlDocument doc = new HtmlDocument();

            string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_PAGE, guild.Id.ToString(), "srv" + guild.ServId));
            if (html == string.Empty) {
                return false;
            }
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection tlist = ranking.SelectNodes("//tr/td[@class='level']");
            for (int i = 0; i <= tlist.Count - 1; i++) {
                Tamer tamerInfo = new Tamer();
                tamerInfo.Name = ClearStr(ranking.SelectNodes("//td[@class='guild']")[i].InnerText);
                OnStatusChanged(DMODownloadStatusCode.GETTING_TAMER, tamerInfo.Name, i, tlist.Count - 1);
                tamerInfo.ServId = guild.ServId;
                tamerInfo.GuildId = guild.Id;
                tamerInfo.Lvl = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='level']")[i].InnerText));
                string tamerType = ranking.SelectNodes("//td[@class='guild']//img")[i].GetAttributeValue("src", null);
                tamerType = tamerType.Substring(tamerType.LastIndexOf('/') + 1, tamerType.LastIndexOf('.') - tamerType.LastIndexOf('/') - 1);
                tamerInfo.TypeId = Convert.ToInt32(tamerType);

                tamerInfo.PartnerName = ClearStr(ranking.SelectNodes("//td[@class='partner']")[i].InnerText);
                tamerInfo.Rank = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='ranking']")[i].InnerText));

                Regex r = new Regex(STR_TAMER_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match m = r.Match(ranking.SelectNodes("//td[@class='detail']")[i].InnerHtml);

                if (m.Success) {
                    tamerInfo.Id = Convert.ToInt32(m.Groups[2].ToString());
                    tamerInfo.Digimons = GetDigimons(tamerInfo, isDetailed);
                    if (tamerInfo.Digimons.Count == 0) {
                        return false;
                    }
                    tamerList.Add(tamerInfo);
                    if (tamerInfo.Name == guild.MasterName) {
                        guild.MasterId = tamerInfo.Id;
                    }
                }
            }
            if (tamerList.Count == 0) {
                return false;
            }
            guild.Members = tamerList;
            return true;
        }

        protected override List<Digimon> GetDigimons(Tamer tamer, bool isDetailed) {
            List<Digimon> digimonList = new List<Digimon>();
            HtmlDocument doc = new HtmlDocument();

            string html = WebDownload.GetHTML(string.Format(STR_URL_TAMER_POPPAGE, tamer.Id.ToString(), "srv" + tamer.ServId.ToString()));
            if (html == string.Empty) {
                return digimonList;
            }
            doc.LoadHtml(html);

            //getting starter
            HtmlNode tamerInfo = doc.DocumentNode.SelectNodes("//div[@class='tamer-area']")[0];
            Digimon partnerInfo = new Digimon();
            partnerInfo.TamerId = tamer.Id;
            partnerInfo.ServId = tamer.ServId;
            partnerInfo.Name = ClearStr(tamerInfo.SelectNodes("//ul/li[@class='partner']/span")[0].InnerText);
            if (!StarterInfo(ref partnerInfo, tamer.Name)) {
                return digimonList;
            }
            digimonList.Add(partnerInfo);

            HtmlNode mercenaryList = doc.DocumentNode.SelectNodes("//div[@id='rankingscroll']")[0];
            HtmlNodeCollection dlist = mercenaryList.SelectNodes("//li/em[@class='partner']");

            if (dlist != null) {
                for (int i = 0; i <= dlist.Count - 1; i++) {
                    Digimon digimonInfo = new Digimon();
                    digimonInfo.TamerId = tamer.Id;
                    digimonInfo.ServId = tamer.ServId;
                    digimonInfo.Name = ClearStr(mercenaryList.SelectNodes("//em[@class='partner']")[i].InnerText);
                    List<DigimonType> types = null;
                    if (Database.OpenConnection()) {
                        types = Database.GetDigimonTypesByName(digimonInfo.Name);
                        Database.CloseConnection();
                    }
                    if (types == null) {
                        continue;
                    }
                    digimonInfo.TypeId = types[0].Id;
                    digimonInfo.Lvl = Convert.ToInt32(ClearStr(mercenaryList.SelectNodes("//span[@class='level']")[i].InnerText));
                    digimonInfo.Rank = Convert.ToInt32(ClearStr(mercenaryList.SelectNodes("//span[@class='ranking']")[i].InnerText));
                    if (isDetailed)
                        DigimonInfo(ref digimonInfo, tamer.Name);
                    digimonList.Add(digimonInfo);
                }
            }
            return digimonList;
        }

        protected override bool StarterInfo(ref Digimon digimon, string tamerName) {
            HtmlDocument doc = new HtmlDocument();
            digimon.SizePc = 100;
            digimon.SizeCm = ResolveStartedSize(digimon.Name);

            string html = WebDownload.GetHTML(string.Format(STR_URL_STARTER_RANK, tamerName, "srv" + digimon.ServId));
            if (html == string.Empty) {
                return false;
            }
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection dlist = ranking.SelectNodes("//tr/td[@class='tamer2']");

            if (dlist != null)
                for (int i = 0; i <= dlist.Count - 1; i++) {
                    if (ClearStr(ranking.SelectNodes("//td[@class='tamer2']")[i].InnerText) == tamerName) {
                        if (Database.OpenConnection()) {
                            digimon.TypeId = Database.GetDigimonTypesByName(digimon.Name)[0].Id;
                            Database.CloseConnection();
                        }
                        digimon.Rank = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='ranking']")[i + 3].InnerText));
                        digimon.Name = ClearStr(ranking.SelectNodes("//td[@class='name']")[i + 3].InnerText);
                        digimon.Lvl = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='level']")[i + 3].InnerText));
                        return true;
                    }
                }

            return false;
        }

        protected override bool DigimonInfo(ref Digimon digimon, string tamerName) {
            //we don't need starters info
            foreach (int id in STARTER_IDS) {
                if (digimon.TypeId == id) {
                    return false;
                }
            }
            HtmlDocument doc = new HtmlDocument();
            List<DigimonType> digimonTypes = new List<DigimonType>();
            if (Database.OpenConnection()) {
                if (Database.GetDigimonTypeById(digimon.TypeId).Id == -1) {
                    Database.CloseConnection();
                    return false;
                }
                digimonTypes = Database.GetDigimonTypesByName(digimon.Name);
                Database.CloseConnection();
            }

            foreach (DigimonType digimonType in digimonTypes) {
                string html = WebDownload.GetHTML(string.Format(STR_URL_MERC_SIZE_RANK, tamerName, "srv" + digimon.ServId.ToString(), digimonType.Id.ToString()));
                if (html == string.Empty) {
                    continue;
                }
                doc.LoadHtml(html);

                HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
                HtmlNodeCollection dlist = ranking.SelectNodes("//tr/td[@class='tamer2']");
                string size;

                if (dlist != null)
                    for (int i = 0; i <= dlist.Count - 1; i++) {
                        if (ClearStr(ranking.SelectNodes("//td[@class='tamer2']")[i].InnerText) == tamerName) {
                            digimon.TypeId = digimonType.Id;
                            size = ranking.SelectNodes("//td[@class='size']")[i + 3].InnerText.Replace("cm", "");
                            string size_cm = size.Substring(0, size.IndexOf(' '));
                            double.TryParse(size_cm.Replace('.', ','), out digimon.SizeCm);
                            digimon.SizePc = Convert.ToInt32(size.Substring(size.LastIndexOf('(') + 1, size.LastIndexOf('%') - size.LastIndexOf('(') - 1));
                            digimon.SizeRank = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='ranking']")[i + 3].InnerText));
                            digimon.Name = ClearStr(ranking.SelectNodes("//td[@class='name']")[i + 3].InnerText);
                            return true;
                        }
                    }
            }
            return false;
        }

        private static string ClearStr(string str) {
            return str.Replace(",", string.Empty).Replace(" ", string.Empty);
        }

        private static double ResolveStartedSize(string digimonName) {
            if ("Agumon".Equals(digimonName)) {
                return 117;
            } else if ("Gaomon".Equals(digimonName)) {
                return 137;
            } else if ("Lalamon".Equals(digimonName)) {
                return 154;
            } else if ("Falcomon".Equals(digimonName)) {
                return 127;
            }
            return 100;
        }
    }
}
