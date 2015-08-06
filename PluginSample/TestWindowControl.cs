using System.Windows.Controls;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Windows;

namespace PluginSample {

    public class TestWindowControl : AbstractWindowControl {

        public TestWindowControl(ILanguageManager LanguageManager, IWindowManager WindowManager) : base(LanguageManager, WindowManager) {
            Button btn = new Button();
            btn.Click += (s, e) => {
                Close();
            };
            btn.Content = LanguageManager.Model.CloseButton;
            AddChild(btn);
        }
    }
}