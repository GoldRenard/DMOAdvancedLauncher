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

namespace DMOLibrary.Profiles.Korea {

    public class KoreaWebProfile : AbstractWebProfile {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(KoreaWebProfile));
        private static string STR_GUILD_ID_REGEX = "(main_sub\\.aspx\\?v=)(\\d+)(&o=)(\\d+)";
        private static string STR_TAMER_LVL_REGEX = "(\\/images\\/comm\\/icon\\/lv_)(\\d+)(\\.gif)";
        private static string STR_TAMER_TYPE_REGEX = "(\\/images\\/ranking\\/icon\\/)(\\d+)(\\.gif)";
        private static string STR_TAMER_ID_REGEX = "(\\/ranking\\/main_pop\\.aspx\\?o=)(\\d+)(&type=)";
        private static string STR_DIGIMON_SIZE = "(\\d+)( cm \\()(\\d+)(%\\))";

        private static string STR_URL_TAMER_POPPAGE = "http://www.digimonmasters.com/ranking/main_pop.aspx?o={0}&type={1}00";
        private static string STR_URL_GUILD_RANK = "http://www.digimonmasters.com/guild/main.aspx?type={1}00&searchtype=GUILDNAME&search={0}";
        private static string STR_URL_GUILD_PAGE = "http://www.digimonmasters.com/guild/main_sub.aspx?v={1}00&o={0}";
        private static string STR_URL_MERC_SIZE_RANK = "http://www.digimonmasters.com/ranking/size.aspx?type={1}00&search={0}&digimon={2}";
        private static string STR_URL_MERC_SIZE_RANK_MAIN = "http://www.digimonmasters.com/ranking/size.aspx";
        private static string STR_URL_STARTER_RANK = "http://www.digimonmasters.com/ranking/partner.aspx?type={1}00&search={0}";

        public override Guild GetGuild(Server server, string guildName, bool isDetailed) {
            OnStarted();
            Guild guild = new Guild() {
                Server = server,
                IsDetailed = isDetailed
            };
            HtmlDocument doc = new HtmlDocument();
            OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, guildName, 0, 50);

