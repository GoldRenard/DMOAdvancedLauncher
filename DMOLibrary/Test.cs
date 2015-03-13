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
using DMOLibrary.Profiles.Joymax;
using DMOLibrary.Profiles.Korea;
using DMOLibrary.Database;

namespace DMOLibrary {

    public class Test {

        public static void DoTest() {
            Server server = MainContext.Instance.Servers.First(i => i.Type == Server.ServerType.KDMO && i.Name == "Lucemon");
            Guild guild = new KoreaWebProfile().GetGuild(server, "VirusBusters", false);
            MergeHelper.Merge(guild);
            guild = new KoreaWebProfile().GetGuild(server, "VirusBusters", true);
            MergeHelper.Merge(guild);
            int i1 = 0;
            i1++;
        }
    }
}