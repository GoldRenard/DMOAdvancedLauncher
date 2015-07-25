using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AdvancedLauncher.Controls {
    public class EnabledTextBlock : TextBlock {

        static EnabledTextBlock() {
            UIElement.IsEnabledProperty.OverrideMetadata(typeof(EnabledTextBlock), new UIPropertyMetadata(true, (d, e) => {
                var childrencount = VisualTreeHelper.GetChildrenCount(d);
                for (int i = 0; i < childrencount; i++) {
                    var child = VisualTreeHelper.GetChild(d, i);
                    child.CoerceValue(UIElement.IsEnabledProperty);
                }
            }, (d, basevalue) => {
                return basevalue;
            }));
        }
    }
}