            string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_RANK, guildName, server.Identifier));
            if (html == string.Empty) {
                OnCompleted(DMODownloadResultCode.WEB_ACCESS_ERROR, guild);
                return guild;
            }
            doc.LoadHtml(html);

            bool isFound = false;
            HtmlNode ranking = doc.DocumentNode;

            string guildMaster = null;

            try {
                ranking = doc.DocumentNode.SelectNodes("//div[@id='body']//table[@class='forum_list'][1]//tbody//tr[not(@onmouseover)]")[4];
                guild.Rank = CheckRankNode(ranking.SelectSingleNode(".//td[1]"));
                guild.Name = ClearStr(ranking.SelectSingleNode(".//td[2]").InnerText);
                guild.Rep = Convert.ToInt32(ClearStr(ranking.SelectSingleNode(".//td[4]").InnerText));
                guildMaster = ClearStr(ranking.SelectSingleNode(".//td[5]").InnerText);

                Regex r1 = new Regex(STR_GUILD_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                string link = ranking.SelectSingleNode(".//td[7]").InnerHtml;
                Match m1 = r1.Match(link);
                if (m1.Success) {
                    guild.Identifier = Convert.ToInt32(m1.Groups[4].ToString());
                    isFound = true;
                }
            } catch {
                isFound = false;
            }

            if (!isFound) {
                OnCompleted(DMODownloadResultCode.NOT_FOUND, guild); // guild not found
                return guild;
            }

            List<DigimonType> types = GetDigimonTypes();
            using (MainContext context = new MainContext()) {
                foreach (DigimonType type in types) {
                    context.AddOrUpdateDigimonType(type, true);
                }
                context.SaveChanges();
            }

            if (GetGuildInfo(ref guild, isDetailed)) {
                guild.Tamers.First(t => t.Name == guildMaster).IsMaster = true;
                guild.UpdateTime = DateTime.Now;
                OnCompleted(DMODownloadResultCode.OK, guild);
                return guild;
            } else {
                OnCompleted(DMODownloadResultCode.CANT_GET, guild); // can't get guild info
                return guild;
            }
        }

        protected override bool GetGuildInfo(ref Guild guild, bool isDetailed) {
            List<Tamer> tamerList = new List<Tamer>();
            HtmlDocument doc = new HtmlDocument();
            LOGGER.InfoFormat("Obtaining info of {0}", guild.Name);
            string html = WebDownload.GetHTML(string.Format(STR_URL_GUILD_PAGE, guild.Identifier, "srv" + guild.Server.Identifier));
            if (html == string.Empty) {
                return false;
            }
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes("//table[@class='forum_list']//tbody")[1];
            HtmlNodeCollection tlist = ranking.SelectNodes(".//tr");
            if (tlist != null) {
                using (MainContext context = new MainContext()) {
                    for (int i = 0; i < tlist.Count; i++) {
                        try {
                            Tamer tamer = new Tamer() {
                                Guild = guild,
                                Name = ClearStr(tlist[i].SelectSingleNode(".//td[3]").InnerText),
                                Rank = CheckRankNode(tlist[i].SelectSingleNode(".//td[2]"))
                            };
                            OnStatusChanged(DMODownloadStatusCode.GETTING_TAMER, tamer.Name, i, tlist.Count);

                            Regex regex = new Regex(STR_TAMER_TYPE_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            Match match = regex.Match(tlist[i].SelectSingleNode(".//td[3]").InnerHtml);
                            if (match.Success) {
                                tamer.Type = context.FindTamerTypeByCode(Convert.ToInt32(match.Groups[2].ToString()));
                            }

                            regex = new Regex(STR_TAMER_LVL_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            match = regex.Match(tlist[i].SelectSingleNode(".//td[4]").InnerHtml);
                            if (match.Success) {
                                tamer.Level = Convert.ToByte(match.Groups[2].ToString());
                            }

                            regex = new Regex(STR_TAMER_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            match = regex.Match(tlist[i].SelectSingleNode(".//td[7]").InnerHtml);
                            if (match.Success) {
                                tamer.AccountId = Convert.ToInt32(match.Groups[2].ToString());
                            }

                            if (tamer.Level != 0 && tamer.AccountId != 0) {
                                tamer.Digimons = GetDigimons(tamer, isDetailed);
                                if (tamer.Digimons.Count == 0) {
                                    return false;
                                }
                                Digimon partner = tamer.Digimons.FirstOrDefault(d => d.Type.IsStarter);
                                if (partner != null) {
                                    partner.Name = ClearStr(tlist[i].SelectSingleNode(".//td[5]").InnerText);
                                }
                                tamerList.Add(tamer);
                                LOGGER.InfoFormat("Found tamer \"{0}\"", tamer.Name);
                            }
                        } catch {
                        }
                    }
                }
            }

            if (tamerList.Count == 0) {
                return false;
            }
            guild.Tamers = tamerList;
            return true;
        }

        protected override List<Digimon> GetDigimons(Tamer tamer, bool isDetailed) {
            LOGGER.InfoFormat("Obtaining digimons for tamer \"{0}\"", tamer.Name);
            List<Digimon> digimonList = new List<Digimon>();
            HtmlDocument doc = new HtmlDocument();

            string html = WebDownload.GetHTML(string.Format(STR_URL_TAMER_POPPAGE, tamer.AccountId, tamer.Guild.Server.Identifier));
            if (html == string.Empty) {
                LOGGER.ErrorFormat("Obtaining digimons for tamer \"{0}\" failed", tamer.Name);
                return digimonList;
            }
            doc.LoadHtml(html);

            //getting starter
            Digimon partner;
            try {
                partner = new Digimon() {
                    Tamer = tamer,
                    Name = ClearStr(doc.DocumentNode.SelectSingleNode("//div[1]//div[2]//div[2]//table[1]//tr[1]//td[2]//table[1]//tr[3]//td[2]//b").InnerText),
                    SizePc = 100
                };
            } catch {
                return digimonList;
            }

            DigimonType type = null;
            using (MainContext context = new MainContext()) {
                type = context.FindDigimonTypeBySearchKDMO(MainContext.PrepareDigimonSearch(partner.Name));
            }
            if (type != null) {
                partner.Type = type;
                partner.Name = type.Name;
                partner.SizeCm = type.SizeCm;
            }
            digimonList.Add(partner);
            if (!GetStarterInfo(ref partner, tamer)) {
                LOGGER.ErrorFormat("Unable to obtain info about starter digimon \"{0}\" for tamer \"{1}\"", partner.Name, tamer.Name);
            }

            HtmlNode mercList = doc.DocumentNode.SelectNodes("//table[@class='list']")[1];
            HtmlNodeCollection dlist = mercList.SelectNodes(".//tr");

            if (dlist != null) {
                using (MainContext context = new MainContext()) {
                    for (int i = 1; i < dlist.Count; i++) {
                        Digimon digimonInfo = new Digimon() {
                            Tamer = tamer,
                            Name = ClearStr(dlist[i].SelectSingleNode(".//td[1]").InnerText),
                            Level = Convert.ToByte(ClearStr(dlist[i].SelectSingleNode(".//td[2]//label").InnerText))
                        };

                        string rank = string.Empty;
                        foreach (char c in ClearStr(dlist[i].SelectSingleNode(".//td[3]//label").InnerText)) {
                            if (Char.IsDigit(c)) {
                                rank += c;
                            } else {
                                break;
                            }
                        }
                        digimonInfo.Rank = Convert.ToInt32(rank);
                        digimonInfo.Type = context.FindDigimonTypeBySearchKDMO(MainContext.PrepareDigimonSearch(digimonInfo.Name));
                        if (digimonInfo.Type == null) {
                            continue;
                        }
                        digimonInfo.Name = digimonInfo.Type.Name;
                        digimonInfo.SizeCm = digimonInfo.Type.SizeCm;

                        if (digimonList.Count(d => d.Type.Equals(digimonInfo.Type)) == 0) {
                            if (isDetailed) {
                                if (!GetMercenaryInfo(ref digimonInfo, tamer)) {
                                    LOGGER.ErrorFormat("Unable to obtain detailed data of digimon \"{0}\" for tamer \"{1}\"", digimonInfo.Name, tamer.Name);
                                }
                            }
                            digimonList.Add(digimonInfo);
                            LOGGER.InfoFormat("Found digimon \"{0}\"", digimonInfo.Name);
                        }
                    }
                }
            }
            return digimonList;
        }

        protected override bool GetStarterInfo(ref Digimon digimon, Tamer tamer) {
            LOGGER.InfoFormat("Obtaining starter digimon for tamer \"{0}\"", tamer.Name);
            HtmlDocument doc = new HtmlDocument();
            string html = WebDownload.GetHTML(string.Format(STR_URL_STARTER_RANK, tamer.Name, tamer.Guild.Server.Identifier));
            if (html == string.Empty) {
                LOGGER.ErrorFormat("Obtaining starter digimon for tamer \"{0}\" failed", tamer.Name);
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
                digimon.Rank = CheckRankNode(partnerNode.SelectSingleNode(".//td[1]"));
                digimon.Name = ClearStr(partnerNode.SelectSingleNode(".//td[2]//label").InnerText);
                digimon.Level = Convert.ToByte(ClearStr(partnerNode.SelectSingleNode(".//td[3]").InnerText));
            }
            return partnerNode != null;
        }

        protected override bool GetMercenaryInfo(ref Digimon digimon, Tamer tamer) {
            //we don't need starters info
            if (digimon.Type.IsStarter) {
                return false;
            }
            LOGGER.InfoFormat("Obtaining detailed data of digimon \"{0}\" for tamer \"{1}\"", digimon.Name, tamer.Name);

            string html = WebDownload.GetHTML(string.Format(STR_URL_MERC_SIZE_RANK, tamer.Name, tamer.Guild.Server.Identifier, digimon.Type.Code));
            if (html == string.Empty) {
                return false;
            }
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode partnerNode;
            try {
                partnerNode = doc.DocumentNode.SelectNodes("//table[@class='forum_list']")[1].SelectSingleNode(".//tbody//tr[not(@onmouseover)]");
            } catch {
                return false;
            }

            if (partnerNode != null) {
                digimon.SizeRank = Convert.ToInt32(ClearStr(partnerNode.SelectSingleNode(".//td[1]").InnerText));
                digimon.Name = ClearStr(partnerNode.SelectSingleNode(".//td[2]//label").InnerText);
                Regex r = new Regex(STR_DIGIMON_SIZE, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match m = r.Match(partnerNode.SelectSingleNode(".//td[3]").InnerHtml);
                if (m.Success) {
                    digimon.SizeCm = Convert.ToInt32(m.Groups[1].ToString());
                    digimon.SizePc = Convert.ToInt32(m.Groups[3].ToString());
                    return true;
                }
            }
            return false;
        }

        private static string ClearStr(string str) {
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

        public override List<DigimonType> GetDigimonTypes() {
            List<DigimonType> dTypes = new List<DigimonType>();

            string html = WebDownload.GetHTML(STR_URL_MERC_SIZE_RANK_MAIN);
            if (html == string.Empty) {
                return dTypes;
            }
            HtmlDocument doc = new HtmlDocument();
            HtmlNode.ElementsFlags.Remove("option");
            doc.LoadHtml(html);

            HtmlNode selectTypes = doc.GetElementbyId("drpDigimon");
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
    }
}