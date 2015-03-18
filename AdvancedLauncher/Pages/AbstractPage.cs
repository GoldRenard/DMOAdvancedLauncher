using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using AdvancedLauncher.Environment;

namespace AdvancedLauncher.Pages {

    public abstract class AbstractPage : UserControl {

        protected abstract void InitializeAbstractPage();

        public AbstractPage() {
            InitializeAbstractPage();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                LauncherEnv.Settings.ProfileChanged += ProfileChanged;
                ProfileChanged();
                LanguageEnv.LanguageChanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
            }
        }

        public virtual void PageActivate() {
        }

        public virtual void PageClose() {
        }

        protected abstract void ProfileChanged();
    }
}