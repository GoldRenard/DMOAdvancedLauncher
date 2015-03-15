using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace AdvancedLauncher.Windows {
    /// <summary>
    /// Логика взаимодействия для TestWindow.xaml
    /// </summary>
    public partial class TestWindow : MetroWindow {
        public TestWindow() {
            InitializeComponent();
            this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            var t = this.WindowStyle;


            int i = 0;
        }
    }
}
