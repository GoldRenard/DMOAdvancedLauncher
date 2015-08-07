using System;
using System.Windows.Controls;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Events.Proxy;

namespace AdvancedLauncher.SDK.UI {

    public class AbstractUserControl : UserControl {

        protected ILanguageManager LanguageManager {
            get;
            private set;
        }

        protected IWindowManager WindowManager {
            get;
            private set;
        }

        public IRemoteControl Container {
            get;
            protected set;
        }

        public AbstractUserControl(ILanguageManager LanguageManager, IWindowManager WindowManager) {
            if (LanguageManager == null) {
                throw new ArgumentException("LanguageManager cannot be null");
            }
            this.WindowManager = WindowManager;
            this.LanguageManager = LanguageManager;
            this.LanguageManager.LanguageChangedProxy(new BaseEventProxy(OnLanguageChanged));
        }

        private void OnLanguageChanged(object sender, BaseEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new BaseEventHandler((s, e2) => {
                    OnLanguageChanged(sender, e2);
                }), sender, e);
                return;
            }
            this.DataContext = LanguageManager.Model;
        }
    }
}