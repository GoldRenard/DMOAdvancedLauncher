using System.Windows.Controls;
using AdvancedLauncher.Management.Interfaces;
using Ninject;

namespace AdvancedLauncher.UI.Validation {

    public abstract class AbstractValidationRule : ValidationRule {

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get; set;
        }

        [Inject]
        public ILanguageManager LanguageManager {
            get; set;
        }

        public AbstractValidationRule() {
            App.Kernel.Inject(this);
        }
    }
}