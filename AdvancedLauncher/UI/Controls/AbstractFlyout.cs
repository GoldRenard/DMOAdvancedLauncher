using AdvancedLauncher.Management.Interfaces;
using MahApps.Metro.Controls;
using Ninject;

namespace AdvancedLauncher.UI.Controls {

    public abstract class AbstractFlyout : Flyout {

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get; set;
        }

        [Inject]
        public ILanguageManager LanguageManager {
            get; set;
        }

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        public AbstractFlyout() {
            App.Kernel.Inject(this);
        }
    }
}