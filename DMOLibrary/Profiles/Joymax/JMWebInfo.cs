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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DMOLibrary.Database.Context;
using DMOLibrary.Database.Entity;
using HtmlAgilityPack;

namespace DMOLibrary.Profiles.Joymax {

    public class JMWebInfo : DMOWebProfile {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(JMWebInfo));
        private static string STR_RANKING_NODE = "//div[@class='list bbs-ranking']";
        private static string STR_GUILD_ID_REGEX = "(\\/Ranking\\/GuildRankingDetail\\.aspx\\?gid=)(\\d+)(&srvn=)";
        private static string STR_TAMER_ID_REGEX = "(\\/Ranking\\/MainPop\\.aspx\\?tid=)(\\d+)(&srvn=)";

        private static string STR_URL_GUILD_PAGE = "http://dmocp.joymax.com/Ranking/GuildRankingDetail.aspx?gid={0}&srvn={1}";
        private static string STR_URL_TAMER_POPPAGE = "http://dmocp.joymax.com/us/Ranking/MainPop.aspx?tid={0}&srvn={1}";
        private static string STR_URL_GUILD_RANK = "http://dmocp.joymax.com/Ranking/GuildRankingList.aspx?st=0&sw={0}&srvn={1}";
        private static string STR_URL_MERC_SIZE_RANK = "http://dmocp.joymax.com/Ranking/SizeRankingList.aspx?sw={0}&srvn={1}&dtype={2}";
        private static string STR_URL_MERC_SIZE_RANK_MAIN = "http://dmocp.joymax.com/Ranking/SizeRankingList.aspx";
        private static string STR_URL_STARTER_RANK = "http://dmocp.joymax.com/Ranking/PartnerRankingList.aspx?sw={0}&srvn={1}";

        public JMWebInfo(DMODatabase Database) {
            this.Database = Database;
        }

        public override GuildOld GetGuild(string guildName, Server server, bool isDetailed, int actualDays) {
            if (IsBusy) DispatcherHelper.DoEvents();
            OnStarted();
            //Check actual guild in database
            GuildOld storedGuild = Database.ReadGuild(guildName, server, actualDays);
            if (storedGuild.Id != -1) {
                if (!(isDetailed && !storedGuild.IsDetailed)) {
                    //and return it
                    OnCompleted(DMODownloadResultCode.OK, storedGuild);
                    return storedGuild;
                }
            }
            //else get database from web
            GuildOld guildInfo = new GuildOld();
            guildInfo.Id = -1;
            HtmlDocument doc = new HtmlDocument();

            OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, guildName, 0, 50);

