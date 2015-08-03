using System.Windows;
using AdvancedLauncher.SDK.Model.Config;

namespace AdvancedLauncher.UI.Extension {

    public class GameModelContainer : DependencyObject {
        public static readonly DependencyProperty GameModelProperty = DependencyProperty.Register("GameModel", typeof(GameModel), typeof(GameModelContainer));

        public GameModel GameModel {
            get {
                return (GameModel)GetValue(GameModelProperty);
            }

            set {
                SetValue(GameModelProperty, value);
            }
        }
    }
}