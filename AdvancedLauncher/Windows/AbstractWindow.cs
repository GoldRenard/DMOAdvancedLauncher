using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using AdvancedLauncher.Environment;

namespace AdvancedLauncher.Windows {

    public abstract class AbstractWindow : UserControl {
        protected Storyboard ShowWindow, HideWindow;

        protected abstract void InitializeAbstractWindow();

        public AbstractWindow() {
            InitializeAbstractWindow();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                ShowWindow = ((Storyboard)this.FindResource("ShowWindow"));
                HideWindow = ((Storyboard)this.FindResource("HideWindow"));
                LanguageEnv.Languagechanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
            }
        }

        public virtual void Show() {
            this.Visibility = Visibility.Visible;
            ShowWindow.Begin();
        }

        public virtual void Close() {
            HideWindow.Begin();
        }

        protected virtual void OnCloseClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}