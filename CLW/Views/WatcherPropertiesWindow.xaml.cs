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

namespace CLW.Views
{
    /// <summary>
    /// Interaction logic for WatcherPropertiesWindow.xaml
    /// </summary>
    public partial class WatcherPropertiesWindow : Window
    {
        public WatcherPropertiesWindow()
        {
            InitializeComponent();
        }

        private void PackIconMaterial_MouseUp(object sender, MouseButtonEventArgs e)
        {
            colorPickerPOPUP.IsOpen = !colorPickerPOPUP.IsOpen;
        }
    }
}
