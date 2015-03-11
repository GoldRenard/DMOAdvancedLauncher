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

using System.Collections.Generic;
using DMOLibrary.Database.Context;
using System.Linq;
using DMOLibrary.Database.Entity;
using System;

namespace DMOLibrary {

    public class Test {

        public static void DoTest() {
            Server server = MainContext.Instance.Servers.First(i => i.Type == Server.ServerType.GDMO);
            server.Guilds.Add(new Guild() {
                Name = "test guild gdmo"
            });
            Server server2 = MainContext.Instance.Servers.First(i => i.Type == Server.ServerType.KDMO);
            server2.Guilds.Add(new Guild() {
                Name = "test guild kdmo"
            });
            MainContext.Instance.SaveChanges();


            Guild guilds = MainContext.Instance.Guilds.First(i => i.Server.Type == Server.ServerType.GDMO);
            TamerType tamerType = MainContext.Instance.TamerTypes.First();
            DigimonType digimonType = MainContext.Instance.DigimonTypes.First();

            Tamer tamer = new Tamer() {
                Name = "GoldRenard",
                Type = tamerType
            };

            Digimon partner = new Digimon() {
                Tamer = tamer,
                Type = digimonType,
                Name = "Renamon"
            };
            //tamer.Partner = partner;

            guilds.Tamers.Add(tamer);
            tamer.Digimons.Add(partner);

            MainContext.Instance.SaveChanges();
            tamer.Partner = partner;
            MainContext.Instance.SaveChanges();

            Tamer tamer2 = MainContext.Instance.Tamers.First();

            int i1 = 1;
            i1++;

        }
    }
}