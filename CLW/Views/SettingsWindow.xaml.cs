using CLW.Services;
using CLW.ViewModel;
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
using System.ComponentModel;

namespace CLW
{
    /// <summary>
    /// Interaction logic for CLWSettingsWindow.xaml
    /// </summary>
    public partial class CLWSettingsWindow : Window
    {
        public CLWSettingsWindow()
        {
            InitializeComponent();

            
        }

        private void SavePrompt(object sender, CancelEventArgs e)
        {
            ConfigService.Instance.Save();
            e.Cancel = false;
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //Datagrd.ItemsSource = ((SettingsWindowViewModel)DataContext).PressetsDeclarations;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}
