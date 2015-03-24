// ======================================================================
// DMOLibrary
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using DMOLibrary.Database.Context;
using DMOLibrary.Database.Entity;
using HtmlAgilityPack;

namespace DMOLibrary.Profiles.Joymax {

    public class JoymaxWebProfile : AbstractWebProfile {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(JoymaxWebProfile));
        private static string STR_RANKING_NODE = "//div[@class='list bbs-ranking']";
        private static string STR_GUILD_ID_REGEX = "(\\/Ranking\\/GuildRankingDetail\\.aspx\\?gid=)(\\d+)(&srvn=)";
        private static string STR_TAMER_ID_REGEX = "(\\/Ranking\\/MainPop\\.aspx\\?tid=)(\\d+)(&srvn=)";

        private static string STR_URL_TAMER_POPPAGE = "http://dmocp.joymax.com/us/Ranking/MainPop.aspx?tid={0}&srvn={1}";
        private static string STR_URL_GUILD_PAGE = "http://dmocp.joymax.com/Ranking/GuildRankingDetail.aspx?gid={0}&srvn={1}";
        private static string STR_URL_GUILD_RANK = "http://dmocp.joymax.com/Ranking/GuildRankingList.aspx?st=0&sw={0}&srvn={1}";
        private static string STR_URL_MERC_SIZE_RANK = "http://dmocp.joymax.com/Ranking/SizeRankingList.aspx?sw={0}&srvn={1}&dtype={2}";
        private static string STR_URL_MERC_SIZE_RANK_MAIN = "http://dmocp.joymax.com/Ranking/SizeRankingList.aspx";
        private static string STR_URL_STARTER_RANK = "http://dmocp.joymax.com/Ranking/PartnerRankingList.aspx?sw={0}&srvn={1}";

        public override Guild GetGuild(Server server, string guildName, bool isDetailed) {
            OnStarted();
            Guild guild = new Guild() {
                Server = server,
                IsDetailed = isDetailed
            };
            HtmlDocument doc = new HtmlDocument();
            OnStatusChanged(DMODownloadStatusCode.GETTING_GUILD, guildName, 0, 50);
            try {
                string html = DownloadContent(string.Format(STR_URL_GUILD_RANK, guildName, "srv" + server.Identifier));
                doc.LoadHtml(html);

                HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
                HtmlNodeCollection tlist = ranking.SelectNodes("//tr/td[@class='guild']");
                bool isFound = false;
                if (tlist != null) {
                    List<DigimonType> types = GetDigimonTypes();
                    using (MainContext context = new MainContext()) {
                        foreach (DigimonType type in types) {
                            context.AddOrUpdateDigimonType(type, false);
                        }
                        context.SaveChanges();
                    }

                    HtmlNode e = null;
                    string guildMaster = null;
                    for (int i = 0; i < tlist.Count - 1; i++) {
                        try {
                            e = ranking.SelectNodes("//td[@class='guild']")[i];
                        } catch {
                        };
                        if (e != null)
                            if (ClearStr(e.InnerText) == guildName) {
                                Regex r = new Regex(STR_GUILD_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                                Match m = r.Match(ranking.SelectNodes("//td[@class='detail']")[i].InnerHtml);
                                if (m.Success) {
                                    guild.Identifier = Convert.ToInt32(m.Groups[2].ToString());
                                    guildMaster = ranking.SelectNodes("//td[@class='master']")[i].InnerText;
                                    guildMaster = guildMaster.Substring(0, guildMaster.IndexOf(' '));
                                    guild.Name = guildName;
                                    guild.Rank = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='ranking']")[i].InnerText));
                                    guild.Rep = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='reputation']")[i].InnerText));
                                    isFound = true;
                                    break;
                                }
                            }
                    }
                    if (!isFound) {
                        OnCompleted(DMODownloadResultCode.NOT_FOUND, guild); // guild not found
                        return guild;
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
                } else {
                    OnCompleted(DMODownloadResultCode.NOT_FOUND, guild);//wrong web page
                    return guild;
                }
            } catch (WebException e) {
                OnCompleted(DMODownloadResultCode.WEB_ACCESS_ERROR, guild);
                return guild;
            }
        }

