using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AdvancedLauncher.SDK.Model.Entity;

namespace AdvancedLauncher.SDK.Model.Web {

    public abstract class AbstractServersProvider : IServersProvider {
        protected ICollection<Server> _ServerList;

        public ICollection<Server> ServerList {
            get {
                if (_ServerList == null) {
                    _ServerList = CreateServerList();
                }
                return _ServerList;
            }
        }

        protected readonly Server.ServerType ServerType;

        public AbstractServersProvider() {
        }

        public AbstractServersProvider(Server.ServerType serverType) {
            this.ServerType = serverType;
        }

        public Server GetServerById(int serverId) {
            return ServerList.Where(i => i.Identifier == serverId).FirstOrDefault();
        }

        protected abstract ReadOnlyCollection<Server> CreateServerList();
    }
}