using System.Windows.Controls;
using AdvancedLauncher.Management.Interfaces;
using Ninject;

namespace AdvancedLauncher.UI.Controls {

    public class AbstractUserControl : UserControl {

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

        public AbstractUserControl() {
            App.Kernel.Inject(this);
        }
    }
}