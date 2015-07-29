using System.Collections.ObjectModel;
using System.Linq;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.SDK.Model.Web;
using DMOLibrary.Database.Context;

namespace DMOLibrary.Profiles {

    public abstract class DatabaseServersProvider : AbstractServersProvider {

        public DatabaseServersProvider(Server.ServerType serverType) : base(serverType) {
        }

        protected override ReadOnlyCollection<Server> CreateServerList() {
            using (MainContext context = new MainContext()) {
                return new ReadOnlyCollection<Server>(context.Servers.Where(i => i.Type == ServerType).ToList());
            }
        }
    }
}