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
using System.Data.Entity;
using DMOLibrary.Database.Entity;

namespace DMOLibrary.Database.Context {

    public class MainContext : BaseContext {

        static MainContext() {
            System.Data.Entity.Database.SetInitializer<MainContext>(new ContextInitializer());
        }

        public DbSet<Server> Servers {
            get;
            set;
        }

        public static List<string> GetInitializeQueries() {
            List<String> queries = new List<string>();
            queries.Add("CREATE TABLE [Servers] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [Identifier] INTEGER(1) NOT NULL, [Name] TEXT NOT NULL, [Type] INTEGER NOT NULL)");
            queries.Add(String.Format("INSERT INTO Servers([Identifier], [Name], [Type]) VALUES (1, 'Seraphimon', {0})", (int)Server.ServerType.ADMO));
            queries.Add(String.Format("INSERT INTO Servers([Identifier], [Name], [Type]) VALUES "
                + "(1, 'Leviamon', {0}),"
                + "(2, 'Lucemon', {0}),"
                + "(3, 'Lilithmon', {0}),"
                + "(4, 'Barbamon', {0}),"
                + "(5, 'Beelzemon', {0})",
                (int)Server.ServerType.GDMO));
            queries.Add(String.Format("INSERT INTO Servers([Identifier], [Name], [Type]) VALUES "
                + "(1, 'Lucemon', {0}),"
                + "(2, 'Leviamon', {0}),"
                + "(3, 'Lilithmon', {0}),"
                + "(4, 'Barbamon', {0})",
                (int)Server.ServerType.KDMO));

            queries.Add(String.Format("INSERT INTO Servers([Identifier], [Name], [Type]) VALUES "
                + "(1, 'Lucemon', {0}),"
                + "(2, 'Leviamon', {0}),"
                + "(3, 'Lilithmon', {0}),"
                + "(4, 'Barbamon', {0})",
                (int)Server.ServerType.KDMO_IMBC));
            return queries;
        }
    }
}