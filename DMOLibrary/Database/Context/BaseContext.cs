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

using System.Data.Entity;
using System.Linq;
using DMOLibrary.Database.Entity;

namespace DMOLibrary.Database.Context {

    public abstract class BaseContext : DbContext {
        public static int REQUIRED_DATABASE_VERSION = 1;
        private static bool INITIALIZED = false;

        public BaseContext()
            : base() {
            if (!INITIALIZED) {
                Initialize();
            }
        }

        public DbSet<VersionInfo> VersionInfoes {
            get;
            set;
        }

        public void Initialize() {
            // first of all we check existing VersionInfoes table
            // and manually create the table for it if it will fail
            try {
                this.VersionInfoes.Count();
            } catch {
                this.Database.ExecuteSqlCommand(VersionInfo.CREATE_QUERY);
            }

            int currentVersion = 0;
            if (this.VersionInfoes.Count() > 0) {
                currentVersion = this.VersionInfoes.Max(x => x.Version);
            }
            MigrationHelper mmSqliteHelper = new MigrationHelper();
            while (currentVersion < REQUIRED_DATABASE_VERSION) {
                currentVersion++;
                foreach (string migration in mmSqliteHelper.Migrations[currentVersion]) {
                    this.Database.ExecuteSqlCommand(migration);
                }
                this.VersionInfoes.Add(new VersionInfo() {
                    Version = currentVersion
                });
                this.SaveChanges();
            }
            INITIALIZED = true;
        }
    }
}