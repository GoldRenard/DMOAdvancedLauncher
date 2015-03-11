using System;
using System.Collections.Generic;
using System.Data.Entity;
using DMOLibrary.Database.Entity;

namespace DMOLibrary.Database.Context {

    public class ContextInitializer
#if DEBUG
        : DropCreateDatabaseAlways<MainContext> {
#endif
#if RELEASE
        : CreateDatabaseIfNotExists<MainContext> {
#endif
        protected override void Seed(MainContext context) {
            SeedServers(context);
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
    }
}