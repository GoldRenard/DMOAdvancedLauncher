using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using AdvancedLauncher.Environment;

namespace AdvancedLauncher.Pages {

    public abstract class AbstractPage : UserControl {
        private Storyboard ShowWindow;

        protected abstract void InitializeAbstractPage();

        public AbstractPage() {
            InitializeAbstractPage();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
                LauncherEnv.Settings.ProfileChanged += ProfileChanged;
                ProfileChanged();
                LanguageEnv.Languagechanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
            }
        }

        public virtual void PageActivate() {
            ShowWindow.Begin();
        }

        public virtual void PageClose() {
        }

        protected abstract void ProfileChanged();
    }
}