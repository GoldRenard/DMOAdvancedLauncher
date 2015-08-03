using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.Model.Proxy {

    public class WebProviderEventProxy<T> : CrossDomainObject
        where T : IWebProviderEventAccessor {
        private readonly T Object;

        public WebProviderEventProxy(T Object) {
            this.Object = Object;
        }

        public void OnDownloadStarted(object sender, SDK.Model.Events.EventArgs e) {
            Object.OnDownloadStarted(sender, e);
        }

        public void OnDownloadCompleted(object sender, DownloadCompleteEventArgs e) {
            Object.OnDownloadCompleted(sender, e);
        }

        public void OnStatusChanged(object sender, DownloadStatusEventArgs e) {
            Object.OnStatusChanged(sender, e);
        }
    }
}