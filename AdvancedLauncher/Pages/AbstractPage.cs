using System.Windows;
using System.Windows.Controls;
using AdvancedLauncher.Environment;

namespace AdvancedLauncher.Pages {

    public abstract class AbstractPage : UserControl {

        protected abstract void InitializeAbstractPage();

        protected bool IsPageActivated = false;

        protected bool IsPageVisible = false;

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
            IsPageActivated = true;
            IsPageVisible = true;
        }

        public virtual void PageClose() {
            IsPageVisible = false;
        }

        protected abstract void ProfileChanged();
    }
}