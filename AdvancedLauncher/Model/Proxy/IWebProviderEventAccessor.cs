using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.Model.Proxy {

    public interface IWebProviderEventAccessor : IEventAccessor {

        void OnDownloadStarted(object sender, SDK.Model.Events.EventArgs e);

        void OnDownloadCompleted(object sender, DownloadCompleteEventArgs e);

        void OnStatusChanged(object sender, DownloadStatusEventArgs e);
    }
}