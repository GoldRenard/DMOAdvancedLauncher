using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using AdvancedLauncher.Environment;

namespace AdvancedLauncher.Windows {

    public abstract class AbstractWindow : UserControl {

        protected abstract void InitializeAbstractWindow();

        public delegate void CloseEventHandler(object sender, EventArgs e);

        public event CloseEventHandler WindowClosed;

        public AbstractWindow() {
            InitializeAbstractWindow();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                LanguageEnv.Languagechanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
            }
        }

        public virtual void Show() {
            this.Visibility = Visibility.Visible;
        }

        public virtual void Close() {
            if (WindowClosed != null) {
                WindowClosed(this, new EventArgs());
            }
        }

        protected virtual void OnCloseClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}