        protected override bool GetGuildInfo(ref Guild guild, bool isDetailed) {
            List<Tamer> tamerList = new List<Tamer>();
            HtmlDocument doc = new HtmlDocument();
            LOGGER.InfoFormat("Obtaining info of {0}", guild.Name);
            string html = DownloadContent(string.Format(STR_URL_GUILD_PAGE, guild.Identifier, "srv" + guild.Server.Identifier));
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection tlist = ranking.SelectNodes("//tr/td[@class='level']");
            using (MainContext context = new MainContext()) {
                for (int i = 0; i < tlist.Count; i++) {
                    Tamer tamer = new Tamer() {
                        Guild = guild,
                        Name = ClearStr(ranking.SelectNodes("//td[@class='guild']")[i].InnerText),
                        Level = Convert.ToByte(ClearStr(ranking.SelectNodes("//td[@class='level']")[i].InnerText)),
                        Rank = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='ranking']")[i].InnerText))
                    };
                    OnStatusChanged(DMODownloadStatusCode.GETTING_TAMER, tamer.Name, i, tlist.Count - 1);

                    string tamerType = ranking.SelectNodes("//td[@class='guild']//img")[i].GetAttributeValue("src", null);
                    tamerType = tamerType.Substring(tamerType.LastIndexOf('/') + 1, tamerType.LastIndexOf('.') - tamerType.LastIndexOf('/') - 1);
                    tamer.Type = context.FindTamerTypeByCode(Convert.ToInt32(tamerType));

                    Regex r = new Regex(STR_TAMER_ID_REGEX, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match m = r.Match(ranking.SelectNodes("//td[@class='detail']")[i].InnerHtml);
                    if (m.Success) {
                        tamer.AccountId = Convert.ToInt32(m.Groups[2].ToString());
                        tamer.Digimons = GetDigimons(tamer, isDetailed);

                        Digimon partner = tamer.Digimons.FirstOrDefault(d => d.Type.IsStarter);
                        if (partner != null) {
                            partner.Name = ClearStr(ranking.SelectNodes("//td[@class='partner']")[i].InnerText);
                        }

                        tamerList.Add(tamer);
                        LOGGER.InfoFormat("Found tamer \"{0}\"", tamer.Name);
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

            string html = DownloadContent(string.Format(STR_URL_TAMER_POPPAGE, tamer.AccountId, "srv" + tamer.Guild.Server.Identifier));
            doc.LoadHtml(html);

            using (MainContext context = new MainContext()) {
                //getting starter
                HtmlNode tamerInfo = doc.DocumentNode.SelectNodes("//div[@class='tamer-area']")[0];
                Digimon partner = new Digimon() {
                    Tamer = tamer,
                    SizePc = 100,
                    Name = ClearStr(tamerInfo.SelectNodes("//ul/li[@class='partner']/span")[0].InnerText)
                };
                partner.Type = context.FindDigimonTypeBySearchGDMO(MainContext.PrepareDigimonSearch(partner.Name));
                partner.SizeCm = partner.Type.SizeCm;
                digimonList.Add(partner);
                if (!GetStarterInfo(ref partner, tamer)) {
                    LOGGER.ErrorFormat("Unable to obtain info about starter digimon \"{0}\" for tamer \"{1}\"", partner.Name, tamer.Name);
                }

                HtmlNode mercenaryList = doc.DocumentNode.SelectNodes("//div[@id='rankingscroll']")[0];
                HtmlNodeCollection dlist = mercenaryList.SelectNodes("//li/em[@class='partner']");

                if (dlist != null) {
                    for (int i = 0; i < dlist.Count; i++) {
                        Digimon digimonInfo = new Digimon() {
                            Tamer = tamer,
                            Name = ClearStr(mercenaryList.SelectNodes("//em[@class='partner']")[i].InnerText),
                            Level = Convert.ToByte(ClearStr(mercenaryList.SelectNodes("//span[@class='level']")[i].InnerText)),
                            Rank = Convert.ToInt32(ClearStr(mercenaryList.SelectNodes("//span[@class='ranking']")[i].InnerText))
                        };
                        digimonInfo.Type = context.FindDigimonTypeBySearchGDMO(MainContext.PrepareDigimonSearch(digimonInfo.Name));
                        if (digimonInfo.Type == null) {
                            continue;
                        }
                        digimonInfo.SizeCm = digimonInfo.Type.SizeCm;

                        if (digimonList.Count(d => d.Type.Equals(digimonInfo.Type)) == 0) {
                            if (isDetailed) {
                                if (!GetMercenaryInfo(ref digimonInfo, tamer)) {
                                    LOGGER.ErrorFormat("Unable to obtain detailed data of digimon \"{0}\" for tamer \"{1}\"", digimonInfo.Name, tamer.Name);
                                }
                            }
                            digimonList.Add(digimonInfo);
                            LOGGER.Info(String.Format("Found digimon \"{0}\"", digimonInfo.Name));
                        }
                    }
                }
            }
            return digimonList;
        }

        protected override bool GetStarterInfo(ref Digimon digimon, Tamer tamer) {
            if (digimon.Type == null) {
                return false;
            }
            LOGGER.InfoFormat("Obtaining starter digimon for tamer \"{0}\"", tamer.Name);
            HtmlDocument doc = new HtmlDocument();

            string html = DownloadContent(string.Format(STR_URL_STARTER_RANK, tamer.Name, "srv" + tamer.Guild.Server.Identifier));
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection dlist = ranking.SelectNodes("//tr/td[@class='tamer2']");

            if (dlist != null) {
                for (int i = 0; i < dlist.Count; i++) {
                    if (ClearStr(ranking.SelectNodes("//td[@class='tamer2']")[i].InnerText) == tamer.Name) {
                        digimon.Rank = Convert.ToInt32(ClearStr(ranking.SelectNodes("//td[@class='ranking']")[i + 3].InnerText));
                        digimon.Name = ClearStr(ranking.SelectNodes("//td[@class='name']")[i + 3].InnerText);
                        digimon.Level = Convert.ToByte(ClearStr(ranking.SelectNodes("//td[@class='level']")[i + 3].InnerText));
                        return true;
                    }
                }
            }
            return false;
        }

        protected override bool GetMercenaryInfo(ref Digimon digimon, Tamer tamer) {
            //we don't need starters info
            if (digimon.Type.IsStarter) {
                return false;
            }
            LOGGER.InfoFormat("Obtaining detailed data of digimon \"{0}\" for tamer \"{1}\"", digimon.Name, tamer.Name);

            string html = DownloadContent(string.Format(STR_URL_MERC_SIZE_RANK, tamer.Name, "srv" + tamer.Guild.Server.Identifier.ToString(), digimon.Type.Code));
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode ranking = doc.DocumentNode.SelectNodes(STR_RANKING_NODE)[0];
            HtmlNodeCollection dlist = ranking.SelectNodes("//tr/td[@class='tamer2']");

            if (dlist != null) {
                for (int i = 0; i < dlist.Count; i++) {
                    if (ClearStr(ranking.SelectNodes("//td[@class='tamer2']")[i].InnerText) == tamer.Name) {
                        string size = ranking.SelectNodes("//td[@class='size']")[i + 3].InnerText.Replace("cm", "");
                        string size_cm = size.Substring(0, size.IndexOf(' '));
                        double SizeCm = 0;
                        double.TryParse(size_cm.Replace('.', ','), out SizeCm);
                        digimon.SizeCm = SizeCm;
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

            string html = DownloadContent(STR_URL_MERC_SIZE_RANK_MAIN);
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
    }
}