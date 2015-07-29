using System.Collections.Generic;
using AdvancedLauncher.SDK.Model.Entity;

namespace AdvancedLauncher.SDK.Model.Web {

    public interface IServersProvider {

        Server GetServerById(int serverId);

        ICollection<Server> ServerList {
            get;
        }
    }
}