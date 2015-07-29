using System.Collections.Generic;
using AdvancedLauncher.SDK.Management;

namespace AdvancedLauncher.SDK.Model.Web {

    public abstract class AbstractNewsProvider : INewsProvider {

        protected ILogManager LogManager {
            get;
            private set;
        }

        public AbstractNewsProvider() {
        }

        public AbstractNewsProvider(ILogManager logManager) {
            Initialize(logManager);
        }

        public abstract List<NewsItem> GetNews();

        public void Initialize(ILogManager logManager) {
            this.LogManager = logManager;
        }
    }
}