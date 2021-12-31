using CLW.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CLW
{
    /// <summary>
    /// Interaction logic for NItemUC.xaml
    /// </summary>
    public partial class NItemUC : UserControl
    {
        private static int count;

        public NItemUC()
        {
            count++;
            Trace.WriteLine("instance # " + count.ToString());
            InitializeComponent();
        }





        // some DepProps got deleted as part of switching to MVVM

        // download logic moved to the ViewModel



        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {

          //code moved to NewItemViewModel 
          

        
        }

        private void userControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)Resources["FadeInMardReadButton"]).Begin();
        }

        private void userControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)Resources["FadeOutMardReadButton"]).Begin();
        }
    }
}
