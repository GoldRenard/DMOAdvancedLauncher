using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Model.Events.Proxy {

    public abstract class EventProxy<T> : CrossDomainObject where T : BaseEventArgs {

        public abstract void Handler(object sender, T e);
    }
}