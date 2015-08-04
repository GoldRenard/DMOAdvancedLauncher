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

using System.Data.Entity;
using AdvancedLauncher.SDK.Model.Entity;

namespace AdvancedLauncher.Database.Context {

    public class ContextInitializer
#if DEBUG
        //: DropCreateDatabaseAlways<MainContext> {
        : CreateDatabaseIfNotExists<MainContext> {
#endif
#if RELEASE
 : CreateDatabaseIfNotExists<MainContext> {
#endif

        protected override void Seed(MainContext context) {
            SeedServers(context);
            SeedDigimonTypes(context);
            SeedTamerTypes(context);
            base.Seed(context);
        }

        #region Servers Seed

        private void SeedServers(MainContext context) {
            SeedServer(context, 1, "Seraphimon", Server.ServerType.ADMO);

            SeedServer(context, 1, "Leviamon", Server.ServerType.GDMO);
            SeedServer(context, 2, "Lucemon", Server.ServerType.GDMO);
            SeedServer(context, 3, "Lilithmon", Server.ServerType.GDMO);
            SeedServer(context, 4, "Barbamon", Server.ServerType.GDMO);
            SeedServer(context, 5, "Beelzemon", Server.ServerType.GDMO);

            SeedServer(context, 1, "Lucemon", Server.ServerType.KDMO);
            SeedServer(context, 2, "Leviamon", Server.ServerType.KDMO);
            SeedServer(context, 3, "Lilithmon", Server.ServerType.KDMO);
            SeedServer(context, 4, "Barbamon", Server.ServerType.KDMO);

            SeedServer(context, 1, "Lucemon", Server.ServerType.KDMO_IMBC);
            SeedServer(context, 2, "Leviamon", Server.ServerType.KDMO_IMBC);
            SeedServer(context, 3, "Lilithmon", Server.ServerType.KDMO_IMBC);
            SeedServer(context, 4, "Barbamon", Server.ServerType.KDMO_IMBC);
        }

        private void SeedServer(MainContext context, byte id, string name, Server.ServerType type) {
            context.Servers.Add(new Server() {
                Identifier = id,
                Name = name,
                Type = type
            });
        }

        #endregion Servers Seed

        #region Digimon Types Seed

        private void SeedDigimonTypes(MainContext context) {
            SeedDigimonType(context, 31003, true, 137, "Gaomon", null, "가오몬", "gaomon", "가오몬");
            SeedDigimonType(context, 31002, true, 154, "Lalamon", null, "라라몬", "lalamon", "라라몬");
            SeedDigimonType(context, 31004, true, 127, "Falcomon", null, "팰코몬", "falcomon", "팰코몬");
            SeedDigimonType(context, 31001, true, 117, "Agumon", null, "아구몬", "agumon", "아구몬");
            SeedDigimonType(context, 31007, "Agumon(Classic)", null, "아구몬클래식", "agumonclassic", "아구몬클래식");
            SeedDigimonType(context, 31043, "Agumon(Black)", null, "아구몬(흑)", "agumonblack", "아구몬흑");
            SeedDigimonType(context, 32019, "Salamon(BlackGatomon)", null, "플롯트몬(블랙가트몬)", "salamonblackgatomon", "플롯트몬블랙가트몬");
            SeedDigimonType(context, 31019, "Salamon", null, "플롯트몬(가트몬)", "salamon", "플롯트몬가트몬");
            SeedDigimonType(context, 31138, "Gabumon", null, "파피몬", "gabumon", "파피몬");
            SeedDigimonType(context, 31068, "Gabumon(Black)", null, "파피몬(흑)", "gabumonblack", "파피몬흑");
            SeedDigimonType(context, 31008, "ExV-mon", null, "브이몬(엑스브이몬)", "exvmon", "브이몬엑스브이몬");
            SeedDigimonType(context, 32008, "Veedramon", null, "브이몬(브이드라몬)", "veedramon", "브이몬브이드라몬");
            SeedDigimonType(context, 31013, "PawnChessmonWhite", null, "폰체스몬W", "pawnchessmonwhite", "폰체스몬W");
            SeedDigimonType(context, 31009, "PawnChessmonBlack", null, "폰체스몬B", "pawnchessmonblack", "폰체스몬B");
            SeedDigimonType(context, 31067, "DemiDevimon", "Devimon", "피코데블몬(고스몬)", "demidevimon", "피코데블몬고스몬");
            SeedDigimonType(context, 32067, "DemiDevimon", "Soulmon", "피코데블몬(소울몬)", "demidevimon", "피코데블몬소울몬");
            SeedDigimonType(context, 32021, "Mosyamon", null, "코테몬(무사몬계열)", "mosyamon", "코테몬무사몬계열");
            SeedDigimonType(context, 31021, "Kotemon", null, "코테몬(그라디몬계열)", "kotemon", "코테몬그라디몬계열");
            SeedDigimonType(context, 31014, "Dracomon", "Blue", "드라코몬(청)", "dracomonblue", "드라코몬청");
            SeedDigimonType(context, 32014, "Dracomon", "Green", "드라코몬(녹)", "dracomongreen", "드라코몬녹");
            SeedDigimonType(context, 31041, "Palmon", "Woodmon", "팔몬(우드몬)", "palmon", "팔몬우드몬");
            SeedDigimonType(context, 32041, "Palmon", "Togemon", "팔몬(니드몬)", "palmon", "팔몬니드몬");
            SeedDigimonType(context, 31033, "Goblimon", null, "고부리몬", "goblimon", "고부리몬");
            SeedDigimonType(context, 31032, "Sharmamon", null, "원시고부리몬", "sharmamon", "원시고부리몬");
            SeedDigimonType(context, 31029, "Kunemon", null, "쿠네몬", "kunemon", "쿠네몬");
            SeedDigimonType(context, 31038, "Dokunemon", null, "도쿠네몬", "dokunemon", "도쿠네몬");
            SeedDigimonType(context, 32034, "MechaNorimon", null, "톱니몬(메카노몬)", "mechanorimon", "톱니몬메카노몬");
            SeedDigimonType(context, 31034, "Gardromon", null, "톱니몬(가드로몬)", "gardromon", "톱니몬가드로몬");
            SeedDigimonType(context, 31036, "Dorumon", null, "돌몬(돌가몬)", "dorumon", "돌몬(돌가몬)");
            SeedDigimonType(context, 32036, "Dorumon[Dex]", null, "돌몬(데크스돌가몬)", "dorumondex", "돌몬데크스돌가몬");
            SeedDigimonType(context, 33036, "Dorumon(Reptiledramon)", null, "돌몬(라프타드라몬계열)", "dorumonreptiledramon", "돌몬라프타드라몬계열");
            SeedDigimonType(context, 31006, "Renamon", null, "레나몬", "renamon", "레나몬");
            SeedDigimonType(context, 31010, "Terriermon", null, "테리어몬", "terriermon", "테리어몬");
            SeedDigimonType(context, 31049, "Elecmon", null, "에렉몬", "elecmon", "에렉몬");
            SeedDigimonType(context, 31050, "Gomamon", null, "쉬라몬", "gomamon", "쉬라몬");
            SeedDigimonType(context, 31028, "Drimogemon", null, "두리몬", "drimogemon", "두리몬");
            SeedDigimonType(context, 31039, "Dracumon", null, "드라큐몬", "dracumon", "드라큐몬");
            SeedDigimonType(context, 31031, "Tentomon", null, "텐타몬", "tentomon", "텐타몬");
            SeedDigimonType(context, 31018, "Gotsumon", null, "울퉁몬", "gotsumon", "울퉁몬");
            SeedDigimonType(context, 31048, "Biyomon", null, "피요몬", "biyomon", "피요몬");
            SeedDigimonType(context, 31066, "Impmon", null, "임프몬", "impmon", "임프몬");
            SeedDigimonType(context, 31017, "Keramon", null, "케라몬", "keramon", "케라몬");
            SeedDigimonType(context, 31020, "Hawkmon", null, "호크몬", "hawkmon", "호크몬");
            SeedDigimonType(context, 21134, "DemiMeramon", null, "페티메라몬", "demimeramon", "페티메라몬");
            SeedDigimonType(context, 31011, "Monodramon", null, "모노드라몬", "monodramon", "모노드라몬");
            SeedDigimonType(context, 41132, "Kiwimon", null, "키위몬", "kiwimon", "키위몬");
            SeedDigimonType(context, 31012, "Patamon", null, "파닥몬", "patamon", "파닥몬");
            SeedDigimonType(context, 31027, "Ryuudamon", null, "류우다몬", "ryuudamon", "류우다몬");
            SeedDigimonType(context, 41033, "Dobermon", null, "도베르몬", "dobermon", "도베르몬");
            SeedDigimonType(context, 41159, "Deputymon", null, "카우보이몬", "deputymon", "카우보이몬");
            SeedDigimonType(context, 31121, "Bearmon", null, "베어몬", "bearmon", "베어몬");
            SeedDigimonType(context, 31005, "Guilmon", null, "길몬", "guilmon", "길몬");
            SeedDigimonType(context, 31022, "Candlemon", null, "캔들몬", "candlemon", "캔들몬");
            SeedDigimonType(context, 31035, "CommanDramon", null, "코만드라몬", "commandramon", "코만드라몬");
            SeedDigimonType(context, 31015, "Lopmon", null, "로프몬", "lopmon", "로프몬");
            SeedDigimonType(context, 41088, "Starmon", null, "스타몬", "starmon", "스타몬");
            SeedDigimonType(context, 31142, "Wormmon", null, "추추몬", "wormmon", "추추몬");
            SeedDigimonType(context, 41075, "Gizumon", null, "기즈몬", "gizumon", "기즈몬");
            SeedDigimonType(context, 31026, "Betamon", null, "베타몬", "betamon", "베타몬");
            SeedDigimonType(context, 41146, "Doggymon", null, "도그몬", "doggymon", "도그몬");
            SeedDigimonType(context, 21137, "Tanemon", null, "시드몬", "tanemon", "시드몬");
            SeedDigimonType(context, 31016, "FanBeemon", null, "아기벌몬", "fanbeemon", "아기벌몬");
            SeedDigimonType(context, 31051, "Kamemon", null, "카메몬", "kamemon", "카메몬");
            SeedDigimonType(context, 31023, "Kudamon", null, "쿠다몬", "kudamon", "쿠다몬");
            SeedDigimonType(context, 31030, "Armadillomon", null, "아르마몬", "armadillomon", "아르마몬");
            SeedDigimonType(context, 31042, "Mushroomon", null, "머슈몬", "mushroomon", "머슈몬");
            SeedDigimonType(context, 31037, "Arkadimon", null, "알카디몬", "arkadimon", "알카디몬");
            SeedDigimonType(context, 33067, "Myotismon", null, "피코데블몬(묘티스몬)", "myotismon", "피코데블몬묘티스몬");
            SeedDigimonType(context, 33069, "Tsukaimon", null, "츄카이몬", "tsukaimon", "츄카이몬");
            SeedDigimonType(context, 33068, "Gazimon", null, "가지몬", "gazimon", "가지몬");
            SeedDigimonType(context, 31040, "Swimmon", null, "스윔몬", "swimmon", "스윔몬");

            SeedDigimonType(context, 33003, "Dorulumon", null, "도루루몬", "dorulumon", "도루루몬");
            SeedDigimonType(context, 45160, "Lucemon", null, "루체몬", "lucemon", "루체몬");
            SeedDigimonType(context, 33002, "Ballistamon", null, "바리스타몬", "ballistamon", "바리스타몬");
            SeedDigimonType(context, 45166, "Baihumon", null, "백호몬", "baihumon", "백호몬");
            SeedDigimonType(context, 35145, "Psychemon", null, "사이케몬", "psychemon", "사이케몬");
            SeedDigimonType(context, 33001, "Shoutmon", null, "샤우트몬", "shoutmon", "샤우트몬");
            SeedDigimonType(context, 71080, "Shoutmon", "X2", "샤우트몬X2", "shoutmonx2", "샤우트몬X2");
            SeedDigimonType(context, 71087, "Shoutmon", "X3", "샤우트몬X3", "shoutmonx3", "샤우트몬X3");
            SeedDigimonType(context, 31147, "Syakomon", null, "샤코몬", "syakomon", "샤코몬");
            SeedDigimonType(context, 31052, "Gomamon", "Jijimon", "쉬라몬(할배몬계열)", "gomamonjijimon", "쉬라몬할배몬계열");
            SeedDigimonType(context, 31144, "Armadillomon", "Shakkoumon", "아르마몬(토우몬계열)", "armadillomonshakkoumon", "아르마몬토우몬계열");
            SeedDigimonType(context, 35143, "Otamamon", null, "올챙몬", "otamamon", "올챙몬");
            SeedDigimonType(context, 45163, "Zhuqiaomon", null, "주작몬", "zhuqiaomon", "주작몬");
            SeedDigimonType(context, 45164, "Quinglongmon", null, "청룡몬", "quinglongmon", "청룡몬");
            SeedDigimonType(context, 33070, "Tsukaimon", "Murmukusmon", "츄카이몬(무크스몬계열)", "tsukaimonmurmukusmon", "츄카이몬무크스몬계열");
            SeedDigimonType(context, 35144, "ToyAgumon", null, "토이아구몬", "toyagumon", "토이아구몬");
            SeedDigimonType(context, 31143, "Patamon", "Shakkoumon", "파닥몬(토우몬계열)", "patamonshakkoumon", "파닥몬토우몬계열");
            SeedDigimonType(context, 35139, "Salamon", "Lilithmon", "플롯트몬(리리스몬계열)", "salamonlilithmon", "플롯트몬리리스몬계열");
            SeedDigimonType(context, 31146, "Salamon", "Silphymon", "플롯트몬(실피드몬계열)", "salamonsilphymon", "플롯트몬실피드몬계열");
            SeedDigimonType(context, 45165, "Xuanwumon", null, "현무몬", "xuanwumon", "현무몬");
            SeedDigimonType(context, 31145, "Hawkmon", "Silphymon", "호크몬(실피드몬계열)", "hawkmonsilphymon", "호크몬실피드몬계열");
        }

        private void SeedDigimonType(MainContext context, int code, string name, string nameAlt, string nameKorean, string searchGDMO, string searchKDMO) {
            SeedDigimonType(context, code, false, name, nameAlt, nameKorean, searchGDMO, searchKDMO);
        }

        private void SeedDigimonType(MainContext context, int code, bool isStarter, string name, string nameAlt, string nameKorean, string searchGDMO, string searchKDMO) {
            SeedDigimonType(context, code, false, 0, name, nameAlt, nameKorean, searchGDMO, searchKDMO);
        }

        private void SeedDigimonType(MainContext context, int code, bool isStarter, double sizeCm, string name, string nameAlt, string nameKorean, string searchGDMO, string searchKDMO) {
            context.DigimonTypes.Add(new DigimonType() {
                Code = code,
                IsStarter = isStarter,
                SizeCm = sizeCm,
                Name = name,
                NameAlt = nameAlt,
                NameKorean = nameKorean,
                SearchGDMO = searchGDMO,
                SearchKDMO = searchKDMO
            });
        }

        #endregion Digimon Types Seed

        #region Tamer Types Seed

        private void SeedTamerTypes(MainContext context) {
            SeedTamerType(context, 80001, "Marcus Damon");
            SeedTamerType(context, 80002, "Thomas H. Norstein");
            SeedTamerType(context, 80003, "Yoshino Fujieda");
            SeedTamerType(context, 80004, "Keenan Krier");
            SeedTamerType(context, 80005, "Taichi Kamiya");
            SeedTamerType(context, 80006, "Tachikawa Mimi");
            SeedTamerType(context, 80007, "Ishida Yamato");
            SeedTamerType(context, 80008, "Takaishi Takeru");
            SeedTamerType(context, 80009, "Sora Takenouchi");
            SeedTamerType(context, 80010, "Hikari «Kari» Kamiya");
        }

        private void SeedTamerType(MainContext context, int code, string name) {
            context.TamerTypes.Add(new TamerType() {
                Code = code,
                Name = name
            });
        }

        #endregion Tamer Types Seed
    }
}