            string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_RANK, guildName, "srv" + server.Identifier));
            if (html == string.Empty) {
                OnCompleted(DMODownloadResultCode.WEB_ACCESS_ERROR, guildInfo);
                return guildInfo;
            }
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection tlist = ranking.SelectNodes("//tr/td[@class='guild']");
            bool isFound = false;
            if (tlist != null) {
                List<DigimonType> types = GetDigimonTypes();
                foreach (DigimonType type in types) {
                    MainContext.Instance.AddOrUpdateDigimonType(type, false);
                }
                MainContext.Instance.SaveChanges();

                HtmlNode e = null;
                for (int i = 0; i <= tlist.Count - 2; i++) {
                    try {
                        e = ranking.SelectNodes("//td[@class='guild']")[i];
                    } catch {
                    };
                    if (e != null)
                        if (ClearStr(e.InnerText) == guildName) {
                            Regex r = new Regex(STR_GUILD_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            Match m = r.Match(ranking.SelectNodes("//td[@class='detail']")[i].InnerHtml);
                            if (m.Success) {
                                guildInfo.Id = Convert.ToInt32(m.Groups[2].ToString());
                                string master = ranking.SelectNodes("//td[@class='master']")[i].InnerText;
                                master = master.Substring(0, master.IndexOf(' '));
                                guildInfo.MasterName = master;
                                guildInfo.ServId = server.Identifier;
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
                    Database.WriteGuild(guildInfo, isDetailed);
                    storedGuild = Database.ReadGuild(guildName, server, actualDays);
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

        protected override bool GetGuildInfo(ref GuildOld guild, bool isDetailed) {
            List<TamerOld> tamerList = new List<TamerOld>();
            HtmlDocument doc = new HtmlDocument();
            LOGGER.InfoFormat("Obtaining info of {0}", guild.Name);
            string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_PAGE, guild.Id.ToString(), "srv" + guild.ServId));
            if (html == string.Empty) {
                return false;
            }
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection tlist = ranking.SelectNodes("//tr/td[@class='level']");
            for (int i = 0; i <= tlist.Count - 1; i++) {
                TamerOld tamerInfo = new TamerOld();
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
                    // The FailMax website is soooo stupid, so sometimes it doesn't shows the digimon list of some tamers
                    /*if (tamerInfo.Digimons.Count == 0) {
                        return false;
                    }*/
                    tamerList.Add(tamerInfo);
                    LOGGER.InfoFormat("Found tamer \"{0}\"", tamerInfo.Name);
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

        protected override List<DigimonOld> GetDigimons(TamerOld tamer, bool isDetailed) {
            LOGGER.InfoFormat("Obtaining digimons for tamer \"{0}\"", tamer.Name);
            List<DigimonOld> digimonList = new List<DigimonOld>();
            HtmlDocument doc = new HtmlDocument();

            string html = WebDownload.GetHTML(string.Format(STR_URL_TAMER_POPPAGE, tamer.Id.ToString(), "srv" + tamer.ServId.ToString()));
            if (html == string.Empty) {
                LOGGER.ErrorFormat("Obtaining digimons for tamer \"{0}\" failed", tamer.Name);
                return digimonList;
            }
            doc.LoadHtml(html);

            //getting starter
            HtmlNode tamerInfo = doc.DocumentNode.SelectNodes("//div[@class='tamer-area']")[0];
            DigimonOld partnerInfo = new DigimonOld();
            partnerInfo.TamerId = tamer.Id;
            partnerInfo.ServId = tamer.ServId;
            partnerInfo.Name = ClearStr(tamerInfo.SelectNodes("//ul/li[@class='partner']/span")[0].InnerText);
            if (StarterInfo(ref partnerInfo, tamer.Name)) {
                digimonList.Add(partnerInfo);
            } else {
                LOGGER.ErrorFormat("Unable to obtain starter digimon \"{0}\" for tamer \"{1}\"", partnerInfo.Name, tamer.Name);
            }

            HtmlNode mercenaryList = doc.DocumentNode.SelectNodes("//div[@id='rankingscroll']")[0];
            HtmlNodeCollection dlist = mercenaryList.SelectNodes("//li/em[@class='partner']");

            if (dlist != null) {
                for (int i = 0; i <= dlist.Count - 1; i++) {
                    DigimonOld digimonInfo = new DigimonOld();
                    digimonInfo.TamerId = tamer.Id;
                    digimonInfo.ServId = tamer.ServId;
                    digimonInfo.Name = ClearStr(mercenaryList.SelectNodes("//em[@class='partner']")[i].InnerText);
                    List<DigimonType> types = null;
                    string searchName = MainContext.PrepareDigimonSearch(digimonInfo.Name);
                    types = MainContext.Instance.FindDigimonTypesBySearchGDMO(searchName);
                    if (types == null) {
                        continue;
                    }
                    digimonInfo.TypeId = types[0].Code;
                    digimonInfo.Lvl = Convert.ToInt32(ClearStr(mercenaryList.SelectNodes("//span[@class='level']")[i].InnerText));
                    digimonInfo.Rank = Convert.ToInt32(ClearStr(mercenaryList.SelectNodes("//span[@class='ranking']")[i].InnerText));

                    if (digimonList.Count(d => d.TypeId.Equals(digimonInfo.TypeId)) == 0) {
                        if (isDetailed) {
                            if (!DigimonInfo(ref digimonInfo, tamer.Name)) {
                                LOGGER.ErrorFormat("Unable to obtain detailed data of digimon \"{0}\" for tamer \"{1}\"", digimonInfo.Name, tamer.Name);
                            }
                        }
                        digimonList.Add(digimonInfo);
                        LOGGER.Info(String.Format("Found digimon \"{0}\"", digimonInfo.Name));
                    }
                }
            }
            return digimonList;
        }

        protected override bool StarterInfo(ref DigimonOld digimon, string tamerName) {
            LOGGER.InfoFormat("Obtaining starter digimon for tamer \"{0}\"", tamerName);
            HtmlDocument doc = new HtmlDocument();
            digimon.SizePc = 100;
            digimon.SizeCm = ResolveStartedSize(digimon.Name);

            string html = WebDownload.GetHTML(string.Format(STR_URL_STARTER_RANK, tamerName, "srv" + digimon.ServId));
            if (html == string.Empty) {
                LOGGER.ErrorFormat("Obtaining starter digimon for tamer \"{0}\" failed", tamerName);
                return false;
            }
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection dlist = ranking.SelectNodes("//tr/td[@class='tamer2']");

            if (dlist != null)
                for (int i = 0; i <= dlist.Count - 1; i++) {
                    if (ClearStr(ranking.SelectNodes("//td[@class='tamer2']")[i].InnerText) == tamerName) {
                        string searchName = MainContext.PrepareDigimonSearch(digimon.Name);
                        List<DigimonType> types = MainContext.Instance.FindDigimonTypesBySearchGDMO(searchName);
                        if (types != null) {
                            if (types.Count > 0) {
                                digimon.TypeId = types[0].Code;
                            }
                        }
                        digimon.Rank = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='ranking']")[i + 3].InnerText));
                        digimon.Name = ClearStr(ranking.SelectNodes("//td[@class='name']")[i + 3].InnerText);
                        digimon.Lvl = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='level']")[i + 3].InnerText));
                        return true;
                    }
                }

            return false;
        }

        protected override bool DigimonInfo(ref DigimonOld digimon, string tamerName) {
            //we don't need starters info
            foreach (int id in STARTER_IDS) {
                if (digimon.TypeId == id) {
                    return false;
                }
            }
            LOGGER.InfoFormat("Obtaining detailed data of digimon \"{0}\" for tamer \"{1}\"", digimon.Name, tamerName);

            DigimonType tryType = MainContext.Instance.FindDigimonTypeByCode(digimon.TypeId);
            if (tryType == null) {
                return false;
            }
            DigimonType digimonType = (DigimonType)tryType;

            string html = WebDownload.GetHTML(string.Format(STR_URL_MERC_SIZE_RANK, tamerName, "srv" + digimon.ServId.ToString(), digimonType.Id.ToString()));
            if (html == string.Empty) {
                return false;
            }
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection dlist = ranking.SelectNodes("//tr/td[@class='tamer2']");
            string size;

            if (dlist != null) {
                for (int i = 0; i <= dlist.Count - 1; i++) {
                    if (ClearStr(ranking.SelectNodes("//td[@class='tamer2']")[i].InnerText) == tamerName) {
                        digimon.TypeId = digimonType.Code;
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

        public override List<DigimonType> GetDigimonTypes() {
            List<DigimonType> dTypes = new List<DigimonType>();

            string html = WebDownload.GetHTML(STR_URL_MERC_SIZE_RANK_MAIN);
            if (html == string.Empty) {
                return dTypes;
            }
            HtmlDocument doc = new HtmlDocument();
            HtmlNode.ElementsFlags.Remove("option");
            doc.LoadHtml(html);

            HtmlNode selectTypes = doc.GetElementbyId("cphContents_ddDigimonType");
            foreach (HtmlNode type in selectTypes.ChildNodes) {
                if (!"option".Equals(type.Name)) {
                    continue;
                }
                DigimonType dType = new DigimonType() {
                    Code = Convert.ToInt32(type.Attributes["value"].Value),
                    Name = type.InnerText
                };
                dTypes.Add(dType);
                LOGGER.DebugFormat("Found {0}", dType);
            }
            return dTypes;
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