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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CLW.Views
{
    /// <summary>
    /// Interaction logic for WatcherView.xaml
    /// </summary>
    public partial class WatcherView : UserControl
    {
        public WatcherView()
        {
            InitializeComponent();
        }



        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(WatcherView), new PropertyMetadata(false,HandleIsSelectedChanged));

        private static void HandleIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void userControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((ViewModel.WatcherViewModel)DataContext).OpenPropertiesWindow.Execute(null);
        }
    }
}
