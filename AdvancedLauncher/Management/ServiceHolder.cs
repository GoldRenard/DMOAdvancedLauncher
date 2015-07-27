using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Model;
using Ninject;

namespace AdvancedLauncher.Management {

    public class ServiceHolder {

        public IProfileManager ProfileManager {
            get {
                return App.Kernel.Get<IProfileManager>();
            }
        }

        public ILauncherManager LauncherManager {
            get {
                return App.Kernel.Get<ILauncherManager>();
            }
        }

        public LanguageModel LanguageModel {
            get {
                return App.Kernel.Get<ILanguageManager>().Model;
            }
        }
    }
}